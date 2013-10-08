using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LinearBinaryPattern
{
    class BmpProcesser
    {
        private static int[] getBounds(Bitmap sourceBMP)
        {
            int[] result = new int[4];
            result[0] = sourceBMP.Width;
            result[1] = 0;
            result[2] = sourceBMP.Height;
            result[3] = 0;
            for (int i = 0; i < sourceBMP.Width; i++)
                for (int j = 0; j < sourceBMP.Height; j++)
                {
                    if (sourceBMP.GetPixel(i, j).A > 0)
                    {
                        if (i < result[0])
                            result[0] = i;
                        if (i > result[1])
                            result[1] = i;
                        if (j < result[2])
                            result[2] = j;
                        if (j > result[3])
                            result[3] = j;
                    }
                }
            return result;
        }

        private static int[] getBoundsRChannel(Bitmap sourceBMP)
        {
            int[] result = new int[4];
            result[0] = sourceBMP.Width;
            result[1] = 0;
            result[2] = sourceBMP.Height;
            result[3] = 0;
            for (int i = 0; i < sourceBMP.Width; i++)
                for (int j = 0; j < sourceBMP.Height; j++)
                {
                    if (sourceBMP.GetPixel(i, j).R == 0)
                    {
                        if (i < result[0])
                            result[0] = i;
                        if (i > result[1])
                            result[1] = i;
                        if (j < result[2])
                            result[2] = j;
                        if (j > result[3])
                            result[3] = j;
                    }
                }
            return result;
        }
     
        public static Bitmap normalizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            int[] bounds = getBounds(sourceBMP);
            Rectangle cloneRect = new Rectangle(bounds[0], bounds[2], bounds[1] - bounds[0], bounds[3] - bounds[2]);
            System.Drawing.Imaging.PixelFormat format = sourceBMP.PixelFormat;
            Bitmap cloneBitmap;
            if (cloneRect.Width <= 0 || cloneRect.Height <= 0)
                cloneBitmap = (Bitmap)sourceBMP.Clone();
            else
                cloneBitmap = sourceBMP.Clone(cloneRect, format);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(cloneBitmap, 0, 0, width, height);
            return result;
        }

        public static Bitmap normalizeBitmapRChannel(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            int[] bounds = getBoundsRChannel(sourceBMP);
            Rectangle cloneRect = new Rectangle(bounds[0], bounds[2], bounds[1] - bounds[0], bounds[3] - bounds[2]);
            System.Drawing.Imaging.PixelFormat format = sourceBMP.PixelFormat;
            Bitmap cloneBitmap;
            if (cloneRect.Width <= 0 || cloneRect.Height <= 0)
                cloneBitmap = (Bitmap)sourceBMP.Clone();
            else
                cloneBitmap = sourceBMP.Clone(cloneRect, format);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(cloneBitmap, 0, 0, width, height);
            return result;
        }

        public static Bitmap GrayScale(Bitmap bmp)
        {
            return GrayScale(bmp, 120);
        }

        public static Bitmap GrayScale(Bitmap Bmp, int partition)
        {
            int rgb;
            Color c;
            Bitmap tempBmp = new Bitmap(Bmp);

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)((.299 * c.R + .587 * c.G + .114 * c.B));
                    if (rgb > partition)
                        rgb = 255;
                    else
                        rgb = 0;
                    tempBmp.SetPixel(x, y, Color.FromArgb(255 - rgb, rgb, rgb, rgb));
                }
            return tempBmp;
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        public static Bitmap preprocessBitmap(Bitmap bmp)
        {
            //Bitmap result = GrayScale(bmp,100);
            Bitmap result = ResizeBitmap(bmp, 100, 100);
            result = normalizeBitmap(result, 100, 100);
            return result;
        }
        public static bool lineIsEmpty(Bitmap bmp, int x)
        {
            bool result = true;
            if (x < bmp.Width && x > 0)
            {
                for (int i = 0; i < bmp.Height; i++)
                    if (bmp.GetPixel(x, i).A > 0)
                        result = false;
            }
            return result;
        }

        static public Bitmap FromAlphaToRGB(Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap.Width,bitmap.Height);
            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int temp = 255 - bitmap.GetPixel(i, j).A;
                    result.SetPixel(i, j, Color.FromArgb(255, temp, temp, temp));
                }
            return result;
        }
    }

}
