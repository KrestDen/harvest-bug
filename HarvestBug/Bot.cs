using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;
using System.IO;

namespace HarvestBug
{
    class Bot
    {
        private string m_login;
       // private string m_userId;
        private string m_token;
        private DbHadler m_db;
        private MessageSendPresenter m_observer;
        Random m_rand = new Random();

        public string Login { get => m_login; set => m_login = value; }

        public override string ToString() 
        {
            return m_login;
        }

        public Bot(string login, ref DbHadler db, MessageSendPresenter observer)
        {
            m_login = login;
            
           // m_userId = userId;
            m_db = db;
            m_token = m_db.GetSavedToken(login);
            if (m_token ==  null && m_token == "" )
            {
                throw new Exception("Registration Required");
            }
            m_observer = observer;
        }

        public Bot(string login, string token, string userId, ref DbHadler db, MessageSendPresenter observer)           
        {
            m_login = login;
            m_token = token;
           // m_userId = userId;
            m_db = db;
            m_observer = observer;
        }

        public void SendMsg(string userId, Message msg)
        {
            string captcha = "";
            string captchaSig = "";

            bool complete = false;
            while (!complete)
            {
                Task<string> sendMsgTask = GetMsgUrl(userId, msg, captchaSig, captcha);
                sendMsgTask.Wait();

                string responseStr = sendMsgTask.Result;
                dynamic responseParser = JObject.Parse(responseStr);
                string msgId = responseParser.response;
                if (msgId == null || msgId == "")
                {
                    if (responseParser.error.error == "need_captcha")
                    {
                        if (captcha != "")
                        {
                            m_observer.NewMessage("Invalid captcha");
                        }
                        else
                        {
                            m_observer.NewMessage("Captha required for " + m_login);
                        }
                        string capchUrl = responseParser.captcha_img;
                        captchaSig = responseParser.captcha_sid;
                        captcha = GetCaptcha(capchUrl, m_observer);
                    }
                    else
                    {
                        m_observer.NewMessage("Unknown error ");
                        complete = true;
                    }
                }
                else
                {
                    complete = true;
                }

                m_observer.NewMessage(sendMsgTask.Result);
            }
        }

        public static /*async*/ string GetRefreshToken(string userName, string pass, MessageSendPresenter presenter)
        {
            presenter.NewMessage("Started to get auth token for " + userName);
            bool needToSend = true;
            string captcha = "";
            string captchaSid = "";
            string refreshToken = "";
            while (needToSend)
            {                
                var client = new HttpClient();

                FormUrlEncodedContent requestContent;
                if (captcha == "")
                {
                    requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", "3697615"),
                        new KeyValuePair<string, string>("client_secret", "AlVXZFMUqyrnABp8ncuU"),
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", pass),
                    });
                }
                else
                {
                    requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                   {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", "3697615"),
                        new KeyValuePair<string, string>("client_secret", "AlVXZFMUqyrnABp8ncuU"),
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", pass),
                        new KeyValuePair<string, string>("captcha_sid", captchaSid),
                        new KeyValuePair<string, string>("captcha_key", captcha)
                   });
                }

                var t = client.PostAsync("https://oauth.vk.com/token", requestContent);
                t.Wait();
                presenter.NewMessage("Finished getting auth token for " + userName);
                HttpResponseMessage response = t.Result;
                HttpContent responseContent = response.Content;
                var r = responseContent.ReadAsStreamAsync();
                r.Wait();

