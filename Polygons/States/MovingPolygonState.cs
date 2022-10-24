using Polygons.Shapes;

namespace Polygons.States
{
    internal class MovingPolygonState : State
    {
        readonly Polygon movedPolygon;
        Point previousPoint;
        public MovingPolygonState(Polygon movedPolygon, Point previousPoint)
        {
            this.movedPolygon = movedPolygon;
            this.previousPoint = previousPoint;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterPolygonMoved(e.Location);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterPolygonMoved(Point location)
        {
            var displacement = location - previousPoint;
            movedPolygon.Move(displacement);
            previousPoint = location;
            context.Canvas.Invalidate();
        }
    }
}
