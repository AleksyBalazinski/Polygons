using System.Diagnostics;

namespace Polygons.States
{
    internal class FixLengthState : State
    {
        public FixLengthState(Form1 context, Algorithm a) : base(context, a)
        {
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            DrawAfterLengthFixed(e.Location);
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void DrawAfterLengthFixed(Point p)
        {
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} will have fixed length");

                        double length = Utilities.QueryForEdgeLength(edge);
                        if (length <= 0)
                            MessageBox.Show($"Invalid input: Positive real values only.",
                                "Polygons", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        polygon.SetLength(edge, length);
                        polygon.FixedLengthEdges.Add(edge);

                        context.Canvas.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
