namespace Polygons.States
{
    internal class MovingVertexState : State
    {
        Vertex movedVertex;
        Segment adjacentEdge1;
        Segment adjacentEdge2;
        Point previousPoint;
        public MovingVertexState(Form1 context, Algorithm a, Vertex movedVertex, Segment adjacentEdge1, Segment adjacentEdge2, Point point) : base(context, a)
        {
            this.movedVertex = movedVertex;
            this.adjacentEdge1 = adjacentEdge1;
            this.adjacentEdge2 = adjacentEdge2;
            previousPoint = point;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterVertexMoved(e.X, e.Y);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState(context, drawingAlgorithm));
        }

        private void DrawAfterVertexMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedVertex.Move(displacement);
            adjacentEdge1.MoveStart(displacement);
            adjacentEdge2.MoveEnd(displacement);
            previousPoint = new Point(x, y);
            context.Canvas.Invalidate();
        }
    }
}
