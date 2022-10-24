using Polygons.Shapes;
using System.Diagnostics;

namespace Polygons.States
{
    internal class DeleteVertexState : State
    {
        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            DrawAfterVertexRemoved(e.Location);
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void DrawAfterVertexRemoved(Point p)
        {
            HitTester hitTester = new(context.Polygons);
            var vertex = hitTester.GetHitVertex(p);
            if (vertex == null)
                return;

            Polygon polygon = vertex.polygon;
            Debug.WriteLine($"Vertex {vertex} designated for deletion");
            if (polygon.Vertices.Count == 3)
            {
                foreach (var e in polygon.Edges)
                {
                    if (e.RelationId != null)
                    {
                        context.relations[(int)e.RelationId].Remove(e.chain!);
                    }
                }
                context.Polygons.Remove(polygon);
            }
            if (vertex.relationIds.Item1 != null && vertex.relationIds.Item1 == vertex.relationIds.Item2)
            {
                Segment prevEdge = vertex.adjacentEdges.Item1;
                Segment nextEdge = vertex.adjacentEdges.Item2;
                List<List<Segment>> chains = context.relations[(int)prevEdge.RelationId!];
                var chain = prevEdge.chain!;
                List<Segment> part1 = chain.SkipWhile(s => s != nextEdge).Skip(1).ToList();
                List<Segment> part2 = chain.TakeWhile(s => s != prevEdge).ToList();
                if (part1.Count != 0)
                    chains.Add(part1);

                if (part2.Count != 0)
                    chains.Add(part2);

                foreach (var e in part1)
                    e.chain = part1;
                foreach (var e in part2)
                    e.chain = part2;

                chains.Remove(nextEdge.chain!);
                chains.Remove(prevEdge.chain!);
            }
            else if (vertex.relationIds.Item1 != null) // next edge is in relation
            {
                // delete next edge from its chain
                Segment nextEdge = vertex.adjacentEdges.Item2;
                List<List<Segment>> chains = context.relations[(int)nextEdge.RelationId!];
                List<Segment> part1 = nextEdge.chain!.SkipWhile(s => s != nextEdge).Skip(1).ToList();
                List<Segment> part2 = nextEdge.chain!.TakeWhile(s => s != nextEdge).ToList();
                if (part1.Count != 0)
                    chains.Add(part1);

                if (part2.Count != 0)
                    chains.Add(part2);

                foreach (var e in part1)
                    e.chain = part1;
                foreach (var e in part2)
                    e.chain = part2;

                chains.Remove(nextEdge.chain!);
            }
            else if (vertex.relationIds.Item2 != null) // next edge is in relation
            {
                // delete previous edge from its chain
                Segment prevEdge = vertex.adjacentEdges.Item1;
                List<List<Segment>> chains = context.relations[(int)prevEdge.RelationId!];
                List<Segment> part1 = prevEdge.chain!.SkipWhile(s => s != prevEdge).Skip(1).ToList();
                List<Segment> part2 = prevEdge.chain!.TakeWhile(s => s != prevEdge).ToList();
                if (part1.Count != 0)
                    chains.Add(part1);

                if (part2.Count != 0)
                    chains.Add(part2);

                foreach (var e in part1)
                    e.chain = part1;
                foreach (var e in part2)
                    e.chain = part2;

                chains.Remove(prevEdge.chain!);
            }
            polygon.Delete(vertex);

            context.Canvas.Invalidate();
        }
    }
}
