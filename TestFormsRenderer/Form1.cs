using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
				Console.WriteLine(displayStopwatch.ElapsedTicks + " " + tickStopwatch.ElapsedTicks);
			}

			Console.WriteLine(displayTicks);
			Console.WriteLine(tickTicks);

			IsAnimationRunning = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TickSimulation(ref Pixel[,] pixels, ref bool moved)
		{
			int xLenght = pixels.GetLength(0);
			int yLenght = pixels.GetLength(1);
			int xLenghtMunus = yLenght - 1;
			int yLanghtMunus = xLenght - 1;
			for (int y = xLenghtMunus; y >= 0; y--)
			for (int x = 0; x < xLenght; x++)
			{
				if (!pixels[x, y].Color)
					continue;
				if (pixels[x, y].Ticked)
					continue;
				if (x == 0)
					continue;
				if (x == xLenghtMunus)
					continue;
				if (y == yLanghtMunus)
					continue;
				int yPlus = y + 1;
				if (!pixels[x, yPlus].Color)
				{
					pixels[x, y].Color = false;
					pixels[x, y].Ticked = true;
					pixels[x, yPlus].Color = true;
					pixels[x, yPlus].Ticked = true;
					moved = true;
					continue;
				}

				int xMunus = x - 1;
				if (!pixels[xMunus, yPlus].Color)
				{
					pixels[x, y].Color = false;
					pixels[x, y].Ticked = true;
					pixels[xMunus, yPlus].Color = true;
					pixels[xMunus, yPlus].Ticked = true;
					moved = true;
					continue;
				}

				int xPlus = x + 1;
				if (!pixels[xPlus, yPlus].Color)
				{
					pixels[x, y].Color = false;
					pixels[x, y].Ticked = true;
					pixels[xPlus, yPlus].Color = true;
					pixels[xPlus, yPlus].Ticked = true;
					moved = true;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void FillPixelArray(int xMin, int xMax, int yMin, int yMax, ref Pixel[,] pixels)
		{
			for (int x = 0; x < pixels.GetLength(0); x++)
			for (int y = 0; y < pixels.GetLength(1); y++)
				if (x < xMax && x > xMin && y < yMax && y > yMin)
					pixels[x, y] = new Pixel(true);
				else
					pixels[x, y] = new Pixel();
		}

		private DateTime lastUpdate = DateTime.Now;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisplayPixelArray(ref Pixel[,] pixels, bool Cached = false)
		{
			for (int x = 0; x < pixels.GetLength(0); x++)
			for (int y = 0; y < pixels.GetLength(1); y++)
				if (pixels[x, y].Ticked)
				{
					bmp.SetPixel(x, y, pixels[x, y].Color ? -16777216 : -1);
					pixels[x, y].Ticked = false;
				}
				else if (Cached)
				{
					bmp.SetPixel(x, y, pixels[x, y].Color ? -16777216 : -1);
					pixels[x, y].Ticked = false;
				}

			if ((DateTime.Now - lastUpdate).Milliseconds > 30)
			{
				lastUpdate = DateTime.Now;
				pictureBox1.Image = bmp.Bitmap;
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();

		private void Form1_Load_1(object sender, EventArgs e)
		{
			AllocConsole();
		}
	}

	public class Pixel
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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