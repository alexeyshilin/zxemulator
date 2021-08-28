using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zxemu
{
    public partial class Form1 : Form
    {
        Core core;

        public Form1()
        {
            InitializeComponent();

            CheckBox cb = new CheckBox();
            cb.Size = Size.Empty;
            cb.KeyDown += Cb_KeyDown;
            cb.KeyUp += Cb_KeyUp;

            pb.Controls.Add(cb);
            pb.Click += Pb_Click;
        }

        private void Cb_KeyUp(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Cb_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            pb.Controls[0].Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Core core = new Core(pb);
            //core.Run();

            core = new Core(pb);
        }
    }
}
