using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarvestBug
{
    public partial class CaptchaForm : Form
    {
        public delegate void CaptchaDelegate(string msg);
        public event CaptchaDelegate CaptchaEntered;

        public CaptchaForm()
        {
            InitializeComponent();
        }

        public void Show(string captchaLink)
        {
            lblCaptcha.Text = captchaLink;
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = captchaLink;
            lblCaptcha.Links.Add(link);
            Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            CaptchaEntered(textBoxCaptcha.Text);
            textBoxCaptcha.Clear();
            lblCaptcha.Text = "";
            Hide();
        }

        private void lblCaptcha_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
            lblCaptcha.Links.Clear();
        }
    }
}
