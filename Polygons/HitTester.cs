using System.Diagnostics;
using Polygons.Shapes;

namespace Polygons
{
    internal class HitTester
    {
        private readonly List<Polygon> polygons;
        public HitTester(List<Polygon> polygons)
        {
            this.polygons = polygons;
        }

        public Vertex? GetHitVertex(Point p)
        {
            Vertex movedVertex;
            foreach (var polygon in polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} hit");
                        movedVertex = vertex;

                        return movedVertex;
                    }
                }
            }
            return null;
        }

        public Segment? GetHitEdge(Point p)
        {
            Segment movedEdge;
            foreach (var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");
                        movedEdge = edge;

                        return movedEdge;
                    }
                }
            }

            return null;
        }

        public Polygon? GetHitPolygon(Point p)
        {
            Polygon? movedPolygon;
            foreach (var polygon in polygons)
            {
                if (polygon.HitTest(p))
                {
                    Debug.WriteLine($"Polygon {polygon} hit");
                    movedPolygon = polygon;

                    return movedPolygon;
                }
            }

            return null;
        }
    }
}
