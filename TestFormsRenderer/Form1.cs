using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestFormsRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool IsAnimationRunning = false;
        
        private void button1_Click(object sender, EventArgs e)
        {
            if(!IsAnimationRunning) new Thread(Animate).Start();
        }

        public void Animate()
        {
            IsAnimationRunning = true;

            Pixel[,] pixels = new Pixel[256, 256];
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    
                }
            }
            
            IsAnimationRunning = false;
        }
    }

    public class Pixel
    {
        public bool Color = false;

        public bool Ticked = false;

        public bool Static = false;
    }
}
