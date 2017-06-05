using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarvestBug
{
    public partial class AddTaskWindow : Form
    {
        public delegate void TaskEntered(string botLogin, TaskData data);
        public event TaskEntered taskEnteredEvent;

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        public void Show(List<string> bots)
        {
            comboBoxBots.Items.AddRange(bots.ToArray());
            Show();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            TaskData task = new TaskData();
            task.type = TaskType.message;
            task.text = textBoxMessage.Text;
            task.attachments = textBoxAttachments.Text;

            taskEnteredEvent(comboBoxBots.Text, task);

            OnHide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnHide();
        }

        private void OnHide()
        {
            textBoxMessage.Clear();
            textBoxAttachments.Clear();
            comboBoxBots.Items.Clear();
            comboBoxBots.Text = "";

            Hide();
        }
    }
}
