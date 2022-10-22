using System.Diagnostics;

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

        private void DrawAfterLengthFixed(Point location)
        {
            Segment? edge = GetHitEdge(location);
            if (edge != null)
            {
                Debug.WriteLine($"Edge {edge} will have fixed length");

                float length = Utilities.QueryForEdgeLength(edge);
                if (length <= 0)
                    MessageBox.Show($"Invalid input: Positive real values only.",
                        "Polygons", MessageBoxButtons.OK, MessageBoxIcon.Error);

                edge.Length = length;
                edge.fixedLength = true;
                edge.declaredLength = length;
                Fixer fixer = new();
                fixer.Fix(edge, new Point(0, 0));
                foreach (var p in context.Polygons)
                {
                    if (p != edge.endpoints.Item1.polygon)
                    {
                        fixer.FixOffshoot(p);
                    }
                }

                context.Canvas.Invalidate();
            }
        }

        private Segment? GetHitEdge(Point p)
        {
            Segment hitEdge;
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");
                        hitEdge = edge;

                        return hitEdge;
                    }
                }
            }

            return null;
        }
    }
}
