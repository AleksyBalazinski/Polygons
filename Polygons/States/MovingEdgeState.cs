using Polygons.Shapes;

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
            Fixer fixer = new();
            fixer.Fix(movedEdge, location - previousPoint);
            previousPoint = location;
            foreach (var p in context.Polygons)
            {
                if (p != movedEdge.endpoints.Item1.polygon)
                {
                    fixer.FixOffshoot(p);
                }
            }

            context.Canvas.Invalidate();
        }
    }
}