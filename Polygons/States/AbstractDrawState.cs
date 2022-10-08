using System.Diagnostics;

namespace Polygons.States
{
    abstract class AbstractDrawState : State
    {
        protected Polygon constructedPolygon;
        protected Segment drawnSegment;
        protected AbstractDrawState(Form1 context, Algorithm a, Polygon constructedPolygon, Segment drawnSegment) : base(context, a)
        {
            this.constructedPolygon = constructedPolygon;
            this.drawnSegment = drawnSegment;
        }

        protected abstract void DrawPolygonUnderConstruction(int x, int y);

        protected void DrawEdgeUnderConstruction(int x, int y)
        {
            if (drawnSegment != null)
            {
                drawnSegment.Point2 = new Point(x, y);
                context.Canvas.Invalidate();
            }
        }

        public override void canvas_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics);
            if (drawnSegment != null)
            {
                drawnSegment.Draw(e.Graphics, drawingAlgorithm);
            }
        }

        public override void DrawScene(Graphics g)
        {
            constructedPolygon.Draw(g, drawingAlgorithm);
            foreach (var polygon in context.Polygons)
                polygon.Draw(g, drawingAlgorithm);
        }
    }
}
