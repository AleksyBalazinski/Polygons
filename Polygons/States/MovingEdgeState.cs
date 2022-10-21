using System.Diagnostics;

namespace Polygons.States
{
    internal class MovingEdgeState : State
    {
        readonly Segment movedEdge;
        Point previousPoint;
        public MovingEdgeState(Segment movedEdge, Point point)
        {
            this.movedEdge = movedEdge;
            previousPoint = point;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterEdgeMoved(e.Location);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterEdgeMoved(Point location)
        {
            Fixer fixer = new(context.relations, context.Canvas);
            fixer.Fix(movedEdge, location - previousPoint);
            previousPoint = location;

            context.Canvas.Invalidate();
        }
    }
}