using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace Polygons.States
{
    internal class FixLengthState : State
    {
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

        private void DrawAfterLengthFixed(PointF p)
        {
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} will have fixed length");

                        float length = Utilities.QueryForEdgeLength(edge);
                        if (length <= 0)
                            MessageBox.Show($"Invalid input: Positive real values only.",
                                "Polygons", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        polygon.SetLength(edge, length);
                        //polygon.FixedLengthEdges.Add(edge);
                        edge.fixedLength = true;
                        (Vertex v1, Vertex v2) = polygon.GetEndpoints(edge);
                        v1.fixedLenghts.Item1 = true;
                        v2.fixedLenghts.Item2 = true;
                        (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
                        e1.fixedLengths.Item1 = true;
                        e2.fixedLengths.Item2 = true;

                        context.Canvas.Invalidate();
                        return;
                    }
                }
            }
        }
    }
}
