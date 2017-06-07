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
    public partial class BotControl : UserControl, BotObserver
    {
        public delegate void ReiterationChanged(string login, string val);
        public event ReiterationChanged reiterationChangedIvent;
        private string m_login;
        public BotControl(string login, string max, string curent)
        {
            InitializeComponent();
            m_login = login;
            lblLogin.Text = login;
            if (curent != null && curent != "")
            {
                lblCounter.Text = curent;
                progress.Value = Convert.ToUInt16(curent);
            }
            else
            {
                lblCounter.Text = "0";
            }
            if (max != "")
            {
                numericMax.Value = Convert.ToDecimal(max);
                progress.Maximum = Convert.ToUInt16(max);                
            }            
        }

        public void ReiterationComplete()
        {
            this.Invoke((Action)delegate
            {
                int currentVal = Convert.ToInt16(lblCounter.Text);
                ++currentVal;
                lblCounter.Text = currentVal.ToString();
                progress.Value = progress.Value + 1;
            });
        }
        public void TaskComplete()
        {
            //todo
        }

        private void lblLogin_Click(object sender, EventArgs e)
        {

        }

        public void ResetCounters()
        {
            this.Invoke((Action)delegate
            {
                lblCounter.Text = "0";
                UpdateProgress();
            });
        }

        private void UpdateProgress()
        {
            if (numericMax.Value <= Convert.ToInt16(lblCounter.Text))
            {
                progress.Maximum = Convert.ToInt16(numericMax.Value);
                progress.Value = progress.Maximum;
            }
            else
            {
                progress.Maximum = Convert.ToUInt16(numericMax.Value);
                progress.Update();
            }
        }

        private void numericMax_ValueChanged(object sender, EventArgs e)
        {
            UpdateProgress();

            if (reiterationChangedIvent != null)
            {
                reiterationChangedIvent(m_login, numericMax.Value.ToString());
            }
        }

        private void numericMax_Leave(object sender, EventArgs e)
        {
           
        }

        private void lblCounter_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
