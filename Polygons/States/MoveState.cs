namespace Polygons.States
{
    internal class MoveState : State
    {
        protected Point previousPoint;

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            HitTester hitTester = new(context.Polygons);
            var vertex = hitTester.GetHitVertex(e.Location);
            if (vertex != null)
            {
                previousPoint = e.Location;
                context.TransitionTo(new MovingVertexState(vertex));
                return;
            }

            var edge = hitTester.GetHitEdge(e.Location);
            if (edge != null)
            {
                previousPoint = e.Location;
                context.TransitionTo(new MovingEdgeState(edge, previousPoint));
                return;
            }

            var polygon = hitTester.GetHitPolygon(e.Location);
            if (polygon != null)
            {
                previousPoint = e.Location;
                context.TransitionTo(new MovingPolygonState(polygon, previousPoint));
                return;
            }
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }
    }
}
