using System.Diagnostics;

namespace Polygons.States
{
    internal class DrawState : AbstractDrawState
    {
        public DrawState(Form1 context, Algorithm a) : base(context, a, new Polygon(), new Segment())
        {
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("canvas_mouseup in drawstate");
            DrawPolygonUnderConstruction(e.X, e.Y);
            context.TransitionTo(new DrawingState(context, drawingAlgorithm, constructedPolygon, drawnSegment));
        }

        protected override void DrawPolygonUnderConstruction(int x, int y)
        {
            Vertex v = new Vertex(x, y);

            Debug.WriteLine("Add vertex " + v.ToString());

            drawnSegment.Point1 = drawnSegment.Point2 = new Point(x, y);

            constructedPolygon.Vertices.Add(v);
        }
    }
}
