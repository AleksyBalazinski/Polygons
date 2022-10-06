using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal class Vertex : IShape
    {
        public Vertex(int x, int y)
        {
            Center = new Point(x, y);
            FillColor = Color.Black;
            Radius = 5;
        }

        public Color FillColor { get; set; }

        public Point Center { get; set; }
        public int Radius { get; set; }

        public void Draw(Graphics g)
        {
            using (var brush = new SolidBrush(FillColor))
                g.FillEllipse(brush, Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);
        }

        public bool HitTest(Point p)
        {
            throw new NotImplementedException();
        }

        public void Move(Point d)
        {
            Center = new Point(Center.X + d.X, Center.Y + d.Y);
        }

        public override string ToString() => String.Format($"({Center.X}, {Center.Y})");
    }
}
