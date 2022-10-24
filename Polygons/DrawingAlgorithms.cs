using System.Diagnostics;

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

        public static void PolygonStandard(Graphics g, Shapes.Polygon polygon, Algorithm a)
        {
            foreach (var v in polygon.Vertices)
            {
                v.Draw(g, a);
                Font font = new("Arial", 10);
                SolidBrush brush = new(Color.Black);
#if DEBUG
                g.DrawString($"({v.Center.X},{v.Center.Y})", font, brush, new Point(v.Center.X + 5, v.Center.Y + 5));
#endif
            }

            foreach (var e in polygon.Edges)
            {
                e.Draw(g, a);
                Font font = new("Arial", 12);
                SolidBrush brush = new(Color.Black);
                if (e.fixedLength)
                    brush = new SolidBrush(Color.DarkGray);

                g.DrawString(string.Format("{0:0.00}", e.Length), font, brush, new Point((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2));
                if (e.RelationId != null)
                {
                    font = new Font("Arial", 12);
                    brush = new SolidBrush(Color.Orange);

                    g.DrawString($"{e.RelationId.Value}", font, brush, new Point((e.Point1.X + e.Point2.X) / 2 - 20, (e.Point1.Y + e.Point2.Y) / 2 - 20));
                }
            }
        }

        public static void PolygonBezier(Graphics g, Shapes.Polygon polygon, Algorithm a)
        {
            foreach (var v in polygon.Vertices)
            {
                v.Draw(g, a);
                Font font = new("Arial", 10);
                SolidBrush brush = new(Color.Black);
#if DEBUG
                g.DrawString($"({v.Center.X},{v.Center.Y})", font, brush, new Point(v.Center.X + 5, v.Center.Y + 5));
#endif
            }

            foreach (var e in polygon.Edges)
            {
                if (e.AllowsBezier) // only free edges can be made into Bezier curves
                {
                    Debug.WriteLine("drawing bezier");
                    const int cpRadius = 5;
                    using var brush = new SolidBrush(Color.Black);
                    g.FillRectangle(brush, e.controlPoints[1].X - cpRadius, e.controlPoints[1].Y - cpRadius, cpRadius * 2, cpRadius * 2);
                    g.FillRectangle(brush, e.controlPoints[2].X - cpRadius, e.controlPoints[2].Y - cpRadius, cpRadius * 2, cpRadius * 2);

                    using var pen = new Pen(Color.Orange, 2);
                    g.DrawBezier(pen, e.controlPoints[0], e.controlPoints[1], e.controlPoints[2], e.controlPoints[3]);
                }
                else
                {
                    e.Draw(g, a);
                    Font font = new("Arial", 12);
                    SolidBrush brush = new(Color.Black);
                    if (e.fixedLength)
                        brush = new SolidBrush(Color.DarkGray);

                    g.DrawString(string.Format("{0:0.00}", e.Length), font, brush, new Point((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2));
                    if (e.RelationId != null)
                    {
                        font = new Font("Arial", 12);
                        brush = new SolidBrush(Color.Orange);

                        g.DrawString($"{e.RelationId.Value}", font, brush, new Point((e.Point1.X + e.Point2.X) / 2 - 20, (e.Point1.Y + e.Point2.Y) / 2 - 20));
                    }
                }
            }
        }

        private static void PutPixel(Graphics g, float x, float y)
        {
            using var brush = new SolidBrush(Color.Red);
            g.FillRectangle(brush, x, y, 1, 1);
        }
    }
}
