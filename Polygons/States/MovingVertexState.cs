namespace Polygons.States
{
    internal class MovingVertexState : State
    {
        readonly Vertex movedVertex;
        readonly Segment adjacentEdge1;
        readonly Segment adjacentEdge2;
        Point previousPoint;
        public MovingVertexState(Vertex movedVertex, Segment adjacentEdge1, Segment adjacentEdge2, Point point)
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
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterVertexMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedVertex.Move(displacement);
            adjacentEdge1.MoveStart(displacement);
            adjacentEdge2.MoveEnd(displacement);
            previousPoint = new Point(x, y);
            // handle parallel relation
            (int? relId1, int? relId2) = movedVertex.relationIds;
            if(relId1 != null)
            {
                AdjustEdgesInRelation(relId1.Value, adjacentEdge1);
            }
            if (relId2 != null)
            {
                AdjustEdgesInRelation(relId2.Value, adjacentEdge2);
            }

            context.Canvas.Invalidate();
        }

        private void AdjustEdgesInRelation(int relationId, Segment edge)
        {
            foreach (var coupledEdge in context.Relations.relations[relationId])
            {
                if (coupledEdge != edge)
                {
                    Polygon? p = context.FindPolygon(coupledEdge);
                    if (p != null)
                        p.ApplyParallelRelation(coupledEdge, edge);
                    else throw new NullReferenceException("omg");
                }
            }
        }
    }
}
