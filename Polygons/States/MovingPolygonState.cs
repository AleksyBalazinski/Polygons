namespace Polygons.States
{
    internal class MovingPolygonState : State
    {
        Polygon movedPolygon;
        Point previousPoint;
        public MovingPolygonState(Form1 context, Algorithm a, Polygon movedPolygon, Point previousPoint) : base(context, a)
        {
            this.movedPolygon = movedPolygon;
            this.previousPoint = previousPoint;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterPolygonMoved(e.X, e.Y);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState(context, drawingAlgorithm));
        }

        private void DrawAfterPolygonMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedPolygon.Move(displacement);
            previousPoint = new Point(x, y);
            context.Canvas.Invalidate();
        }
    }
}
