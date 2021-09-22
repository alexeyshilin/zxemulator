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
using NAudio.CoreAudioApi;
using Konamiman.Z80dotNet;

namespace zxemu
{
    class Core_AudioIn { }

    public partial class Core
    {
        private readonly WaveIn sampler;


        private Core(Core_AudioIn coreVideo) : this(new Core_AudioOut())
        {
            sampler = new WaveIn()
            {
                WaveFormat = new WaveFormat(baseFreq, 8, 1)
            };
            sampler.DataAvailable += Sampler_DataAvailable1;

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

                CPU_Execute_Block();
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
