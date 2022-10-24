using Polygons.Shapes;

namespace Polygons.States
{
    internal class VaryLengthState : State
    {
        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            HitTester hitTester = new(context.Polygons);
            Segment? edge = hitTester.GetHitEdge(e.Location);
            if (edge != null)
            {
                edge.fixedLength = false;
            }
            context.Canvas.Invalidate();
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }
    }
}
