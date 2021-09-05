using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zxemu
{
    public class IntBuffer
    {
        private int previousValue;
        private List<int> buffer;

        public int Value
        {
            get
            {
                if (buffer.Count > 0)
                {
                    int i = buffer.ElementAt(0);
                    buffer.RemoveAt(0);
                    return i;
                }
                
                //return 0x1f;
                return previousValue;
            }
            set
            {
                buffer.Add(value);
                //previousValue = value;
            }
        }

        public int LastValue => previousValue;

        public IntBuffer(int initialValue = 0)
        {
            previousValue = initialValue;
            buffer = new List<int>();
        }


    }
}
