using System.Diagnostics;


namespace Polygons.States
{
    internal class DeleteState : State
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

        private void DrawAfterVertexRemoved(PointF p)
        {
            foreach (var polygon in context.Polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} designated for deletion");
                        polygon.Delete(vertex);

                        context.Canvas.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
