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
    class Core_AudioIn { }

    public partial class Core
    {



        private Core(Core_AudioIn coreVideo) : this(new Core_AudioOut())
        {

        }



        private void AudioIn()
        {



            //Task.Run(sampler.StartRecording);
        }



        private void Sampler_DataAvailable1(object sender, WaveInEventArgs e)
        {
            //throw new NotImplementedException();

            for (int i = 0; i < e.BytesRecorded; i++)
            {
                audioInState = e.Buffer[i] > 150;

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
