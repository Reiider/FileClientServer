using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class OneFile : UserControl
    {
        public int Index { get; set; }
        public string NameFile { get { return label1.Text; } set { label1.Text = value; } }

        public delegate void load(int index);
        public event load Download;

        public OneFile()
        {
            InitializeComponent();
        }

        public void set(string s, int index)
        {
            NameFile = s;
            Index = index;
        }

        private void bDownload_Click(object sender, EventArgs e)
        {
            Download(Index);
        }
    }
}
