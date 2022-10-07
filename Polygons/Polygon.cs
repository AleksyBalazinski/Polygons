namespace Polygons
{
    internal class Polygon : IShape
    {
        public List<Vertex> Vertices { get; }
        public List<Segment> Edges { get; }

        public HashSet<Segment> FixedLengthEdges { get; }
        
        public Polygon()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Segment>();
            FixedLengthEdges = new HashSet<Segment>();
        }
        public override string ToString()
        {
            string res = "Vertices: ";
            foreach(var v in Vertices)
                res += v.ToString();
            res += "\nEdges: ";
            foreach (var e in Edges)
                res += e.ToString();
            return res;
        }

        public bool HitTest(Point p)
        {
            int xMin = int.MaxValue;
            int xMax = int.MinValue;
            int yMin = int.MaxValue;
            int yMax = int.MinValue;

            foreach(var v in Vertices)
            {
                if(v.Center.X < xMin)
                    xMin = v.Center.X;
                if(v.Center.X > xMax)
                    xMax = v.Center.X;
                if(v.Center.Y < yMin)
                    yMin = v.Center.Y;
                if(v.Center.Y > yMax)
                    yMax = v.Center.Y;
            }

            return p.X >= xMin && p.X <= xMax && p.Y >= yMin && p.Y <= yMax;
        }

        public void Draw(Graphics g, Algorithm a)
        {
            a.Apply(g, this);
        }

        public void Move(Point d)
        {
            foreach (var v in Vertices)
                v.Move(d);
            foreach (var e in Edges)
                e.Move(d);
        }
    }
}
