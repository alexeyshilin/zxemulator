using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using Konamiman.Z80dotNet;
//using NAudio.Wave;

namespace zxemu
{
    public partial class Core
    {
        private static readonly int baseFreq = 96000;
        private static readonly int clockFreq = 3500000;
        private readonly float speed;
        float lineFreq;

        private static readonly int BRIGHT = 0xff, NORM=0xd7;

        private PictureBox pb_screen;
        private Bitmap screen;

        private readonly Z80Processor cpu = new Z80Processor();
        //private readonly byte[] ram;
        //private bool cpuIrq;

        //private readonly WaveIn sampler;

        public Core(PictureBox pb)
        {
            /*
            sampler = new WaveIn()
            {
                WaveFormat = new WaveFormat(baseFreq, 8, 1)
            };
            sampler.DataAvailable += Sampler_DataAvailable1;
            */

            speed = (float)clockFreq / (float)baseFreq;
            lineFreq = (312f * 50f) / baseFreq;
            pb_screen = pb;

            screen = new Bitmap(416, 312, PixelFormat.Format8bppIndexed);
            ColorPalette pal = screen.Palette;
            pal.Entries[0] = Color.FromArgb(0, 0, 0);
            pal.Entries[1] = Color.FromArgb(0, 0, NORM);
            pal.Entries[2] = Color.FromArgb(NORM, 0, 0);
            pal.Entries[3] = Color.FromArgb(NORM, 0, NORM);
            pal.Entries[4] = Color.FromArgb(0, NORM, 0);
            pal.Entries[5] = Color.FromArgb(0, NORM, NORM);
            pal.Entries[6] = Color.FromArgb(NORM, NORM, 0);
            pal.Entries[7] = Color.FromArgb(NORM, NORM, NORM);
            pal.Entries[8] = Color.FromArgb(0, 0, 0);
            pal.Entries[9] = Color.FromArgb(0, 0, BRIGHT);
            pal.Entries[10] = Color.FromArgb(BRIGHT, 0, 0);
            pal.Entries[11] = Color.FromArgb(BRIGHT, 0, BRIGHT);
            pal.Entries[12] = Color.FromArgb(0, BRIGHT, 0);
            pal.Entries[13] = Color.FromArgb(0, BRIGHT, BRIGHT);
            pal.Entries[14] = Color.FromArgb(BRIGHT, BRIGHT, 0);
            pal.Entries[15] = Color.FromArgb(BRIGHT, BRIGHT, BRIGHT);
            screen.Palette = pal;
            pb_screen.BackgroundImage = screen;

            AudioIn();
        }

    }
}
