using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestBug
{
    class BotWorker
    {
        Bot m_bot;
        BotControl m_observer;
        TaskData m_task;
        DbHadler m_db;

        public BotWorker(TaskData task, ref DbHadler db,  string login, BotControl observer,  MessageSendPresenter processObserver)
        {
            m_db = db;
            m_task = task;
            Bot = new Bot(login, ref db,  processObserver);
            m_observer = observer;
            m_observer.reiterationChangedIvent += MaxChanged;
        }

        internal Bot Bot { get => m_bot; set => m_bot = value; }
        
        public void ResetCounters()
        {
            m_task.max = "0";
            m_task.current = "0";
        }

        public string GetMsg()
        {
            return m_task.text;
        }

        private void MaxChanged(string login, string max)
        {
            m_task.max = max;
        }

        public void SetMsg(string msg)
        {

        }

        public bool IsFinished()
        {
            if (m_task.current == "")
            {
                return false;
            }
            if (m_task.max == "")
            {
                return true;
            }
            return Convert.ToInt16(m_task.max) <= Convert.ToInt16(m_task.current);
        }

        public string GetHello()
        {
            DateTime localDate = DateTime.Now;
            if (localDate.Hour < 12)
            {
                return "Доброе утро";
            }
            else if (localDate.Hour < 18)
            {
                return "Добрый день";
            }
            else
            {
                return "Добрый вечер";
            }
        }

        public void DoTask()
        {
            bool hasBeenSent = false;
            while (!hasBeenSent)
            {
                string userForSpam = m_db.GetNextUserIDForSpam();
                User nextUSerForSpam = m_bot.GetUser(userForSpam);
                if (nextUSerForSpam.online == "1" && nextUSerForSpam.can_write_private_message == "1")
                {
                    Message msg = new Message();
                    string generalMsg = m_task.text;                    
                    string decoratedMsg = GetHello() + ", " + " " + nextUSerForSpam.first_name + ". " + generalMsg;
                    msg.message = decoratedMsg;
                    msg.attachments = m_task.attachments;
                    Bot.SendMsg(userForSpam, msg);
                    m_db.MarkUserAsDone(userForSpam);
                    m_db.UpdateTaskCounter(Bot.Login);
                    m_observer.ReiterationComplete();
                    hasBeenSent = true;
                    m_task.current = (Convert.ToInt16(m_task.current) + 1).ToString();
                }
            }
        }
    }
}
