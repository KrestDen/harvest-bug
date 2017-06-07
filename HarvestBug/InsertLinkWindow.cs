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
    public partial class InsertLinkWindow : Form
    {
        public delegate void LinkAdded(string link);
        public event LinkAdded linkAddedEvent;
        public InsertLinkWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            linkAddedEvent(textBox.Text);
            Hide();
        }
    }
}
