using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarvestBug
{
    public delegate void InvokeDelegate();
    public delegate void InvokeDelegateStr(string str);

    public partial class MainForm : Form
    {
        private MessageSendPresenter m_presenter;
        private string m_lastMsg;
        private CaptchaForm m_captchaForm;
        private NewBotWindow m_newBotWindow;
        static AutoResetEvent CaptchaEntered;
        private AddTaskWindow m_addTaskWindow;
        private InsertLinkWindow m_insertLinkWindow;

        public MainForm()
        {
            CaptchaEntered = new AutoResetEvent(false);
            InitializeComponent();

            m_presenter = new MessageSendPresenter(this);
            m_captchaForm = new CaptchaForm();
            m_newBotWindow = new NewBotWindow();
            m_addTaskWindow = new AddTaskWindow();
            m_insertLinkWindow = new InsertLinkWindow();
            m_newBotWindow.newBotEntered += OnNewBot;
            m_captchaForm.CaptchaEntered += SetCapcha;
            m_addTaskWindow.taskEnteredEvent += OnNewTask;
            m_botContainer.maxValueChangedEvent += OnMaxValueChanged;
            m_insertLinkWindow.linkAddedEvent += OnAddNewUsers;

            FillBotContainer();
        }

        private void SetCapcha(string captcha)
        {
            m_presenter.SetCapcha(captcha);
        }

        public void WriteToConsole(string msg)
        {
            this.Invoke((Action)delegate
            {
                listLog.Items.Add(msg);
            });         
        }

        private void OnMaxValueChanged(string login, string maxValue)
        {
            m_presenter.ChangeMaxValue(login, maxValue);
        }

        private void OnNewTask(string botLogin, TaskData task)
        {
            m_presenter.AddNewTask(botLogin, task);
            AddTask(botLogin, task);
        }

        private void AddTask(string botLogin, TaskData task)
        {
            BotControl control = m_presenter.FillBotControl(botLogin, task);
            control.Dock = DockStyle.Top;
            m_botContainer.InsertBotControl(control);
            m_botContainer.Update();
        }

        public void NeedCaptcha(string captchaUrl)
        {
            this.Invoke((Action)delegate
            {
                m_captchaForm.Show(captchaUrl);
            });
        }

        private async void OnNewBot(string login, string password)
        {
            if (login != "" && password != "")
            {
                login = login.Replace("+", "");
                await m_presenter.SaveCredantials(login, password);
            }
            
            button1.Enabled = true;
        }
        private void button1_Click_2(object sender, EventArgs e)
        {
            button1.Enabled = false;
            m_newBotWindow.Show();        
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!m_presenter.IsBuisy())
            {
                SetBusy(true);
                 m_presenter.StartSending();
            }
            else
            {
                MessageBox.Show("Already running");
            }
        }

        public void SetBusy(bool busy)
        {
            this.Invoke((Action)delegate
            {
                btnAddTask.Enabled = !busy;
                btnResetCounters.Enabled = !busy;
                btnStart.Enabled = !busy;
                button1.Enabled = !busy;
            });
        }

        private async void btnAddTask_Click(object sender, EventArgs e)
        {
            List<string> freeBots = await m_presenter.GetFreeBots();

            m_addTaskWindow.Show(freeBots);
        }

        private void FillBotContainer()
        {
            List<BotControl> botControls = new List<BotControl>();
            m_presenter.FillBotControls(ref botControls);

            foreach(BotControl control in botControls)
            {
                control.Dock = DockStyle.Top;
                m_botContainer.InsertBotControl(control);
            }
        }

        private async void btnResetCounters_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                m_botContainer.ResetCounters();
                m_presenter.ResetCounters();
            });
        }

        private void btnAddUsers_Click(object sender, EventArgs e)
        {
             m_insertLinkWindow.Show();
           // m_presenter.Test();
        }

        private void OnAddNewUsers(string link)
        {
            if (link != "")
            {
                m_presenter.AddUsersForSpam(link);
            }
            else
            {
                WriteToConsole("Empty link");
            }
        }
    }
}