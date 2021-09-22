﻿using System;
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
        private static readonly int baseFreq = 96000;
        private static readonly int clockFreq = 3500000;
        private readonly float speed;
        float lineFreq;

        private static readonly int BRIGHT = 0xff, NORM = 0xd7;

        private PictureBox pb_screen;
        private Bitmap screen;

        private readonly Z80Processor cpu = new Z80Processor();
        //private readonly byte[] ram;
        //private bool cpuIrq;

        private readonly WaveIn sampler;

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

            InitIO();
            InitAudioOut();
            AudioIn();
        }
    }
}
