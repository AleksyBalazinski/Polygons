using System.Drawing.Drawing2D;
using System.Diagnostics;

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
        public int? RelationId { get; set; }

        public (int?, int?) relationIds; // adjacent edges RENAME
        public bool fixedLength;
        public float declaredLength;
        public (bool, bool) fixedLengths;
        public List<Segment>? chain; // pointer to mother chain
        public (Vertex, Vertex) endpoints;
        public (Segment, Segment) adjacentEdges;
        public float Length
        {
            get => MathF.Sqrt((Point1.X - Point2.X) * (Point1.X - Point2.X)
                + (Point1.Y - Point2.Y) * (Point1.Y - Point2.Y));
            set
            {
                float coef = 0.5f * (value / Length - 1);
                Point p1p2 = new Point(Point2.X - Point1.X, Point2.Y - Point1.Y);
                Point2 = new Point(Point2.X + coef * p1p2.X, Point2.Y + coef * p1p2.Y);
                Point1 = new Point(Point1.X - coef * p1p2.X, Point1.Y - coef * p1p2.Y);
            }
        }
        public Point MidPoint
        {
            get => (Point1 + Point2) / 2;
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
            Point1 += d;
            Point2 += d;
        }

        public void MoveStart(Point d)
        {
            Point1 += d;
        }

        public void MoveStartAbs(Point p)
        {
            Point1 = p;
        }

        public void MoveEnd(Point d)
        {
            Point2 += d;
        }

        public void MoveEndAbs(Point p)
        {
            Point2 = p;
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
