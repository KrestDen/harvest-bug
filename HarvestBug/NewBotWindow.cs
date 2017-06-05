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
    public partial class NewBotWindow : Form
    {
        public delegate void NewBot(string login, string password);
        public event NewBot newBotEntered;
        
        public NewBotWindow()
        {
            InitializeComponent();
        }
                
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            newBotEntered(textBoxLogin.Text, textBoxPassword.Text);
            textBoxPassword.Clear();
            textBoxLogin.Clear();
            Hide();
        }
    }
}
