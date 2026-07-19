using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class TextDrawing
    {
        #region Constants

        #endregion

        #region Delegates

        #endregion

        #region Events

        #endregion

        #region Enums

        public enum DrawMethod
        {
            /// <summary>
            /// Create the smallest bitmap needed to draw the text without word wrap
            /// </summary>
            Smallest,
            /// <summary>
            /// Draw text with the biggest font possible while not exceeding rectangle dimensions, without word wrap
            /// </summary>
            LargestNoWrap,
            /// <summary>
            /// Draw text in rectangle while performing word wrap. font size is a constant input. drawing may exceed bitmap rectangle.
            /// </summary>
            NoWrap,
            /// <summary>
            /// Draw text with the biggest font possible while not exceeding rectangle dimensions, with word wrap
            /// </summary>
            LargestWrap
        }

        #endregion

        #region DLL Imports

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors and Destructor

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        private static void SetGraphicsQuality(Graphics g, PixelOffsetMode Quality)
        {
            // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing).
            // One exception is that path gradient brushes do not obey the smoothing mode.
            // Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // The interpolation mode determines how intermediate values between two endpoints are calculated.
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
            g.PixelOffsetMode = Quality;

            // This one is important
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        #endregion

        #region Public Methods

        public static Size MeasureDrawnTextBitmapSize(string Text, Font Font)
        {
            Bitmap bmp = new Bitmap(1, 1);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(Text, Font);
                return new Size((int)(Math.Ceiling(size.Width)), (int)(Math.Ceiling(size.Height)));
            }

        }

        public static int MaxFontSize(string Text, Font Font, RectangleF RectangleDimensions, bool AllowWrap, int MinimumFontSize = 6, int MaximumFontSize = 1000)
        {
            Font newFont;
            Rectangle rect = Rectangle.Ceiling(RectangleDimensions);

            for (int newFontSize = MinimumFontSize; ; newFontSize++)
            {
                newFont = new Font(Font.FontFamily, newFontSize, Font.Style);

                List<string> ls = WrapText(Text, newFont, rect.Width);

                StringBuilder sb = new StringBuilder();

                if (AllowWrap)
                {
                    for (int i = 0; i < ls.Count; ++i)
                    {
                        sb.Append(ls[i] + Environment.NewLine);
                    }
                }
                else
                {
                    sb.Append(Text);
                }

                Size size = MeasureDrawnTextBitmapSize(sb.ToString(), newFont);
                if (size.Width > RectangleDimensions.Width || size.Height > RectangleDimensions.Height)
                {
                    return (newFontSize - 1);
                }

                if (newFontSize >= MaximumFontSize)
                {
                    return (newFontSize - 1);
                }

            }

        }

        public static List<string> WrapText(string Text, Font Font, int LineWidthPx)
        {
            string[] originalLines = Text.Split(new string[] { " " }, StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            double actualWidthInPixels = 0;

            foreach (string str in originalLines)
            {
                Size size = MeasureDrawnTextBitmapSize(str, Font);

                actualLine.Append(str + " ");
                actualWidthInPixels += size.Width;

                if (actualWidthInPixels > LineWidthPx)
                {
                    actualLine = actualLine.Remove(actualLine.ToString().Length - str.Length - 1, str.Length);
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualLine.Append(str + " ");
                    actualWidthInPixels = size.Width;
                }
            }

            if (actualLine.Length > 0)
            {
                wrappedLines.Add(actualLine.ToString());
            }

            return wrappedLines;
        }

        public static Bitmap DrawTextToBitmap(string Text, StringAlignment Alignment, Font Font, Color color, DrawMethod Method, RectangleF rectanglef)
        {
            StringFormat drawFormat = new StringFormat();
            Bitmap bmp;

            drawFormat.Alignment = Alignment;

            switch (Method)
            {
                case DrawMethod.Smallest:
                    {
                        Size size = MeasureDrawnTextBitmapSize(Text, Font);

                        if (size.Width == 0 || size.Height == 0)
                        {
                            bmp = new Bitmap(1, 1);
                        }
                        else
                        {
                            bmp = new Bitmap(size.Width, size.Height);
                        }

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            SetGraphicsQuality(g, PixelOffsetMode.HighQuality);

                            g.DrawString(Text, Font, new SolidBrush(color), 0, 0);

                            return bmp;
                        }

                    }
                case DrawMethod.NoWrap:
                    {
                        Rectangle rect = Rectangle.Ceiling(rectanglef);
                        bmp = new Bitmap(rect.Width, rect.Height);

                        if (rect.Width == 0 || rect.Height == 0)
                        {
                            bmp = new Bitmap(1, 1);
                        }
                        else
                        {
                            bmp = new Bitmap(rect.Width, rect.Height);
                        }

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            SetGraphicsQuality(g, PixelOffsetMode.HighQuality);

                            g.DrawString(Text, Font, new SolidBrush(color), rectanglef, drawFormat);

                            return bmp;
                        }

                    }
                case DrawMethod.LargestNoWrap:
                    {
                        Rectangle rect = Rectangle.Ceiling(rectanglef);

                        bmp = new Bitmap(rect.Width, rect.Height);

                        if (rect.Width == 0 || rect.Height == 0)
                        {
                            bmp = new Bitmap(1, 1);
                        }
                        else
                        {
                            bmp = new Bitmap(rect.Width, rect.Height);
                        }

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            int fontSize = MaxFontSize(Text, Font, rectanglef, false);

                            SetGraphicsQuality(g, PixelOffsetMode.HighQuality);

                            g.DrawString(Text, new Font(Font.FontFamily, fontSize, Font.Style, GraphicsUnit.Point), new SolidBrush(color), rectanglef, drawFormat);


                            return bmp;
                        }

                    }
                case DrawMethod.LargestWrap:
                    {
                        Rectangle rect = Rectangle.Ceiling(rectanglef);

                        if (rect.Width == 0 || rect.Height == 0)
                        {
                            bmp = new Bitmap(1, 1);
                        }
                        else
                        {
                            bmp = new Bitmap(rect.Width, rect.Height);
                        }

                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            int fontSize = MaxFontSize(Text, Font, rectanglef, true);

                            SetGraphicsQuality(g, PixelOffsetMode.HighQuality);

                            g.DrawString(Text, new Font(Font.FontFamily, fontSize, Font.Style, GraphicsUnit.Point), new SolidBrush(color), rectanglef, drawFormat);


                            return bmp;
                        }

                    }
            }
            return null;

        }

        #endregion

        #region Classes

        // By my own convention all classes should be in their own file, however sometimes it makes sense to include a class within the same file as it's parent Namespace

        #endregion

    }
}