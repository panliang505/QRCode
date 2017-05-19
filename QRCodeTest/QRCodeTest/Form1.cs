using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CustomQRCode;


namespace QRCodeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_qrcode.Text))
            {
                return;
            }
            int size = int.Parse(textBox1.Text);
            int w = (int)Mm2Pixel(size);
            int h = (int)Mm2Pixel(size);
            pictureBox1.Height = h;
            pictureBox1.Width = w;

            QRCode2 qrcode = new QRCode2();
            System.Drawing.Size s = new Size(w, h);
            qrcode.CreateQRCode(txt_qrcode.Text, QRCoder.QRCodeGenerator.ECCLevel.Q, s);

            pictureBox1.Image = qrcode.Imge;

            return;
        }


        private const double MillimererTopixel = 25.4;
        private const int Dpix = 96;

        public static double Mm2Pixel(double length)
        {
            return Dpix * length / MillimererTopixel;
        }
    }
}
