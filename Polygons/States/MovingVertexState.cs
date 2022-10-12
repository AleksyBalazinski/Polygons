using System.Diagnostics;

namespace Polygons.States
{
    internal class MovingVertexState : State
    {
        readonly Vertex movedVertex;
        readonly Segment adjacentEdge1;
        readonly Segment adjacentEdge2;
        PointF previousPoint;
        public MovingVertexState(Vertex movedVertex, Segment adjacentEdge1, Segment adjacentEdge2, PointF point)
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
            if(movedVertex.fixedLenghts.Item2)
            {
                // fix p1 and rotate p2 to make it parallel to virtual segment p1 -> (x, y)
                Debug.WriteLine("endpoint of an edge of fixed length");
                float oldX = adjacentEdge2.Point2.X;
                float oldY = adjacentEdge2.Point2.Y;
                adjacentEdge2.SetParallelTo(new Segment(new PointF(adjacentEdge2.Point1.X, adjacentEdge2.Point1.Y), new PointF(x, y)), false);

                var displacement = new PointF(adjacentEdge2.Point2.X - oldX, adjacentEdge2.Point2.Y - oldY);
                movedVertex.Move(displacement);
                adjacentEdge1.MoveStart(displacement);
                context.Canvas.Invalidate();
            }
            else if (movedVertex.fixedLenghts.Item1)
            {
                // fix p1 and rotate p2 to make it parallel to virtual segment p1 -> (x, y)
                Debug.WriteLine("endpoint of an edge of fixed length");
                float oldX = adjacentEdge1.Point1.X;
                float oldY = adjacentEdge1.Point1.Y;
                adjacentEdge1.SetParallelTo(new Segment(new PointF(adjacentEdge1.Point2.X, adjacentEdge1.Point2.Y), new PointF(x, y)), true);

                var displacement = new PointF(adjacentEdge1.Point1.X - oldX, adjacentEdge1.Point1.Y - oldY);
                movedVertex.Move(displacement);
                adjacentEdge2.MoveEnd(displacement);
                context.Canvas.Invalidate();
            }
            else
            {
                var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
                movedVertex.Move(displacement);
                adjacentEdge1.MoveStart(displacement);
                adjacentEdge2.MoveEnd(displacement);
                previousPoint = new PointF(x, y);
            }
            

            // handle parallel relation
            /*(int? relId1, int? relId2) = movedVertex.relationIds;
            if(relId1 != null)
            {
                AdjustEdgesInRelation(relId1.Value, adjacentEdge1);
            }
            if (relId2 != null)
            {
                AdjustEdgesInRelation(relId2.Value, adjacentEdge2);
            }*/

            context.Canvas.Invalidate();
        }

        private void AdjustEdgesInRelation(int relationId, Segment edge)
        {
            /*foreach (var coupledEdge in context.Relations.relations[relationId])
            {
                if (coupledEdge != edge)
                {
                    Polygon? p = context.FindPolygon(coupledEdge);
                    if (p != null)
                        p.ApplyParallelRelation(coupledEdge, edge);
                    else throw new NullReferenceException("omg");
                }
            }*/
        }
    }
}
