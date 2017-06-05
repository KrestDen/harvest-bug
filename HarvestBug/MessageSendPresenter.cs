using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace HarvestBug
{
    
    class MessageSendPresenter : ProcessObserver
    {
        MainForm m_view;
        Bot m_bot;
        DbHadler m_db;
        bool m_inProcess;
        HardcodedMessagePresenter m_hardcodedManeger;
        string m_captcha;

        List<BotWorker> m_workers;

        public AutoResetEvent CaptchaEntered;

        public string Captcha { get => m_captcha; set => m_captcha = value; }

        public MessageSendPresenter(MainForm view)
        {
            CaptchaEntered = new AutoResetEvent(false);

            m_view = view;
            m_db = new DbHadler("PhotoSender.db");
            m_inProcess = false;
            m_workers = new List<BotWorker>();
        }

        public async Task<int> UploadFromFile(string path, string sent)
        {
            return await Task<int>.Run(() =>
            {
                StreamReader reader = new StreamReader(path);
                List<string> users = new List<string>();
                while (reader.Peek() >= 0)
                {
                    string id = reader.ReadLine();
                    id = id.Replace(" ", "");
                    id = id.Replace("https:", "");
                    id = id.Replace("/", "");
                    id = id.Replace("vk.com/id", "");
                    id = id.Replace("vk.com", "");

                    if (id != "" && !m_db.UserExist(id))
                    {
                        users.Add(id);
                    }
                    m_db.AddUsersForSpam(users, sent);
                }
                NewMessage("Done");
                return users.Count;
            });
        }

        public void SetCapcha(string captcha)
        {
            m_captcha = captcha;
            CaptchaEntered.Set();
        }

        public async Task<List<string> > GetFreeBots()
        {
            return await Task<List<string>>.Run(() =>
            {
                List<string> busyBots = new List<string>();
                m_db.GetBusyBots(ref busyBots);
                List<string> bots = new List<string>();
                m_db.GetBotsList(ref bots);

                List<string> freeBots = new List<string>();
                foreach (string bot in bots)
                {
                    if (!busyBots.Exists(item => item == bot))
                    {
                        freeBots.Add(bot);
                    }
                }
                //todo feel
                return freeBots;
            });
        }

        public void FillBotControls(ref List<BotControl> botControls)
        {
            Dictionary<string, TaskData> tasks = new Dictionary<string, TaskData>();
            m_db.GetTasks(ref tasks);

            foreach (KeyValuePair<string, TaskData> task in tasks)
            {
                BotControl botControl = new BotControl(task.Key, task.Value.max, task.Value.current);
                botControls.Add(botControl);
                BotWorker botWorker = new BotWorker(task.Value, ref m_db, task.Key, botControl, this);
                m_workers.Add(botWorker);
            }
        }

        public BotControl FillBotControl(string botLogin, TaskData task)
        {
            BotControl botControl = new BotControl(botLogin, task.max, task.current);
            BotWorker botWorker = new BotWorker(task, ref m_db, botLogin, botControl, this);
            m_workers.Add(botWorker);
            return botControl;
        }

        public void NeedCaptcha(string capchaUrl)
        {
            m_view.NeedCaptcha(capchaUrl);
        }

        public async void AddNewTask(string botLogin, TaskData task)
        {
            await Task.Run(() =>
            {
                m_db.AddTask(botLogin, task);
            });
        }

        public void ChangeMaxValue(string login, string max)
        {
            m_db.ChangeMaxValue(login, max);
        }

        public async void ReplaceDisplayName()
        {
            await Task.Run(() =>
            {
                Dictionary<string, string> users = new Dictionary<string, string>();
                m_db.GetUsersList(ref users);

                string token = m_db.GetSavedToken("05036@i.ua");
                if (token == "")
                {
                    Bot.GetRefreshToken("05036@i.ua", "12345lebed12345", this);
                    m_db.SaveToken("05036@i.ua", "12345lebed12345", token);
                }
                m_bot = new Bot("05036@i.ua", token, "205164899", ref m_db, this);
                foreach (KeyValuePair<string, string> user in users)
                {
                    if (Convert.ToUInt32(user.Key) > 202)
                    {
                        NewMessage("Start " + user);
                        User userData = m_bot.GetUser(user.Value);
                        if (user.Value != userData.userId && userData.userId != "")
                        {
                            m_db.Replace(user.Key, userData.userId);
                            NewMessage("Replaced with  " + userData.userId);
                        }
                        NewMessage("Finished");
                    }
                }
            });
        }

        public async Task<bool> SaveCredantials(string login, string password)
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    string token = m_db.GetSavedToken(login);
                    if (token == "" /* || expired */)
                    {
                        token = Bot.GetRefreshToken(login, password, this);
                        if (token == null)
                        {
                            NewMessage("Login " + login + "; password  " + password + "Error while getting token ");
                            return false;
                        }
                        m_db.SaveToken(login, password, token);
                    }
                    NewMessage("Login " + login + "; password  " + password + " Ready " + token);
                    m_bot = new Bot(login, token, "205164899", ref m_db, this);
                }
                catch(Exception ex)
                {
                    NewMessage("Error while login in " + login + "; password  " + password + ex.ToString());
                }
                return true;
            });
        }

        public async void StartSending()
        {
            m_inProcess = true;
            //m_hardcodedManeger.SendMessages();
            await Task.Run(() =>
            {
                try
                {
                    while (!AllBotsFinished())
                    {
                        foreach (BotWorker worker in m_workers)
                        {
                            if (!worker.IsFinished())
                            {
                                worker.DoTask();
                                worker.Bot.Timeout();
                            }
                        }
                    }
                    m_inProcess = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                m_view.SetBusy(false);
                return;
            });
        }
                    
        private bool AllBotsFinished()
        {
            foreach (BotWorker worker in m_workers)
            {
                if (!worker.IsFinished())
                {
                    return false;
                }
            }

            return true;
        }
        public bool IsBuisy()
        {
            return m_inProcess;
        }

        public void NewMessage(string msg)
        {            
            string logMsg = GetTimeStr() + " " + msg;
            m_view.WriteToConsole(logMsg);
            Log(logMsg);
        }

        public void OnFinish()
        {
            m_inProcess = false;
        }

        private string GetTimeStr()
        {
            return '[' + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ']';
        }

        public void ResetCounters()
        {
            foreach (BotWorker worker in m_workers)
            {
                worker.ResetCounters();

            }
            m_db.ResetCounters();
        }

        private void Log(string msg)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true))
            {
                writer.WriteLine(msg);
            }
        }

        public async void AddUsersForSpam(string publicId)
        {
            await Task.Run(() =>
            {
                List<string> users = new List<string>();
                string token = m_db.GetSavedToken("05036@i.ua");
                if (token == "" /* || expired */)
                {
                    token = Bot.GetRefreshToken("05036@i.ua", "12345lebed12345", this);
                    m_db.SaveToken("05036@i.ua", "12345lebed12345", token);
                }
                NewMessage("Token " + token);
                m_bot = new Bot("05036@i.ua", token, "205164899", ref m_db, this);
                new System.Threading.Thread(delegate ()
                {
                    NewMessage("Start getting users from " + publicId);
                    m_bot.GetMembers(publicId, out users);
                    NewMessage("Finished getting users from " + publicId);
                    m_db.AddUsersForSpam(users, "0");
                    NewMessage("Finished adding users to database from " + publicId);
                }).Start();
            });
        }
    }
}
