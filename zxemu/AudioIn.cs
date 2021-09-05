using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using NAudio.Wave;
using Konamiman.Z80dotNet;

namespace zxemu
{
    public partial class Core : IMemory, IZ80InterruptSource
    {
        private float lastTCount = 0;
        private float lastLine = 0;
        private int lineCount = 0;
        private readonly byte[] screenData = new byte[312 * 416];
        private byte borderColor = 7;
        private readonly byte[] ram = new byte[65536];
        private bool flashInvert = false;
        private bool irq = false;
        private static readonly Rectangle screenRect = new Rectangle(0, 0, 416, 312);
        private Timer flashTimer;


        // IZ80InterruptSource
        public event EventHandler NmiInterruptPulse;

        public int Size => 65536;

        //public bool IntLineIsActive => throw new NotImplementedException();
        public bool IntLineIsActive
        {
            get
            {
                if (irq)
                {
                    irq = false;
                    return true;
                }

                return false;
            }
        }

        //public byte? ValueOnDataBus => throw new NotImplementedException();
        public byte? ValueOnDataBus => 255;
        // /IZ80InterruptSource

        public byte this[int address]
        {
            get => ram[address];
            set => ram[address] = value;
        }

        private void AudioIn()
        {
            flashTimer = new Timer
            {
                Interval = 500
            };
            flashTimer.Tick += FlashTimer_Tick;
            flashTimer.Start();

            cpu.Memory = this;

            Array.Copy(File.ReadAllBytes("48k.rom"), ram, 16384);
            cpu.RegisterInterruptSource(this);
            //sampler.StartRecording();
            Task.Run(sampler.StartRecording);
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            flashInvert = !flashInvert;
        }

        private void Sampler_DataAvailable1(object sender, WaveInEventArgs e)
        {
            //throw new NotImplementedException();

            for (int i = 0; i < e.BytesRecorded; i++)
            {
                float tCount = lastTCount;

                while (tCount < speed)
                    tCount += cpu.ExecuteNextInstruction();

                lastTCount = tCount - speed;
                lastLine += lineFreq;

                if (lastLine >= 1)
                {
                    DrawLine(lineCount++);

                    if (lineCount >= 312)
                    {
                        lineCount = 0;
                        FireInterrupt();
                    }

                    lastLine--;
                }
            }
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

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            throw new NotImplementedException();
        }

        public byte[] GetContents(int startAddress, int length)
        {
            throw new NotImplementedException();
        }
    }
}
