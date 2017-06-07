using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestBug
{
    class HardcodedMessagePresenter
    {
        private Dictionary<Bot, KeyValuePair<Message, int>> m_dict;
        private int m_messagesForEachBot = 5;
        private DbHadler m_dbHandler;
        private ProcessObserver m_observer; 

        public HardcodedMessagePresenter()
        {
            //FillDict();
        }

        public void SendMessages()
        {
            while (!IsAllBotsFinished())
            {
                foreach (KeyValuePair<Bot, KeyValuePair<Message, int>> bot in m_dict)
                {
                    try
                    {
                        if (bot.Value.Value < m_messagesForEachBot)
                        {
                            string nextUserId = m_dbHandler.GetNextUserIDForSpam();

                            // 2. bot Get User info (uiserID)
                            User nextUSerForSpam = bot.Key.GetUser(nextUserId);
                            if (nextUSerForSpam.online == "1" && nextUSerForSpam.can_write_private_message == "1")
                            {
                                // 3. Prepare message 
                                Message msg = bot.Value.Key;
                                GetMessageObj(nextUSerForSpam, ref msg);

                                // 4. bot send Message(messge)
                                bot.Key.SendMsg(nextUserId, msg);

                                // 5. Write status to UI
                                m_observer.NewMessage("Message has been sent from " + bot.Key.ToString() + "to " + nextUSerForSpam.ToString());

                                // 6. Mark userID as sended
                                m_dbHandler.MarkUserAsDone(nextUserId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        m_observer.NewMessage("Error: " + ex.ToString());
                    }
                }
            }
            
            m_observer.OnFinish();
        }

        private void GetMessageObj(User nextUSerForSpam, ref Message message)
        {
            string generalMsg = message.message;
            message.message = "Добрый день, " + nextUSerForSpam.first_name + " " + nextUSerForSpam.last_name + ". " + generalMsg;
        }

        private bool IsAllBotsFinished()
        {
            foreach (KeyValuePair<Bot, KeyValuePair<Message, int>> bot in m_dict)
            {
                if (bot.Value.Value < m_messagesForEachBot)
                {
                    return false;
                }
            }

            return true;
        }

        private Message GetMessage(string message, string attachments)
        {
            Message msg = new Message();
            msg.message = message;
            msg.attachments = attachments;

            return msg;
        }

        private string GetGeneralMsg()
        {
            return ".\nСервис ФОТОМАГНИТ предлагает Вам принять участие в акции и гарантировано получить фотомагнит бесплатно.\nhttps://vk.com/club_photo_magnit?w=wall-53332601_1057%2Fall";
        }
    }
}
