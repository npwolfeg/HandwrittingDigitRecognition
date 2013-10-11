using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LinearBinaryPattern
{
    static class BmpProcesser
    {
        public static Rectangle getBounds(Bitmap sourceBMP)
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
            Rectangle rect = new Rectangle(result[0], result[2], result[1] - result[0], result[3] - result[2]);
            return rect;
        }

        public static HashSet<Point> getConnectedPicture(Point e, Bitmap bmpSource)
        {
            Color c = bmpSource.GetPixel(e.X, e.Y);
            Bitmap bmp = new Bitmap(bmpSource.Width, bmpSource.Height);
            HashSet<Point> pts = new HashSet<Point>();
            HashSet<Point> result = new HashSet<Point>();
            pts.Add(e);
            result.Add(e);
            while (pts.Count > 0)
            {
                Point p = pts.First();
                int x = p.X;
                int y = p.Y;
                bmp.SetPixel(x, y, c);
                for (int i = -1; i < 2; i++)
                    for (int j = -1; j < 2; j++)
                    {
                        int a = x + i;
                        int b = y + j;
                        if (a > -1 && a < bmp.Width && b > -1 && b < bmp.Height && bmpSource.GetPixel(a, b) == c && bmp.GetPixel(a, b) != c)
                        {
                            pts.Add(new Point(a, b));
                            result.Add(new Point(a, b));
                        }
                    }
                pts.Remove(p);
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
                    if (sourceBMP.GetPixel(i, j).R < 255)
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

        public static Bitmap renewRChannel(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                    if (bmp.GetPixel(i, j).R < 255)
                        bmp.SetPixel(i,j,Color.FromArgb(255,0,0,0));
            return bmp;
        }

        public static Bitmap renew(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                    if (bmp.GetPixel(i, j).A > 0)
                        bmp.SetPixel(i, j, Color.Black);
            return bmp;
        }

        public static Bitmap copyPartOfBitmap(Bitmap bmp, Rectangle cloneRect)
        {
            System.Drawing.Imaging.PixelFormat format = bmp.PixelFormat;
            Bitmap result;
            if (cloneRect.Width <= 0 || cloneRect.Height <= 0)
                result = (Bitmap)bmp.Clone();
            else
                result = bmp.Clone(cloneRect, format);
            return result;
        }
     
        public static Bitmap normalizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            Bitmap cloneBitmap = copyPartOfBitmap(bmp, getBounds(bmp));
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(cloneBitmap, 0, 0, width, height);
            return result;
        }

        // need to refactor the same way as normalizeBitmap
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
            result = renew(result);
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

        static public Bitmap DrawGrid(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(bmp);
            for (int i = 0; i < bmp.Width; i++)
                using (Graphics g = Graphics.FromImage(result))
                    g.DrawLine(new Pen(Color.Orange, 2), i * 100, 0, i * 100, bmp.Height);
            for (int i = 0; i < bmp.Height; i++)
                using (Graphics g = Graphics.FromImage(result))
                    g.DrawLine(new Pen(Color.Orange, 2), 0, i * 100, bmp.Width, i * 100);
            return result;
        }
    }

}
