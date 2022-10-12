using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Polygons
{
    internal class Segment : IShape
    {
        public Segment() { SegmentWidth = 2; SegmentColor = Color.Black; }
        public Segment(PointF p1, PointF p2) { Point1 = p1; Point2 = p2; }
        public Color SegmentColor { get; set; }
        public int SegmentWidth { get; set; }
        public PointF Point1 { get; set; }
        public PointF Point2 { get; set; }
        public int? RelationId { get; set; }

        public (int?, int?) relationIds; // adjacent edges RENAME
        public bool fixedLength;
        public (bool, bool) fixedLengths;
        public List<Segment>? chain; // pointer to mother chain
        public float Length
        {
            get => (float)Math.Sqrt((Point1.X - Point2.X) * (Point1.X - Point2.X) 
                + (Point1.Y - Point2.Y) * (Point1.Y - Point2.Y));
            set
            {
                float coef = 0.5f * (value / Length - 1);
                PointF p1p2 = new PointF(Point2.X - Point1.X, Point2.Y - Point1.Y);
                Point2 = new PointF(Point2.X + coef * p1p2.X, Point2.Y + coef * p1p2.Y);
                Point1 = new PointF(Point1.X - coef * p1p2.X, Point1.Y - coef * p1p2.Y);
            }
        }
        public PointF MidPoint
        {
            get => new PointF((Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        }

        public bool HitTest(PointF p)
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

        public void Move(PointF d)
        {
            Point1 = new PointF(Point1.X + d.X, Point1.Y + d.Y);
            Point2 = new PointF(Point2.X + d.X, Point2.Y + d.Y);
        }

        public void MoveStart(PointF d)
        {
            Point1 = new PointF(Point1.X + d.X, Point1.Y + d.Y);
        }

        public void MoveStartAbs(PointF p)
        {
            Point1 = new PointF(p.X, p.Y);
        }

        public void MoveEnd(PointF d)
        {
            Point2 = new PointF(Point2.X + d.X, Point2.Y + d.Y);
        }

        public void MoveEndAbs(PointF p)
        {
            Point2 = new PointF(p.X, p.Y);
        }

        public void SetEqualTo(Segment reference)
        {
            Segment currentAtOrigin = new Segment(new PointF(0, 0), new PointF(Point2.X - Point1.X, Point2.Y - Point1.Y));
            Segment referenceAtOrigin = new Segment(new PointF(0, 0), new PointF(reference.Point1.X - reference.Point2.X, reference.Point1.Y - reference.Point2.Y));

            PointF displacement = new PointF(currentAtOrigin.Point2.X - referenceAtOrigin.Point2.X, currentAtOrigin.Point2.Y - referenceAtOrigin.Point2.Y);
            Point1 = new PointF(Point1.X + displacement.X, Point1.Y + displacement.Y);
        }

        public void SetParallelTo(Segment reference, bool isBefore) // isBefore guards us against a "positive feedback loop"
        {
            // BUG sequence of djacent edges in a relation -- we should move points alternatingly, i.e. start of the first edge, end of the second edge, etc.
            PointF vector = new PointF(Point2.X - Point1.X, Point2.Y - Point1.Y);
            PointF referenceVector = new PointF(reference.Point2.X - reference.Point1.X, reference.Point2.Y - reference.Point1.Y);
            if(DotProduct(referenceVector, vector) < 0) // favor non-self-intersecting polygons
                referenceVector = new PointF(reference.Point1.X - reference.Point2.X, reference.Point1.Y - reference.Point2.Y);

            (float sin, float cos) = AngleBetweenVectors(vector, referenceVector);
            Debug.WriteLine($"Before adjusting: sin = {sin}, cos = {cos}");

            PointF vectorRotated = Rotate(vector, sin, cos);
            Debug.WriteLine($"(1) vectorRotated = ({vectorRotated.X}, {vectorRotated.Y})");
            if(isBefore)
            {
                Debug.WriteLine("isBefore");
                Point1 = new PointF(Point2.X - vectorRotated.X, Point2.Y - vectorRotated.Y);
            }
                
            else
                Point2 = new PointF(Point1.X + vectorRotated.X, Point1.Y + vectorRotated.Y);

            (sin, cos) = AngleBetweenVectors(vectorRotated, referenceVector); // DEBUG
            Debug.WriteLine($"After adjusting: sin = {sin}, cos = {cos}");
        }

        private (float, float) AngleBetweenVectors(PointF a, PointF b)
        {
            float cosTheta = DotProduct(a, b) / (VectorLength(a) * VectorLength(b));
            float sinTheta = CrossProduct(a, b) / (VectorLength(a) * VectorLength(b));

            return (sinTheta, cosTheta);
        }

        private float DotProduct(PointF a, PointF b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        private float CrossProduct(PointF a, PointF b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        private float VectorLength(PointF a)
        {
            return MathF.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        private PointF Rotate(PointF v, float sinTheta, float cosTheta)
        {
            return new PointF(v.X * cosTheta - v.Y * sinTheta, v.X * sinTheta + v.Y * cosTheta);
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
