using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging.Filters;

namespace bmpConvertor
{
    class Program
    {
        static void Main(string[] args)
        {

            DirectoryInfo dir = new DirectoryInfo(@"E:\사진사진\jesus1");

            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                

                Bitmap original = new Bitmap(file.FullName);

                //MakeGrayscale3(original).Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\jesus\" + file.Name, ImageFormat.Bmp);

                var bmp8bpp = Grayscale.CommonAlgorithms.BT709.Apply(original);

                bmp8bpp.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\jesus\" + file.Name, ImageFormat.Bmp);

                //Convert(original).Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\jesus\" + file.Name, ImageFormat.Bmp);
            }
            

        }

        public static Image Convert(Bitmap oldbmp)
        {
            using (var ms = new MemoryStream())
            {
                oldbmp.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
            });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}
