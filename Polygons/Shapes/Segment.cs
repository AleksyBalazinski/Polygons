using System.Drawing.Drawing2D;

namespace Polygons.Shapes
{
    internal class Segment : IShape
    {
        public Segment() { SegmentWidth = 2; SegmentColor = Color.Black; }
        public Segment(Point p1, Point p2) { Point1 = p1; Point2 = p2; }
        public Color SegmentColor { get; set; }
        public int SegmentWidth { get; set; }
        private Point point1;
        private Point point2;
        public Point Point1
        {
            get => point1;
            set
            {
                point1 = value;
                controlPoints[0] = point1;
            }
        }
        public Point Point2
        {
            get => point2;
            set
            {
                point2 = value;
                // we're using the fact that point1 is already defined
                controlPoints[1] = 0.67f * point1 + 0.33f * point2;
                controlPoints[2] = 0.33f * point1 + 0.67f * point2;
                controlPoints[3] = point2;
            }
        }
        public int? RelationId { get; set; }

        public (int?, int?) relationIds; // adjacent edges
        public bool fixedLength;
        public float declaredLength;
        public List<Segment>? chain; // pointer to mother chain
        public (Vertex, Vertex) endpoints;
        public (Segment, Segment) adjacentEdges;
        public Point[] controlPoints = new Point[4]; // set of control points defining a Bezier curve
        public bool AllowsBezier { get => RelationId == null && !fixedLength; }
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
