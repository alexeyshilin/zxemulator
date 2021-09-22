using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Konamiman.Z80dotNet;

namespace zxemu
{
    public class Core_IO : IMemory
    {
        public Action<int, byte> Set;
        public Func<int, byte> Get;

        /*
        public Core_IO(Func<int, byte> get, Action<int, byte> set)
        {
            Get = get;
            Set = set;
        }
        */

        // IMemory
        public byte this[int address]
        {
            //get => throw new NotImplementedException();
            //set => throw new NotImplementedException();

            get => Get(address);
            set => Set(address, value);
        }

        public int Size => throw new NotImplementedException();

        public byte[] GetContents(int startAddress, int length)
        {
            throw new NotImplementedException();
        }

        public void SetContents(int startAddress, byte[] contents, int startIndex = 0, int? length = null)
        {
            throw new NotImplementedException();
        }
        // /IMemory

    }

    public partial class Core
    {
        private Core_IO portSpace;
        private Dictionary<Keys, int[]> keyMap;
        private Dictionary<int, IntBuffer> keyState;
        private int speaker = 0;
        private readonly List<ulong> spkEvents = new List<ulong>();
        private bool audioInState = false;

        private Core(Core_IO portSpace) : this(new Core_CPU())
        {
            portSpace.Get = PortIn;
            portSpace.Set = PortOut;
            cpu.PortsSpace = portSpace;
        }

        private void InitIO()
        {
            //portSpace = new Core_IO(PortIn, PortOut);
            //portSpace.Get = PortIn;
            //portSpace.Set = PortOut;
            //cpu.PortsSpace = portSpace;

            keyMap = new Dictionary<Keys, int[]>();
            keyMap[Keys.ShiftKey] = new int[] { 0xfefe, 1 };
            keyMap[Keys.Z] = new int[] { 0xfefe, 2 };
            keyMap[Keys.X] = new int[] { 0xfefe, 4 };
            keyMap[Keys.C] = new int[] { 0xfefe, 8 };
            keyMap[Keys.V] = new int[] { 0xfefe, 16 };
            keyMap[Keys.A] = new int[] { 0xfdfe, 1 };
            keyMap[Keys.S] = new int[] { 0xfdfe, 2 };
            keyMap[Keys.D] = new int[] { 0xfdfe, 4 };
            keyMap[Keys.F] = new int[] { 0xfdfe, 8 };
            keyMap[Keys.G] = new int[] { 0xfdfe, 16 };
            keyMap[Keys.Q] = new int[] { 0xfbfe, 1 };
            keyMap[Keys.W] = new int[] { 0xfbfe, 2 };
            keyMap[Keys.E] = new int[] { 0xfbfe, 4 };
            keyMap[Keys.R] = new int[] { 0xfbfe, 8 };
            keyMap[Keys.T] = new int[] { 0xfbfe, 16 };
            keyMap[Keys.D1] = new int[] { 0xf7fe, 1 };
            keyMap[Keys.D2] = new int[] { 0xf7fe, 2 };
            keyMap[Keys.D3] = new int[] { 0xf7fe, 4 };
            keyMap[Keys.D4] = new int[] { 0xf7fe, 8 };
            keyMap[Keys.D5] = new int[] { 0xf7fe, 16 };
            keyMap[Keys.D0] = new int[] { 0xeffe, 1 };
            keyMap[Keys.D9] = new int[] { 0xeffe, 2 };
            keyMap[Keys.D8] = new int[] { 0xeffe, 4 };
            keyMap[Keys.D7] = new int[] { 0xeffe, 8 };
            keyMap[Keys.D6] = new int[] { 0xeffe, 16 };
            keyMap[Keys.P] = new int[] { 0xdffe, 1 };
            keyMap[Keys.O] = new int[] { 0xdffe, 2 };
            keyMap[Keys.I] = new int[] { 0xdffe, 4 };
            keyMap[Keys.U] = new int[] { 0xdffe, 8 };
            keyMap[Keys.Y] = new int[] { 0xdffe, 16 };
            keyMap[Keys.Enter] = new int[] { 0xbffe, 1 };
            keyMap[Keys.L] = new int[] { 0xbffe, 2 };
            keyMap[Keys.K] = new int[] { 0xbffe, 4 };
            keyMap[Keys.J] = new int[] { 0xbffe, 8 };
            keyMap[Keys.H] = new int[] { 0xbffe, 16 };
            keyMap[Keys.Space] = new int[] { 0x7ffe, 1 };
            keyMap[Keys.ControlKey] = new int[] { 0x7ffe, 2 };
            keyMap[Keys.M] = new int[] { 0x7ffe, 4 };
            keyMap[Keys.N] = new int[] { 0x7ffe, 8 };
            keyMap[Keys.B] = new int[] { 0x7ffe, 16 };

            keyState = new Dictionary<int, IntBuffer>();
            keyState[0xfefe] = new IntBuffer(0x1f); // 0xbf 0x1f
            keyState[0xfdfe] = new IntBuffer(0x1f);
            keyState[0xfbfe] = new IntBuffer(0x1f);
            keyState[0xf7fe] = new IntBuffer(0x1f);
            keyState[0xeffe] = new IntBuffer(0x1f);
            keyState[0xdffe] = new IntBuffer(0x1f);
            keyState[0xbffe] = new IntBuffer(0x1f);
            keyState[0x7ffe] = new IntBuffer(0x1f); //???
            //keyState[0x00fe] = new IntBuffer(0x1f); //???
        }

        private byte PortIn(int address)
        {
            //Console.WriteLine(address.ToString());
            
            if (keyState.ContainsKey(address))
            {
                IntBuffer ks = keyState[address];
                lock (ks)
                {
                    byte b =  (byte)ks.Value;
                    //Console.WriteLine(b.ToString());
                    return b;
                }
            }else if(address == 0xfe)
            {
                return (byte)(audioInState ? 0xff : 0xbf);
                //byte port = (byte)(audioInState ? 0xff : 0xbf);
                //return port;
            }
            
            return 255;
        }

        private void PortOut(int address, byte value)
        {
            //Console.WriteLine("[PortOut]");

            if (address == 254)
            {
                borderColor = (byte)(value & 7);

                int lastSpeaker = speaker;
                speaker = (value & 0x18) >> 3;
                if(speaker != lastSpeaker)
                {
                    ulong sstate = (cpu.TStatesElapsedSinceStart << 2) | (uint)speaker;
                    lock (spkEvents)
                    {
                        spkEvents.Add(sstate);
                    }
                }
            }
        }

        public void KeyPress(Keys key, bool down)
        {
            //Console.WriteLine("Key:" + key + "\t" + down);

            if (keyMap.ContainsKey(key))
            {
                int port = keyMap[key][0];
                int bit = keyMap[key][1];
                
                IntBuffer ks = keyState[port];

                lock (ks)
                {
                    
                    int lv = ks.LastValue;

                    if (down)
                    {
                        lv &= ~bit & 0xff;
                    }
                    else
                    {
                        lv |= bit & 0xff;
                    }

                    ks.Value = lv;
                }



            }
        }
    }
}
