using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Polygons
{
    internal class Polygon : IShape
    {
        List<Vertex> vertices;
        List<Segment> edges;
        
        public Polygon()
        {
            vertices = new List<Vertex>();
            edges = new List<Segment>();
        }

        public bool IsEmpty() => vertices.Count == 0;

        public Vertex GetVertex(int indx) => vertices[indx];
        public int Size { get { return vertices.Count; } }
        public void AddVertex(Vertex vertex) => vertices.Add(vertex);
        public void AddEdge(Segment edge) => edges.Add(edge);
        public override string ToString()
        {
            string res = "";
            foreach(var v in vertices)
                res += v.ToString();
            return res;
        }
        public Vertex? GetLastVertex() => vertices.LastOrDefault();

        public bool HitTest(Point p)
        {
            int xMin = int.MaxValue;
            int xMax = int.MinValue;
            int yMin = int.MaxValue;
            int yMax = int.MinValue;

            foreach(var v in vertices)
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

        public void Draw(Graphics g)
        {
            foreach (var v in vertices)
                v.Draw(g);
            foreach (var e in edges)
                e.Draw(g);
        }

        public void Move(Point d)
        {
            foreach (var v in vertices)
                v.Move(d);
            foreach (var e in edges)
                e.Move(d);
        }
    }
}
