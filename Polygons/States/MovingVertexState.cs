using System.Diagnostics;

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
            Fixer fixer = new(context.relations, context.Canvas);
            fixer.Fix(movedVertex, location);

            context.Canvas.Invalidate();
        }
    }
}