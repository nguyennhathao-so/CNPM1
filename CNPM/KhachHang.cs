using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhachHang : UserControl
    {
        public KhachHang()
        {
            InitializeComponent();
        }
        private Bitmap SetImageOpacity(Image image, float opacity)
        {
            // Create a bitmap the same size as the original image
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Create a color matrix with the specified opacity
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = opacity;

                // Create image attributes
                ImageAttributes imgAttributes = new ImageAttributes();
                imgAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                // Draw the image onto the bitmap with the specified opacity
                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttributes);
            }

            return bmp;
        }
        private void KhachHang_Load(object sender, EventArgs e)
        {
            Bitmap originalImage = new Bitmap(TrongPicture.Image);
            Bitmap transparentImage = SetImageOpacity(originalImage, 0.3f); // 30% opacity
            TrongPicture.Image = transparentImage;
        }
    }
}
