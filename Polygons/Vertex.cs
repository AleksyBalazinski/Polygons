using System.Drawing.Drawing2D;

namespace Polygons
{
    internal class Vertex : IShape
    {
        public Vertex(float x, float y)
        {
            Center = new PointF(x, y);
            Radius = 5;
        }

        public Vertex(PointF p)
        {
            Center = p;
            Radius = 5;
        }

        public Color FillColor { get; set; }

        public PointF Center { get; set; }
        public float Radius { get; set; }

        public (int?, int?) relationIds;
        public (bool, bool) fixedLenghts;

        public void Draw(Graphics g, Algorithm a)
        {
            a.Apply(g, this);
        }

        public bool HitTest(PointF p)
        {
            var hit = false;
            using (var path = GetPath())
                hit = path.IsVisible(p);
            return hit;

        }

        public void Move(PointF d)
        {
            Center = new PointF(Center.X + d.X, Center.Y + d.Y);
        }

        public void MoveAbs(PointF p)
        {
            Center = new PointF(p.X, p.Y);
        }

        public override string ToString() => String.Format($"({Center.X}, {Center.Y})");

        private GraphicsPath GetPath()
        {
            var path = new GraphicsPath();
            path.AddEllipse(Center.X - Radius, Center.Y - Radius, 2 * Radius, 2 * Radius);
            return path;
        }
    }
}
