using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DarkTCP
{
    public partial class ImgPreview : Form
    {
        public MemoryStream Stream;

        public ImgPreview()
        {
            InitializeComponent();
        }

        private void ImgPreview_Load(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Image = Image.FromStream(Stream);
            }
            catch { throw; }
        }
    }
}
