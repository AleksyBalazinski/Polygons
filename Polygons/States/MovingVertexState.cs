namespace Polygons.States
{
    internal class MovingVertexState : State
    {
        readonly Vertex movedVertex;
        public MovingVertexState(Vertex movedVertex)
        {
            this.movedVertex = movedVertex;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            DrawAfterVertexMoved(e.Location);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterVertexMoved(Point location)
        {
            Fixer fixer = new();
            fixer.Fix(movedVertex, location);
            foreach (var p in context.Polygons)
            {
                if (p != movedVertex.polygon)
                {
                    fixer.FixOffshoot(p);
                }
            }

            context.Canvas.Invalidate();
        }
    }
}