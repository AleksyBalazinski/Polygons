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

                        context.Canvas.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
