using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TubaToolbox
{
    public static class IconGenerator
    {
        public static void GenerateIcon(string path)
        {
            if (File.Exists(path)) return;

            try
            {
                int size = 256;
                using (var bmp = new Bitmap(size, size))
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(Color.Transparent);

                    int pad = 8;

                    using (var outerPath = new GraphicsPath())
                    {
                        outerPath.AddArc(pad, pad, size - 2 * pad, size - 2 * pad, 0, 360);

                        using (var bgBrush = new LinearGradientBrush(
                            new Point(pad, pad),
                            new Point(pad, size - pad),
                            Color.FromArgb(41, 128, 185),
                            Color.FromArgb(22, 100, 160)))
                        {
                            g.FillPath(bgBrush, outerPath);
                        }

                        using (var pen = new Pen(Color.FromArgb(255, 255, 255, 255), 4))
                        {
                            g.DrawPath(pen, outerPath);
                        }
                    }

                    int innerPad = 20;
                    using (var innerPath = new GraphicsPath())
                    {
                        innerPath.AddArc(innerPad, innerPad, size - 2 * innerPad, size - 2 * innerPad, 0, 360);

                        using (var shineBrush = new LinearGradientBrush(
                            new Point(innerPad, innerPad),
                            new Point(innerPad, size / 2),
                            Color.FromArgb(60, 255, 255, 255),
                            Color.FromArgb(0, 255, 255, 255)))
                        {
                            g.FillPath(shineBrush, innerPath);
                        }
                    }

                    using (var font = new Font("Segoe UI", 120, FontStyle.Bold))
                    {
                        string text = "T";
                        var textSize = g.MeasureString(text, font);
                        float x = (size - textSize.Width) / 2;
                        float y = (size - textSize.Height) / 2 - 8;

                        using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                        {
                            g.DrawString(text, font, shadowBrush, x + 3, y + 3);
                        }

                        using (var textBrush = new LinearGradientBrush(
                            new Point(0, (int)y),
                            new Point(0, (int)(y + textSize.Height)),
                            Color.White,
                            Color.FromArgb(220, 240, 255)))
                        {
                            g.DrawString(text, font, textBrush, x, y);
                        }
                    }

                    SaveAsIcon(bmp, path);
                }
            }
            catch { }
        }

        private static void SaveAsIcon(Bitmap bmp, string path)
        {
            using (var pngMs = new MemoryStream())
            {
                bmp.Save(pngMs, ImageFormat.Png);
                byte[] pngData = pngMs.ToArray();

                using (var fs = new FileStream(path, FileMode.Create))
                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write((short)0);
                    writer.Write((short)1);
                    writer.Write((short)1);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((short)1);
                    writer.Write((short)32);
                    writer.Write(pngData.Length);
                    writer.Write(22);
                    writer.Write(pngData);
                }
            }
        }
    }
}
