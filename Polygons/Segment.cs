using System.Drawing.Drawing2D;

namespace Polygons
{
    internal class Segment : IShape
    {
        public Segment() { SegmentWidth = 2; SegmentColor = Color.Black; }
        public Segment(Point p1, Point p2) { Point1 = p1; Point2 = p2; }
        public Color SegmentColor { get; set; }
        public int SegmentWidth { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public double Length
        {
            get => Math.Sqrt((Point1.X - Point2.X) * (Point1.X - Point2.X) 
                + (Point1.Y - Point2.Y) * (Point1.Y - Point2.Y));
            set
            {
                double coef = 0.5 * (value / Length - 1);
                Point p1p2 = new Point(Point2.X - Point1.X, Point2.Y - Point1.Y);
                Point2 = new Point((int)(Point2.X + coef * p1p2.X), (int)(Point2.Y + coef * p1p2.Y));
                Point1 = new Point((int)(Point1.X - coef * p1p2.X), (int)(Point1.Y - coef * p1p2.Y));
            }
        }
        public bool HitTest(Point p)
        {
            bool result = false;
            using (var path = GetPath())
            using (var pen = new Pen(SegmentColor, 4))
                result = path.IsOutlineVisible(p, pen);
            return result;
        }

        public void Draw(Graphics g, Algorithm a)
        {
            a.Apply(g, this);
        }

        public void Move(Point d)
        {
            Point1 = new Point(Point1.X + d.X, Point1.Y + d.Y);
            Point2 = new Point(Point2.X + d.X, Point2.Y + d.Y);
        }

        public void MoveStart(Point d)
        {
            Point1 = new Point(Point1.X + d.X, Point1.Y + d.Y);
        }

        public void MoveStartAbs(Point p)
        {
            Point1 = new Point(p.X, p.Y);
        }

        public void MoveEnd(Point d)
        {
            Point2 = new Point(Point2.X + d.X, Point2.Y + d.Y);
        }

        public void MoveEndAbs(Point p)
        {
            Point2 = new Point(p.X, p.Y);
        }

        public override string ToString()
        {
            return $"({Point1.X}, {Point1.Y})->({Point2.X}, {Point2.Y})";
        }

        private GraphicsPath GetPath()
        {
            var path = new GraphicsPath();
            path.AddLine(Point1, Point2);
            return path;
        }
    }
}