                using (StreamReader reader = new StreamReader(r.Result))
                {
                    string responseStr = /*await*/ reader.ReadToEnd();
                    dynamic getTokenParser = JObject.Parse(responseStr);
                    refreshToken = getTokenParser.access_token;
                    if (refreshToken == null || refreshToken == "")
                    {
                        if (getTokenParser.error == "need_captcha")
                        {
                            if (captcha != "")
                            {
                                presenter.NewMessage("Invalid captcha");
                            }
                            else
                            {
                                presenter.NewMessage("Captha required " + userName);
                            }
                            string capchUrl = getTokenParser.captcha_img;
                            captchaSid = getTokenParser.captcha_sid;
                            captcha = GetCaptcha(capchUrl, presenter);
                        }
                    }
                    else
                    {
                        needToSend = false;
                    }
                }
            }
            return refreshToken;
        }

        private static string GetCaptcha(string captchaUrl, MessageSendPresenter presenter)
        {
            presenter.NeedCaptcha(captchaUrl);
            presenter.CaptchaEntered.WaitOne();
            return presenter.Captcha;
        }

        public void GetMembers(string groupId, out List<string> members)
        {
            members = new List<string>();
            Task<string> membersCountTask = GetMembersTask(groupId, "0", "0");
            membersCountTask.Wait();

            dynamic finishParser = JObject.Parse(membersCountTask.Result);
            dynamic responseAdding = finishParser.response;
            
            string countStr = (string)responseAdding["count"];
            int count = Convert.ToInt32(countStr);

            int size = 1000;
            for (int i = 0; i < count; i+= size)
            {
                Task<string> membersTask = GetMembersTask(groupId, i.ToString(), size.ToString());
                membersCountTask.Wait();

                dynamic finishParser_2 = JObject.Parse(membersTask.Result);
                dynamic responseAdding_2 = finishParser_2.response;
                //  string responseAdding_2 = finishParser_2.response.ToString();
                //  dynamic finishParserResponse_2 = JArray.Parse(responseAdding);
                JArray items = (JArray)responseAdding_2["items"];
                members.InsertRange(members.Count, items.ToObject<List<string>>());
            }
        }

        private async Task<string> GetMembersTask(string groupId, string offset, string count)
        {
            m_observer.NewMessage("Started to get memembers of " + groupId);
            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                new KeyValuePair<string, string>("group_id", groupId),
                new KeyValuePair<string, string>("v", "5.29"),
                new KeyValuePair<string, string>("offset", offset),
                new KeyValuePair<string, string>("count", count),
                });
                var t = client.PostAsync("https://api.vk.com/method/groups.getMembers", requestContent);
                t.Wait();
                HttpResponseMessage response = t.Result;
                HttpContent responseContent = response.Content;
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    m_observer.NewMessage("Finished geting memembers of " + groupId);
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task<string> GetMsgUrl(string userId, Message msg, string captchaSig, string captcha)
        {
            m_observer.NewMessage("Started to send message to " + userId);
            using (var client = new HttpClient())
            {
                FormUrlEncodedContent requestContent;

                if (captcha == "")
                {
                    requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("user_id", userId),
                        new KeyValuePair<string, string>("v", "5.29"),
                        new KeyValuePair<string, string>("access_token", m_token),
                        new KeyValuePair<string, string>("message", msg.message),
                        new KeyValuePair<string, string>("attachment", msg.attachments),
                    });
                }
                else
                {
                    requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("user_id", userId),
                        new KeyValuePair<string, string>("v", "5.29"),
                        new KeyValuePair<string, string>("access_token", m_token),
                        new KeyValuePair<string, string>("message", msg.message),
                        new KeyValuePair<string, string>("attachment", msg.attachments),
                        new KeyValuePair<string, string>("captcha_sid", captchaSig),
                        new KeyValuePair<string, string>("captcha_key", captcha)
                    });
                }

                var t = client.PostAsync("https://api.vk.com/method/messages.send", requestContent);
                t.Wait();
                HttpResponseMessage response = t.Result;
                HttpContent responseContent = response.Content;
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    m_observer.NewMessage("Finished send message to " + userId);
                    return await reader.ReadToEndAsync();                    
                }
            }
        }
        
        public void Timeout()
        {
            int timeout = m_rand.Next(500, 5000);
            m_observer.NewMessage("Timeout " + timeout);
            Thread.Sleep(timeout);
        }

        public void LongTimeout()
        {
            int timeout = m_rand.Next(60000, 300000);
            m_observer.NewMessage("Timeout " + timeout);
            Thread.Sleep(timeout);
        }

        public User GetUser(string user)
        {
            Task<string> userDataTask = GetUserDataTask(user);
            userDataTask.Wait();

            dynamic finishParser = JObject.Parse(userDataTask.Result);
            if (finishParser.response == null)
            {
                return new User();
            }
            string responseAdding = finishParser.response.ToString();
            dynamic finishParserResponse = JArray.Parse(responseAdding);

            JObject responseArray = (JObject)finishParserResponse[0];
            User man = new User();
            man.userId = (string)responseArray["id"];
            man.first_name = (string)responseArray["first_name"];
            man.last_name = (string)responseArray["last_name"];
            man.domain = (string)responseArray["domain"];
            man.online = (string)responseArray["online"];
            man.can_write_private_message = (string)responseArray["can_write_private_message"];

            JObject cityVal = (JObject)responseArray["city"];
            if (cityVal != null)
            {
                man.city = (string)cityVal["title"];  
            }                      

            return man;
        }

        private async Task<string> GetUserDataTask(string userId)
        {
            m_observer.NewMessage("Started to get info about " + userId);
            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                new KeyValuePair<string, string>("user_ids", userId),
                new KeyValuePair<string, string>("v", "5.29"),
                //new KeyValuePair<string, string>("access_token", m_token),
                new KeyValuePair<string, string>("fields", "city,verified,online,can_write_private_message,screen_name,domain"),
                new KeyValuePair<string, string>("name_case", "Nom"),
                });
                var t = client.PostAsync("https://api.vk.com/method/users.get", requestContent);
                t.Wait();
                HttpResponseMessage response = t.Result;
                HttpContent responseContent = response.Content;
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    return await reader.ReadToEndAsync();
                    m_observer.NewMessage("Finshed geting info about " + userId);
                }
            }
        }
    }
}
