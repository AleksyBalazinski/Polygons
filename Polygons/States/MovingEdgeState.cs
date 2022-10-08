namespace Polygons.States
{
    internal class MovingEdgeState : State
    {
        Segment movedEdge;
        Segment adjacentEdge1;
        Segment adjacentEdge2;
        Vertex endpoint1;
        Vertex endpoint2;
        Point previousPoint;
        public MovingEdgeState(Form1 context, Algorithm a, Segment movedEdge, Segment adjacentEdge1, Segment adjacentEdge2, Vertex endpoint1, Vertex endpoint2, Point point) : base(context, a)
        {
            this.movedEdge = movedEdge;
            this.adjacentEdge1 = adjacentEdge1;
            this.adjacentEdge2 = adjacentEdge2;
            this.endpoint1 = endpoint1;
            this.endpoint2 = endpoint2;
            previousPoint = point;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterEdgeMoved(e.X, e.Y);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState(context, drawingAlgorithm));
        }

        private void DrawAfterEdgeMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedEdge.Move(displacement);
            adjacentEdge1.MoveEnd(displacement);
            adjacentEdge2.MoveStart(displacement);
            endpoint1.Move(displacement);
            endpoint2.Move(displacement);
            previousPoint = new Point(x, y);
            context.Canvas.Invalidate();
        }
    }
}
