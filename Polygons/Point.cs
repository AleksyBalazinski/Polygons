namespace Polygons
{
    internal struct Point
    {
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Point(PointF a)
        {
            X = a.X;
            Y = a.Y;
        }
        public float X { get; set; }
        public float Y { get; set; }

        public static Point operator -(Point a)
            => new (-a.X, -a.Y);
        public static Point operator +(Point a, Point b)
            => new (a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b)
            => new (a.X - b.X, a.Y - b.Y);
        public static Point operator *(float c, Point a)
            => new (a.X * c, a.Y * c);
        public static Point operator /(Point a, float c)
            => new (a.X / c, a.Y / c);

        public static implicit operator PointF(Point a)
            => new System.Drawing.PointF(a.X, a.Y);
        public static implicit operator Point(System.Drawing.PointF a)
            => new (a);
        public static implicit operator Point(System.Drawing.Point a)
            => new (a);
    }
}