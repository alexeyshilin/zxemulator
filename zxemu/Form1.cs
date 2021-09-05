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
        private readonly HashSet<Keys> keyStates;
        public Form1()
        {
            InitializeComponent();

            CheckBox cb = new CheckBox();
            cb.Size = Size.Empty;
            cb.KeyDown += Cb_KeyDown;
            cb.KeyUp += Cb_KeyUp;

            PB_Screen.Controls.Add(cb);
            PB_Screen.Click += Pb_Click;

            keyStates = new HashSet<Keys>();
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            PB_Screen.Controls[0].Focus();
        }

        private void Cb_KeyUp(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();

            keyStates.Remove(e.KeyCode);
            core.KeyPress(e.KeyCode, false);
            e.Handled = true;
        }

        private void Cb_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();

            if (!keyStates.Contains(e.KeyCode))
            {
                keyStates.Add(e.KeyCode);
                core.KeyPress(e.KeyCode, true);
            }

            e.Handled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Core core = new Core(pb);
            //core.Run();

            core = new Core(PB_Screen);
        }
    }
}
