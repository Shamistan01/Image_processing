using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ObraborkaIzobrajeniy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _random = new Random();

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sw = Stopwatch.StartNew();
                menuStrip1.Enabled = trackBar1.Enabled = false;
                pictureBox1.Image = null;
                _bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run(() => { RunProcessing(bitmap); });
                menuStrip1.Enabled = trackBar1.Enabled = true;
                sw.Stop();
                Text = sw.Elapsed.ToString();
            }
        }

        private void RunProcessing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelIsInStep = (bitmap.Height * bitmap.Width)/100;
            var currentPixelSet = new List<Pixel>(pixels.Count - pixelIsInStep);
            for(int i = 1; i < trackBar1.Maximum; i++)
            {
                for(int j = 0; j < pixelIsInStep; j++)
                {
                    var index = _random.Next(pixels.Count);
                    currentPixelSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }
                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach (var pixel in currentPixelSet)
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                _bitmaps.Add(currentBitmap);

                this.Invoke(new Action(() =>
                {
                    Text = $"{i} %";
                }));
                
            }
            _bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width*bitmap.Height);

            for(int y = 0; y < bitmap.Height; y++)
            {
                for(int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color=bitmap.GetPixel(x,y),
                        Point=new Point() { X = x, Y = y }
                    });
                }
            }
            return pixels;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count==0)
                return;

            pictureBox1.Image = _bitmaps[trackBar1.Value - 1];
        }
    }
}
