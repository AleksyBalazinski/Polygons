﻿namespace Polygons.States
{
    internal class MovingEdgeState : State
    {
        readonly Segment movedEdge;
        readonly Segment adjacentEdge1;
        readonly Segment adjacentEdge2;
        readonly Vertex endpoint1;
        readonly Vertex endpoint2;
        PointF previousPoint;
        public MovingEdgeState(Segment movedEdge, Segment adjacentEdge1, Segment adjacentEdge2, Vertex endpoint1, Vertex endpoint2, PointF point)
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
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterEdgeMoved(int x, int y)
        {
            var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
            movedEdge.Move(displacement);
            adjacentEdge1.MoveEnd(displacement);
            adjacentEdge2.MoveStart(displacement);
            endpoint1.Move(displacement);
            endpoint2.Move(displacement);
            previousPoint = new PointF(x, y);

            // handle parallel relation
            (int? relId1, int? relId2) = movedEdge.relationIds;
            if (relId1 != null)
            {
                AdjustEdgesInRelation(relId1.Value, adjacentEdge2);
            }
            if (relId2 != null)
            {
                AdjustEdgesInRelation(relId2.Value, adjacentEdge1);
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
