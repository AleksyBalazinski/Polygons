using Polygons.Shapes;
using System.Diagnostics;

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
            foreach (var polygon in polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} hit");

                        return vertex;
                    }
                }
            }
            return null;
        }

        public Segment? GetHitEdge(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");

                        return edge;
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

        public (Segment, int)? GetHitControlPoint(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (Utilities.IsOnPoint(edge.controlPoints[1], p, 10))
                        return (edge, 1);
                    if (Utilities.IsOnPoint(edge.controlPoints[2], p, 10))
                        return (edge, 2);
                }
            }

            return null;
        }
    }
}
