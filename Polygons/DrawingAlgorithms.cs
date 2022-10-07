namespace Polygons
{
    internal static class DrawingAlgorithms
    {
        public static void LineBresenham(Graphics g, Point p1, Point p2)
        {
            int x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            /*
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            int px = 2 * dy - dx;
            int py = 2 * dx - dy;

            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);

            int x = x1;
            int y = y1;

            PutPixel(g, x, y);
            while (x < x2)
            {
                if (px < 0)
                {
                    px += incrE;
                    x++;
                }
                else
                {
                    px += incrNE;
                    x++;
                    y++;
                }
                PutPixel(g, x, y);
            }*/

            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
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

        public static void LineLibrary(Graphics g, Point p1, Point p2)
        {
            using (var pen = new Pen(Color.Black, 2))
                g.DrawLine(pen, p1, p2);
        }

        public static void CircleLibrary(Graphics g, Point center, int radius)
        {
            using (var brush = new SolidBrush(Color.Black))
                g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private static void PutPixel(Graphics g, int x, int y)
        {
            using (var brush = new SolidBrush(Color.Red))
                g.FillRectangle(brush, x, y, 1, 1);
        }
    }
}
