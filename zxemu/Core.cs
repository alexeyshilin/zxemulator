using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using Konamiman.Z80dotNet;
using NAudio.Wave;

namespace zxemu
{
    public partial class Core
    {
        private static readonly int baseFreq = 96000;

        public Core(PictureBox screen) : this(new Core_Video(screen))
        {
            //sampler.StartRecording();
            Task.Run(sampler.StartRecording);
        }
    }
}
