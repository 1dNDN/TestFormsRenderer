using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TestFormsRenderer
{

    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        public int[] Bits { get; }
        public bool Disposed { get; private set; }
        public int Height { get; }
        public int Width { get; }

        protected GCHandle BitsHandle { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(int x, int y, Color colour)
        {
            int index = x + y * Width;
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(int x, int y, int colour)
        {
            Bits[x + y * Width] = colour;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color GetPixel(int x, int y)
        {
            int index = x + y * Width;
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}