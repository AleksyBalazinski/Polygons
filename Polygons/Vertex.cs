using System.Drawing.Drawing2D;

namespace Polygons
{
    internal class Vertex : IShape
    {
        public Vertex(int x, int y)
        {
            Center = new Point(x, y);
            Radius = 5;
        }

        public Vertex(Point p)
        {
            Center = p;
            Radius = 5;
        }

        public Color FillColor { get; set; }

        public Point Center { get; set; }
        public int Radius { get; set; }

        public void Draw(Graphics g, Algorithm a)
        {
            a.Apply(g, this);
        }

        public bool HitTest(Point p)
        {
            var hit = false;
            using (var path = GetPath())
                hit = path.IsVisible(p);
            return hit;

        }

        public void Move(Point d)
        {
            Center = new Point(Center.X + d.X, Center.Y + d.Y);
        }

        public void MoveAbs(Point p)
        {
            Center = new Point(p.X, p.Y);
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
