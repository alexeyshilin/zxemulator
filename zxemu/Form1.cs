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

            PB_Screen.Controls.Add(cb);
            PB_Screen.Click += Pb_Click;

            cb.KeyDown += Cb_KeyDown;
            cb.KeyUp += Cb_KeyUp;

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
            Console.WriteLine("KeyUp keyStates.len={0}", keyStates.Count);
        }

        private void Cb_KeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();

            if (!keyStates.Contains(e.KeyCode))
            {
                Console.WriteLine("KeyDown keyStates.len={0}", keyStates.Count);
                keyStates.Add(e.KeyCode);
                core.KeyPress(e.KeyCode, true);
            }

            e.Handled = true;
        }

        private void Cb_KeyUp_(object sender, KeyEventArgs e)
        {
            keyStates.Remove(e.KeyCode);
            core?.KeyPress(e.KeyCode, false);
            e.Handled = true;
        }

        private void Cb_KeyDown_(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    SpecialKey(Keys.ShiftKey, Keys.D0);
                    break;
                case Keys.Menu:
                    SpecialKey(Keys.ControlKey, Keys.ShiftKey);
                    break;
                case Keys.Up:
                    SpecialKey(null, Keys.D7);
                    break;
                case Keys.Down:
                    SpecialKey(null, Keys.D6);
                    break;
                case Keys.Left:
                    SpecialKey(null, Keys.D5);
                    break;
                case Keys.Right:
                    SpecialKey(null, Keys.D8);
                    break;
                case Keys.CapsLock:
                    SpecialKey(Keys.ShiftKey, Keys.D2);
                    break;
                default:
                    if (!keyStates.Contains(e.KeyCode))
                    {
                        keyStates.Add(e.KeyCode);
                        core?.KeyPress(e.KeyCode, true);
                    }
                    break;
            }
            e.Handled = true;
        }

        private void SpecialKey(Keys? qualifier, Keys key)
        {
            KeyEventArgs qualArgs =
                qualifier != null && !keyStates.Contains((Keys)qualifier) ?
                new KeyEventArgs((Keys)qualifier) : null;
            KeyEventArgs keyArgs = new KeyEventArgs(key);
            if (qualArgs != null)
                Cb_KeyDown(this, qualArgs);
            Cb_KeyDown(this, keyArgs);
            Cb_KeyUp(this, keyArgs);
            if (qualArgs != null)
                Cb_KeyUp(this, qualArgs);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Core core = new Core(pb);
            //core.Run();

            core = new Core(PB_Screen);
        }
    }
}
