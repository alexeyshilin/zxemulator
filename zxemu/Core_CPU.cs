using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Konamiman.Z80dotNet;

namespace zxemu
{
    class Core_CPU
    {
    }

    public partial class Core : IMemory, IZ80InterruptSource
    {
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

        private Core(Core_CPU _)
        {
            cpu.Memory = this;

            Array.Copy(File.ReadAllBytes("48k.rom"), ram, 16384);
            cpu.RegisterInterruptSource(this);
            //sampler.StartRecording();
        }
    }
}
