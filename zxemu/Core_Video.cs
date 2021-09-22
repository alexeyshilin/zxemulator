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
using System.Runtime.InteropServices;

using Konamiman.Z80dotNet;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace zxemu
{
    class Core_Video
    {
        public PictureBox screen;
        public Core_Video(PictureBox pBox) => screen = pBox;
    }

    public partial class Core
    {
        //
        private float lastTCount = 0;
        private float lastLine = 0;
        private int lineCount = 0;
        private readonly byte[] screenData = new byte[312 * 416];
        private byte borderColor = 7;
        private bool flashInvert = false;
        private static readonly Rectangle screenRect = new Rectangle(0, 0, 416, 312);
        private Timer flashTimer;
        //


        private static readonly int BRIGHT = 0xff, NORM = 0xd7;

        private PictureBox pb_screen;
        private Bitmap screen;


        //public Core(PictureBox pb)
        private Core(Core_Video coreVideo) : this(new Core_AudioIn())
        {
            /*
            sampler = new WaveIn()
            {
                WaveFormat = new WaveFormat(baseFreq, 8, 1)
            };
            */

            //create enumerator
            var enumerator1 = new MMDeviceEnumerator();
            //cycle through all audio devices
            for (int i = 0; i < WaveIn.DeviceCount; i++)
                Console.WriteLine("{0} - {1}", i, enumerator1.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)[i]);
            //clean up
            enumerator1.Dispose();

            //create enumerator
            var enumerator2 = new MMDeviceEnumerator();
            //cyckle trough all audio devices
            for (int i = 0; i < WaveOut.DeviceCount; i++)
                Console.WriteLine("{0} - {1}", i, enumerator2.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)[i]);
            //clean up
            enumerator2.Dispose();

            sampler = new WaveIn()
            {
                WaveFormat = new WaveFormat(baseFreq, 8, 1),
                DeviceNumber = 0
            };

            sampler.DataAvailable += Sampler_DataAvailable1;

            speed = (float)clockFreq / (float)baseFreq;
            lineFreq = (312f * 50f) / baseFreq;
            pb_screen = coreVideo.screen;

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

            //
            flashTimer = new Timer
            {
                Interval = 500
            };
            flashTimer.Tick += FlashTimer_Tick;
            flashTimer.Start();
            //

            InitIO();
            InitAudioOut();
            AudioIn();
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            flashInvert = !flashInvert;
        }

        private void DrawLine(int line)
        {
            //throw new NotImplementedException();

            if (line < 8) return;

            int lineStart = line * 416;

            if (line < 64 || line >= 256)
            {
                Fill(screenData, borderColor, lineStart, 416);
                return;
            }

            Fill(screenData, borderColor, lineStart, 80); // left border
            Fill(screenData, borderColor, lineStart + 336, 80); // right border

            lineStart += 80;
            line -= 64;

            int charY = 0x5800 + ((line >> 3) << 5);
            int lineAddr = ((line & 0x07) << 8) | ((line & 0x38) << 2) | ((line & 0xC0) << 5) | 0x4000;
            for (int charX = 0; charX < 32; charX++)
            {
                byte att = ram[charX + charY];
                int ink = att & 0x07;
                //ink = 0x01; // red
                int paper = (att & 0x38) >> 3;
                if ((att & 0x40) != 0) { ink += 8; paper += 8; }
                bool flash = (att & 0x80) != 0;
                bool doFlash = flash && flashInvert;
                byte byt = ram[lineAddr++];
                for (int bit = 128; bit > 0; bit >>= 1)
                {
                    if (doFlash)
                        screenData[lineStart++] = (byte)((byt & bit) != 0 ? paper : ink);
                    else
                        screenData[lineStart++] = (byte)((byt & bit) != 0 ? ink : paper);
                }
            }
        }

        private void FireInterrupt()
        {
            //throw new NotImplementedException();

            irq = true;

            byte[] clone = (byte[])screenData.Clone();
            if (!pb_screen.IsDisposed)
            {
                pb_screen.Invoke((System.Windows.Forms.MethodInvoker)delegate () {
                    BitmapData bmd = screen.LockBits(
                        screenRect,
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format8bppIndexed
                        );
                    Marshal.Copy(clone, 0, bmd.Scan0, clone.Length);
                    screen.UnlockBits(bmd);
                    pb_screen.Refresh();
                });
            }
        }

        private static void Fill(byte[] array, byte with, int start, int len)
        {
            int end = start + len;

            while (start < end)
            {
                array[start++] = with;
            }
        }

    }
}
