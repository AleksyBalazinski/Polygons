using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal class Polygon
    {
        List<Vertex> vertices;
        
        public Polygon()
        {
            vertices = new List<Vertex>();
        }

        public bool IsEmpty() => vertices.Count == 0;

        public Vertex GetVertex(int indx) => vertices[indx];
        public (double, double) GetVertexCoordinates(int indx) => (vertices[indx].X, vertices[indx].Y);
        public int Size { get { return vertices.Count; } }
    }
}
