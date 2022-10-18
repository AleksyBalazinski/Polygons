using System.Diagnostics;

namespace Polygons.States
{
    internal class MoveState : State
    {
        protected Point previousPoint;

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var vertexInfo = FindVertexToBeMoved(e.Location);
            if (vertexInfo != null)
            {
                Vertex vertex = vertexInfo;
                context.TransitionTo(new MovingVertexState(vertex));
                return;
            }

            var edgeInfo = FindEdgeToBeMoved(e.Location);
            if (edgeInfo != null)
            {
                (Segment edge, Segment edge1, Segment edge2, Vertex vertex1, Vertex vertex2) = edgeInfo.Value;
                context.TransitionTo(new MovingEdgeState(edge, edge1, edge2, vertex1, vertex2, previousPoint));
                return;
            }

            var polygon = FindPolygonToBeMoved(e.Location);
            if (polygon != null)
            {
                context.TransitionTo(new MovingPolygonState(polygon, previousPoint));
                return;
            }
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private Vertex? FindVertexToBeMoved(Point p)
        {
            Vertex movedVertex;
            foreach (var polygon in context.Polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} hit");
                        movedVertex = vertex;
                        previousPoint = p;

                        return movedVertex;
                    }
                }
            }
            return null;
        }

        private (Segment, Segment, Segment, Vertex, Vertex)? FindEdgeToBeMoved(Point p)
        {
            Segment movedEdge;
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");
                        movedEdge = edge;
                        (Segment edge1, Segment edge2) = edge.adjacentEdges;
                        (Vertex vertex1, Vertex vertex2) = edge.endpoints;

                        previousPoint = p;

                        return (movedEdge, edge1, edge2, vertex1, vertex2);
                    }
                }
            }

            return null;
        }

        private Polygon? FindPolygonToBeMoved(Point p)
        {
            Polygon? movedPolygon;
            foreach (var polygon in context.Polygons)
            {
                if (polygon.HitTest(p))
                {
                    Debug.WriteLine($"Polygon {polygon} hit");
                    movedPolygon = polygon;
                    previousPoint = p;

                    return movedPolygon;
                }
            }

            return null;
        }
    }
}
