namespace Polygons
{
    internal static class DrawingAlgorithms
    {
        public static void LineBresenham(Graphics g, Point p1, Point p2)
        {
            float x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            float x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;

            dx = x2 - x1;
            dy = y2 - y1;
            dx1 = Math.Abs(dx);
            dy1 = Math.Abs(dy);
            px = 2 * dy1 - dx1;
            py = 2 * dx1 - dy1;
            if (dy1 <= dx1)
            {
                if (dx >= 0)
                {
                    x = x1;
                    y = y1;
                    xe = x2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    xe = x1;
                }
                PutPixel(g, x, y);
                for (i = 0; x < xe; i++)
                {
                    x = x + 1;
                    if (px < 0)
                    {
                        px = px + 2 * dy1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            y = y + 1;
                        }
                        else
                        {
                            y = y - 1;
                        }
                        px = px + 2 * (dy1 - dx1);
                    }
                    PutPixel(g, x, y);
                }
            }
            else
            {
                if (dy >= 0)
                {
                    x = x1;
                    y = y1;
                    ye = y2;
                }
                else
                {
                    x = x2;
                    y = y2;
                    ye = y1;
                }
                PutPixel(g, x, y);
                for (i = 0; y < ye; i++)
                {
                    y = y + 1;
                    if (py <= 0)
                    {
                        py = py + 2 * dx1;
                    }
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        {
                            x = x + 1;
                        }
                        else
                        {
                            x = x - 1;
                        }
                        py = py + 2 * (dx1 - dy1);
                    }
                    PutPixel(g, x, y);
                }
            }
        }

        public static void LineXiaolinWu(Graphics g, Point p1, Point p2)
        {
            float x0 = p1.X, x1 = p2.X, y0 = p1.Y, y1 = p2.Y;
            static int ipart(float x) => (int)x;
            static int round(float x) => ipart(x + 0.5f);
            static float fpart(float x)
            {
                if (x < 0)
                    return 1 - (x - MathF.Floor(x));
                return x - MathF.Floor(x);
            }
            static float rfpart(float x) => 1 - fpart(x);

            bool steep = Math.Abs(p2.Y - p1.Y) > Math.Abs(p2.X - p1.X);
            float temp;
            if (steep)
            {
                temp = x0; x0 = y0; y0 = temp;
                temp = x1; x1 = y1; y1 = temp;
            }
            if (x0 > x1)
            {
                temp = x0; x0 = x1; x1 = temp;
                temp = y0; y0 = y1; y1 = temp;
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient;
            if (dx == 0 && dy == 0)
                gradient = 1;
            else
                gradient = dy / dx;

            float xEnd = round(x0);
            float yEnd = y0 + gradient * (xEnd - x0);
            float xGap = rfpart(x0 + 0.5f);
            float xPixel1 = xEnd;
            float yPixel1 = ipart(yEnd);

            if (steep)
            {
                PutPixel(g, yPixel1, xPixel1, rfpart(yEnd) * xGap);
                PutPixel(g, yPixel1 + 1, xPixel1, fpart(yEnd) * xGap);
            }
            else
            {
                PutPixel(g, xPixel1, yPixel1, rfpart(yEnd) * xGap);
                PutPixel(g, xPixel1, yPixel1 + 1, fpart(yEnd) * xGap);
            }

            float intery = yEnd + gradient;

            xEnd = round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = fpart(x1 + 0.5f);

            float xPixel2 = xEnd;
            float yPixel2 = ipart(yEnd);

            if (steep)
            {
                PutPixel(g, yPixel2, xPixel2, rfpart(yEnd) * xGap);
                PutPixel(g, yPixel2 + 1, xPixel2, fpart(yEnd) * xGap);
            }
            else
            {
                PutPixel(g, xPixel2, yPixel2, rfpart(yEnd) * xGap);
                PutPixel(g, xPixel2, yPixel2 + 1, fpart(yEnd) * xGap);
            }

            if (steep)
            {
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    PutPixel(g, ipart(intery), x, rfpart(intery));
                    PutPixel(g, ipart(intery) + 1, x, fpart(intery));
                    intery += gradient;
                }
            }
            else
            {
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    PutPixel(g, x, ipart(intery), rfpart(intery));
                    PutPixel(g, x, ipart(intery) + 1, fpart(intery));
                    intery += gradient;
                }
            }
        }

        public static void LineLibrary(Graphics g, Point p1, Point p2)
        {
            using var pen = new Pen(Color.Black, 2);
            g.DrawLine(pen, p1, p2);
        }

        public static void CircleLibrary(Graphics g, Point center, float radius)
        {
            using var brush = new SolidBrush(Color.Black);
            g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private static void PutPixel(Graphics g, float x, float y)
        {
            using var brush = new SolidBrush(Color.Black);
            g.FillRectangle(brush, x, y, 1, 1);
        }

        private static void PutPixel(Graphics g, float x, float y, float c)
        {
            int alpha = (int)(c * 255f);
            if (alpha > 255)
                alpha = 255;
            if (alpha < 0)
                alpha = 0;
            using var brush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
            g.FillRectangle(brush, x, y, 1, 1);
        }
    }
}
