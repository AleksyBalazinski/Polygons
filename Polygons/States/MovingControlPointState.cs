using Polygons.Shapes;
using System.Diagnostics;

namespace Polygons.States
{
    internal class MovingControlPointState : State
    {
        readonly Segment edge;
        readonly int cpId;
        public MovingControlPointState(Segment edge, int cpId)
        {
            this.edge = edge;
            this.cpId = cpId;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine($"Moving cp no. {cpId} of edge {edge} to ({e.X}, {e.Y})");
            edge.controlPoints[cpId] = new Point(e.X, e.Y);
            context.Canvas.Invalidate();
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }
    }
}
