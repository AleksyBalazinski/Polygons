namespace Polygons
{
    internal class Polygon : IShape
    {
        public List<Vertex> Vertices { get; }
        public List<Segment> Edges { get; }
        
        public Polygon()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Segment>();
        }

        public bool IsEmpty() => Vertices.Count == 0;

        public Vertex GetVertex(int indx) => Vertices[indx];
        public int Size { get { return Vertices.Count; } }
        public void AddVertex(Vertex vertex) => Vertices.Add(vertex);
        public void AddEdge(Segment edge) => Edges.Add(edge);
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
        public Vertex? GetLastVertex() => Vertices.LastOrDefault();

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
