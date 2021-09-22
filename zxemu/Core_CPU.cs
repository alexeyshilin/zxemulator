using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Konamiman.Z80dotNet;

/*
namespace zxemu
{
    class Core_CPU
    {
    }

    public partial class Core : IMemory, IZ80InterruptSource
    {
        private readonly Z80Processor cpu = new Z80Processor();
        //private readonly byte[] ram;
        //private bool cpuIrq;
        private readonly byte[] ram = new byte[65536];
        private bool irq = false;

        private static readonly int baseFreq = 96000;
        private static readonly int clockFreq = 3500000;
        private readonly float speed;
        float lineFreq;

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
            speed = (float)clockFreq / (float)baseFreq;

            Array.Copy(File.ReadAllBytes("48k.rom"), ram, 16384);
            cpu.Memory = this;
            cpu.RegisterInterruptSource(this);
            //sampler.StartRecording();
        }

        private void CPU_Execute_Block()
        {
            float tCount = lastTCount;

            while (tCount < speed)
                tCount += cpu.ExecuteNextInstruction();

            lastTCount = tCount - speed;

            // video
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
            // /video
        }
    }
}
*/

namespace zxemu
{
    class Core_CPU { }

    public partial class Core : IMemory, IZ80InterruptSource
    {
        private static readonly int cpuClockFreq = 3500000;


        private readonly Z80Processor cpu;
        private float cpuLastTCount;
        private readonly float cpuSpeed;
        private readonly byte[] cpuRam;
        private bool cpuIrq;

        public int Size => cpuRam.Length;

        public byte this[int address]
        {
            get => cpuRam[address];
            set => cpuRam[address] = value;
        }

#pragma warning disable CS0067
        public event EventHandler NmiInterruptPulse;
#pragma warning restore CS0067

        public bool IntLineIsActive
        {
            get
            {
                if (cpuIrq)
                {
                    cpuIrq = false;
                    return true;
                }
                return false;
            }
        }

        public byte? ValueOnDataBus => 255;

        private Core(Core_CPU _)
        {
            cpu = new Z80Processor();
            cpuIrq = false;
            cpuLastTCount = 0;
            cpuSpeed = (float)cpuClockFreq / (float)baseFreq;
            cpuRam = new byte[65536];
            Array.Copy(File.ReadAllBytes("48k.rom"), cpuRam, 16384);
            cpu.SetMemoryAccessMode(0, 16384, MemoryAccessMode.ReadOnly);
            cpu.Memory = this;
            cpu.RegisterInterruptSource(this);
        }

        private void CPU_Execute_Block()
        {
            float tCount = cpuLastTCount;
            while (tCount < cpuSpeed)
                tCount += cpu.ExecuteNextInstruction();
            cpuLastTCount = tCount - cpuSpeed;
            Video_Scan();
        }

        private void CPU_Interrupt()
        {
            cpuIrq = true;
            Video_Display();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            Array.Copy(contents, 0, cpuRam, startIndex, length ?? contents.Length);
        }

        public byte[] GetContents(int startAddress, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(cpuRam, startAddress, bytes, 0, length);
            return bytes;
        }

    }
}
