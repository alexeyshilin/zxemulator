﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace zxemu
{
    class Core_AudioOut { }

    public partial class Core : WaveStream
    {
        private readonly WaveFormat waveFormat = new WaveFormat(baseFreq, 8, 1);
        public override WaveFormat WaveFormat => waveFormat;

        private long position = 0;
        private byte spkValue = 128;
        private byte nextSpkValue = 0;
        private ulong sampleCount = 0;
        private bool waitingForChange = false;
        private ulong sampleChange = 0;
        //private ulong soundLatency = 18500;
        private ulong soundLatency = (ulong) baseFreq/5;

        private readonly List<ulong> spkEvents = new List<ulong>();

        //public override long Length => throw new NotImplementedException();
        public override long Length => long.MaxValue;

        public override long Position { get => position; set => throw new NotImplementedException(); }

        private Core(Core_AudioOut coreVideo) : this(new Core_IO())
        {
            //WaveOutEvent waveOut = new WaveOutEvent();
            //waveOut.DesiredLatency = 75;

            WaveOutEvent waveOut = new WaveOutEvent
            {
                DesiredLatency = 75
            };

            waveOut.Init(this);
            Task.Run(waveOut.Play);
        }

        private void InitAudioOut()
        {

        }

        private void SpeakerChanger()
        {
            sampleCount++;

            if(!waitingForChange)
            {
                ulong next;
                lock(spkEvents)
                {
                    if (spkEvents.Count == 0)
                        return;

                    next = spkEvents.ElementAt(0);
                    spkEvents.RemoveAt(0);
                }

                nextSpkValue = (byte)((next & 3) * 10);
                next >>= 2;
                //sampleChange = (ulong)((double)next / (double)speed);
                sampleChange = (ulong)((double)next / (double)cpuSpeed) + soundLatency;
                waitingForChange = true;
            }
            else
            {
                if(sampleCount >= sampleChange)
                {
                    spkValue = nextSpkValue;
                    waitingForChange = false;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //throw new NotImplementedException();

            for(int i = 0; i < count; i++)
            {
                SpeakerChanger();
                buffer[i] = spkValue;
            }

            return count;
        }
    }
}
