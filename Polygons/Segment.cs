using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal class Segment : IShape
    {
        public Segment() { SegmentWidth = 2; SegmentColor = Color.Black; }
        public Color SegmentColor { get; set; }
        public int SegmentWidth { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public bool HitTest(Point p)
        {
            throw new NotImplementedException();
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

        public void MoveOneEnd(Point newEnd)
        {
            Point2 = new Point(newEnd.X, newEnd.Y);
        }
    }
}
