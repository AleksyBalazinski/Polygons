using System.Diagnostics;

namespace Polygons.States
{
    internal class AddVertexState : State
    {
        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            DrawAfterVertexAdded(e.Location);
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void DrawAfterVertexAdded(Point p)
        {
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} designated for adding a vertex");
                        polygon.Subdivide(edge);
                        if (edge.RelationId != null)
                        {
                            List<List<Segment>> chains = context.relations[(int)edge.RelationId];
                            List<Segment> part1 = edge.chain.SkipWhile(s => s != edge).Skip(1).ToList();
                            List<Segment> part2 = edge.chain.TakeWhile(s => s != edge).ToList();
                            if (part1.Count != 0)
                                chains.Add(part1);

                            if (part2.Count != 0)
                                chains.Add(part2);

                            chains.Remove(edge.chain);

                            foreach (var e in part1)
                                e.chain = part1;
                            foreach (var e in part2)
                                e.chain = part2;
                        }

                        context.Canvas.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
