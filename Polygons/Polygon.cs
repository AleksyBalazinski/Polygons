using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

namespace Polygons
{
    internal class Polygon : IShape
    {
        public List<Vertex> Vertices { get; }
        public List<Segment> Edges { get; }
        //public HashSet<Segment> FixedLengthEdges { get; }
        
        public Polygon()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Segment>();
            //FixedLengthEdges = new HashSet<Segment>();
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

        public (Segment, Segment) GetAdjacentEdges(Vertex v) // TODO cache results
        {
            int vi = Vertices.IndexOf(v);
            Segment edge2 = Edges[vi];
            Segment edge1;
            if (vi == 0)
                edge1 = Edges[^1];
            else
                edge1 = Edges[vi - 1];

            return (edge1, edge2);
        }

        public (Segment, Segment) GetAdjacentEdges(Segment e) // TODO cache results
        {
            int ei = Edges.IndexOf(e);
            Segment edge1, edge2;
            if (ei == 0)
                edge1 = Edges[^1];
            else
                edge1 = Edges[ei - 1];

            if (ei == Edges.Count - 1)
                edge2 = Edges[0];
            else
                edge2 = Edges[ei + 1];

            return (edge1, edge2);
        }

        public (Vertex, Vertex) GetEndpoints(Segment e) // TODO cache results
        {
            int ei = Edges.IndexOf(e);
            Vertex vertex1, vertex2;
            if (ei == 0)
                vertex1 = Vertices[0];
            else
                vertex1 = Vertices[ei];

            if (ei == Edges.Count - 1)
                vertex2 = Vertices[0];
            else
                vertex2 = Vertices[ei + 1];

            return (vertex1, vertex2);
        }

        public void Subdivide(Segment e)
        {
            int ei = Edges.IndexOf(e);
            Vertex mid = new Vertex(e.MidPoint);
            Vertices.Insert(ei + 1, mid);
            Edges.Insert(ei, new Segment(e.Point1, mid.Center));
            Edges.Insert(ei + 1, new Segment(mid.Center, e.Point2));
            Edges.Remove(e);
        }

        public void SetLength(Segment e, float length)
        {
            e.Length = length;

            int ei = Edges.IndexOf(e);
            Vertices[ei].MoveAbs(e.Point1);
            if (ei == Edges.Count - 1)
                Vertices[0].MoveAbs(e.Point2);
            else
                Vertices[ei + 1].MoveAbs(e.Point2);

            if (ei == 0)
                Edges[^1].MoveEndAbs(e.Point1);
            else
                Edges[ei - 1].MoveEndAbs(e.Point1);

            if(ei == Edges.Count - 1)
                Edges[0].MoveStartAbs(e.Point2);
            else
                Edges[ei + 1].MoveStartAbs(e.Point2);
        }

        public void Delete(Vertex v)
        {
            (Segment edge1, Segment edge2) = GetAdjacentEdges(v);
            int vi = Vertices.IndexOf(v);
            if (vi == 0)
                Edges.Add(new Segment(edge1.Point1, edge2.Point2));
            else
                Edges.Insert(vi - 1, new Segment(edge1.Point1, edge2.Point2));

            Vertices.Remove(v);
            Edges.Remove(edge2);
            Edges.Remove(edge1);
        }

        public void ApplyParallelRelation(Segment edge, PointF axis, float sinTheta, float cosTheta)
        {
            PointF v = new PointF(edge.Point2.X - axis.X, edge.Point2.Y - axis.Y);
            var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
            edge.Point2 = new PointF(vRot.X + axis.X, vRot.Y + axis.Y);
            (Vertex vertex1, Vertex vertex2) = GetEndpoints(edge);
            vertex2.Center = edge.Point2;
        }

        public void ApplyParallelRelation1(Segment edge, PointF axis, float sinTheta, float cosTheta)
        {
            PointF v = new PointF(edge.Point1.X - axis.X, edge.Point1.Y - axis.Y);
            var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
            edge.Point1 = new PointF(vRot.X + axis.X, vRot.Y + axis.Y);
            (Vertex vertex1, _) = GetEndpoints(edge);
            vertex1.Center = edge.Point1;
        }

        public void ApplyParallelRelation12(Segment edge, PointF axis, float sinTheta, float cosTheta)
        {
            PointF v = new(edge.Point2.X - axis.X, edge.Point2.Y - axis.Y);
            var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
            edge.Point2 = new PointF(vRot.X + axis.X, vRot.Y + axis.Y);

            PointF u = new(edge.Point1.X - axis.X, edge.Point1.Y - axis.Y);
            var uRot = Geometry.Rotate(u, sinTheta, cosTheta);
            edge.Point1 = new PointF(uRot.X + axis.X, uRot.Y + axis.Y);

            (Vertex vertex1, Vertex vertex2) = GetEndpoints(edge);
            vertex1.Center = edge.Point1;
            vertex2.Center = edge.Point2;
        }

        public void ApplyParallelRelation(List<Segment> chain, PointF axis, float sinTheta, float cosTheta)
        {
            foreach(var e in chain)
            {
                (Vertex vertex1, Vertex vertex2) = GetEndpoints(e);
                PointF v = new PointF(e.Point1.X - axis.X, e.Point1.Y - axis.Y);
                var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
                e.Point1 = new PointF(vRot.X + axis.X, vRot.Y + axis.Y);
                vertex1.Center = e.Point1;

                PointF u = new PointF(e.Point2.X - axis.X, e.Point2.Y - axis.Y);
                var uRot = Geometry.Rotate(u, sinTheta, cosTheta);
                e.Point2 = new PointF(uRot.X + axis.X, uRot.Y + axis.Y);
                vertex2.Center = e.Point2;
                
                if(e == chain[^1])
                {
                    (_, Segment next) = GetAdjacentEdges(e);
                    next.Point1 = e.Point2;
                }
            }
        }

        public void ApplyTranslation(List<Segment> chain, PointF displacement)
        {
            foreach(var e in chain)
            {
                e.Point1 = new PointF(e.Point1.X + displacement.X, e.Point1.Y + displacement.Y);
                e.Point2 = new PointF(e.Point2.X + displacement.X, e.Point2.Y + displacement.Y);
                (Vertex vertex1, Vertex vertex2) = GetEndpoints(e);
                vertex1.Center = e.Point1;
                vertex2.Center = e.Point2;

                if(e == chain[0])
                {
                    (Segment prev, _) = GetAdjacentEdges(e);
                    prev.Point2 = e.Point1;
                }
                if (e == chain[^1])
                {
                    (_, Segment next) = GetAdjacentEdges(e);
                    next.Point1 = e.Point2;
                }
            }
        }

        private bool IsBefore(Segment s1, Segment s2)
        {
            int indxS1 = Edges.IndexOf(s1);
            int indxS2 = Edges.IndexOf(s2);

            if(indxS1 < indxS2 || indxS1 == Edges.Count - 1 && indxS2 == 0)
                return true;
            return false;
        }

        public bool HitTest(PointF p)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

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

        public void Move(PointF d)
        {
            foreach (var v in Vertices)
                v.Move(d);
            foreach (var e in Edges)
                e.Move(d);
        }
    }
}
