using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarvestBug
{
    public partial class BotContainer : UserControl
    {

        public delegate void MaxValueChangedDelegate(string login, string value);
        public event MaxValueChangedDelegate maxValueChangedEvent;
        public BotContainer()
        {
            InitializeComponent();
        }

        public void InsertBotControl(BotControl control)
        {
            Controls.Add(control);
            control.reiterationChangedIvent += MaxValueChanged;
        }

        private void BotContainer_Load(object sender, EventArgs e)
        {

        }

        public void  ResetCounters()
        {
            foreach (var botControl in Controls)
            {
                ((BotControl)botControl).ResetCounters();
            }
        }

        private void MaxValueChanged(string login, string max)
        {
            maxValueChangedEvent(login, max);
        }
    }
}
