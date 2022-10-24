namespace Polygons.Shapes
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
        public override string ToString()
        {
            string res = "Vertices: ";
            foreach (var v in Vertices)
                res += v.ToString();
            res += "\nEdges: ";
            foreach (var e in Edges)
                res += e.ToString();
            return res;
        }

        public void Subdivide(Segment e)
        {
            int ei = Edges.IndexOf(e);
            Vertex mid = new(e.MidPoint);
            Segment e1 = new(e.Point1, mid.Center);
            Segment e2 = new(mid.Center, e.Point2);
            mid.adjacentEdges.Item1 = e1;
            mid.adjacentEdges.Item2 = e2;
            mid.polygon = e.endpoints.Item1.polygon;

            e1.endpoints.Item1 = e.endpoints.Item1;
            e1.endpoints.Item2 = mid;
            e2.endpoints.Item1 = mid;
            e2.endpoints.Item2 = e.endpoints.Item2;

            e1.endpoints.Item1.adjacentEdges.Item2 = e1;
            e2.endpoints.Item2.adjacentEdges.Item1 = e2;

            e.adjacentEdges.Item1.adjacentEdges.Item2 = e1;
            e.adjacentEdges.Item2.adjacentEdges.Item1 = e2;

            e1.adjacentEdges.Item1 = e.adjacentEdges.Item1;
            e1.adjacentEdges.Item2 = e2;
            e2.adjacentEdges.Item2 = e.adjacentEdges.Item2;
            e2.adjacentEdges.Item1 = e1;

            // removing relation
            if (e.RelationId != null)
            {
                e1.endpoints.Item1.relationIds.Item1 = null;
                e1.adjacentEdges.Item1.relationIds.Item1 = null;
                e2.endpoints.Item2.relationIds.Item2 = null;
                e2.adjacentEdges.Item2.relationIds.Item2 = null;
            }

            Vertices.Insert(ei + 1, mid);
            Edges.Insert(ei, e1);
            Edges.Insert(ei + 1, e2);
            Edges.Remove(e);
        }

        public void Delete(Vertex v)
        {
            (Segment edge1, Segment edge2) = v.adjacentEdges;
            int vi = Vertices.IndexOf(v);
            Segment newEdge = new Segment(edge1.Point1, edge2.Point2);
            newEdge.endpoints.Item1 = edge1.endpoints.Item1;
            newEdge.endpoints.Item2 = edge2.endpoints.Item2;
            edge2.endpoints.Item2.adjacentEdges.Item1 = newEdge;
            edge1.endpoints.Item1.adjacentEdges.Item2 = newEdge;

            newEdge.adjacentEdges.Item1 = edge1.adjacentEdges.Item1;
            newEdge.adjacentEdges.Item2 = edge2.adjacentEdges.Item2;

            edge1.adjacentEdges.Item1.adjacentEdges.Item2 = newEdge;
            edge2.adjacentEdges.Item2.adjacentEdges.Item1 = newEdge;

            if (vi == 0)
            {
                Edges.Add(newEdge);
            }
            else
            {
                Edges.Insert(vi - 1, newEdge);
            }

            // removing relation
            if (v.relationIds.Item2 != null)
            {
                edge1.endpoints.Item1.relationIds.Item1 = null;
                edge1.adjacentEdges.Item1.relationIds.Item1 = null;
            }
            if (v.relationIds.Item1 != null)
            {
                edge2.endpoints.Item2.relationIds.Item2 = null;
                edge2.adjacentEdges.Item2.relationIds.Item2 = null;
            }

            Vertices.Remove(v);
            Edges.Remove(edge2);
            Edges.Remove(edge1);
        }

        public bool HitTest(Point p)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

            foreach (var v in Vertices)
            {
                if (v.Center.X < xMin)
                    xMin = v.Center.X;
                if (v.Center.X > xMax)
                    xMax = v.Center.X;
                if (v.Center.Y < yMin)
                    yMin = v.Center.Y;
                if (v.Center.Y > yMax)
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