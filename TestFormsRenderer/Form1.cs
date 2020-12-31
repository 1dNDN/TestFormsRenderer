using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TestFormsRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool IsAnimationRunning;
        public DirectBitmap bmp;
        private object lockObject = new object();

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsAnimationRunning) new Thread(Animate).Start();
        }

        public void Animate()
        {
            IsAnimationRunning = true;
            Pixel[,] pixels = new Pixel[256, 256];
            FillPixelArray(78, 178, 56, 156, ref pixels);
            bmp = new DirectBitmap(pixels.GetLength(0), pixels.GetLength(1));
            DisplayPixelArray(ref pixels, true);
            Thread.Sleep(500);
            bool moved = true;

            long displayTicks = 0;
            long tickTicks = 0;
            
            while (moved)
            {
                moved = false;
                Stopwatch tickStopwatch = new Stopwatch();
                Stopwatch displayStopwatch = new Stopwatch();
                tickStopwatch.Start();
                TickSimulation(ref pixels, ref moved);
                tickStopwatch.Stop();
                displayStopwatch.Start();
                DisplayPixelArray(ref pixels);
                displayStopwatch.Stop();
                displayTicks += displayStopwatch.ElapsedTicks;
                tickTicks += tickStopwatch.ElapsedTicks;
            }
            
            Console.WriteLine(displayTicks);
            Console.WriteLine(tickTicks);

            IsAnimationRunning = false;
        }

        private static void TickSimulation(ref Pixel[,] pixels, ref bool moved)
        {
            int xLenght = pixels.GetLength(0);
            int yLenght = pixels.GetLength(1);
            
            for (int y = yLenght - 1; y >= 0; y--)
            for (int x = 0; x < xLenght; x++)
            {
                if (!pixels[x, y].Color)
                    continue;
                if (pixels[x, y].Ticked)
                    continue;
                if (x == 0)
                    continue;
                if (x == xLenght - 1)
                    continue;
                if (y == yLenght - 1)
                    continue;
                if (!pixels[x, y + 1].Color)
                {
                    pixels[x, y].Color = false;
                    pixels[x, y].Ticked = true;
                    pixels[x, y + 1].Color = true;
                    pixels[x, y + 1].Ticked = true;
                    moved = true;
                    continue;
                }

                if (!pixels[x - 1, y + 1].Color)
                {
                    pixels[x, y].Color = false;
                    pixels[x, y].Ticked = true;
                    pixels[x - 1, y + 1].Color = true;
                    pixels[x - 1, y + 1].Ticked = true;
                    moved = true;
                    continue;
                }

                if (!pixels[x + 1, y + 1].Color)
                {
                    pixels[x, y].Color = false;
                    pixels[x, y].Ticked = true;
                    pixels[x + 1, y + 1].Color = true;
                    pixels[x + 1, y + 1].Ticked = true;
                    moved = true;
                }
            }
        }

        private static void FillPixelArray(int xMin, int xMax, int yMin, int yMax, ref Pixel[,] pixels)
        {
            for (int x = 0; x < pixels.GetLength(0); x++)
            for (int y = 0; y < pixels.GetLength(1); y++)
                if (x < xMax && x > xMin && y < yMax && y > yMin)
                    pixels[x, y] = new Pixel(true);
                else
                    pixels[x, y] = new Pixel();
        }

        private void DisplayPixelArray(ref Pixel[,] pixels, bool Cached = false)
        {
            for (int x = 0; x < pixels.GetLength(0); x++)
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                if (pixels[x, y].Ticked)
                {
                    bmp.SetPixel(x, y, pixels[x, y].Color ? Color.Black : Color.White);
                    pixels[x, y].Ticked = false;
                }
                else if (Cached)
                {
                    bmp.SetPixel(x, y, pixels[x, y].Color ? Color.Black : Color.White);
                    pixels[x, y].Ticked = false;
                }
            }
            
            pictureBox1.Image = bmp.Bitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }

    public class Pixel
    {
        public Pixel(bool Color = false, bool Ticked = false, bool Static = false)
        {
            this.Color = Color;
            this.Ticked = Ticked;
            this.Static = Static;
        }

        public bool Color;

        public bool Ticked;

        public bool Static;
    }
}