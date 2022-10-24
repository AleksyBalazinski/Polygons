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

        public static void LineBresenhamThick(Graphics g, Point p1, Point p2)
        {
            float x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            float temp;
            if (steep)
            {
                temp = x1; x1 = y1; y1 = temp;
                temp = x2; x2 = y2; y2 = temp;
            }
            if (x1 > x2)
            {
                temp = x1; x1 = x2; x2 = temp;
                temp = y1; y1 = y2; y2 = temp;
            }
            float dx = x2 - x1;
            float dy = Math.Abs(y2 - y1);
            float error = dx / 2.0f;
            int ystep = (y1 < y2) ? 1 : -1;
            int y = (int)y1;

            int maxX = (int)x2;

            for (int x = (int)x1; x <= maxX; x++)
            {
                if (steep)
                {
                    PutThickPixel(g, y, x, 2);
                }
                else
                {
                    PutThickPixel(g, x, y, 2);
                }

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
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

        private static void PutThickPixel(Graphics g, float x, float y, int thickness)
        {
            thickness /= 2;
            using var brush = new SolidBrush(Color.Black);
            for (int xi = (int)x - thickness; xi < x + thickness; xi++)
            {
                for (int yi = (int)y - thickness; yi < y + thickness; yi++)
                {
                    PutPixel(g, xi, yi);
                }
            }

        }
    }
}
