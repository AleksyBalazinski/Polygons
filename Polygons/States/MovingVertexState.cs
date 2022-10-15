using System.Diagnostics;

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
            if(movedVertex.fixedLenghts.Item2)
            {
                // fix p1 and rotate p2 to make it parallel to virtual segment p1 -> (x, y)
                float oldX = adjacentEdge1.Point2.X;
                float oldY = adjacentEdge1.Point2.Y;
                //Point oldDirection = new(oldX - adjacentEdge1.Point1.X, oldY - adjacentEdge1.Point1.Y);
                Point oldDirection = adjacentEdge1.Point2 - adjacentEdge1.Point1;
                Point newDirection = new(x - adjacentEdge1.Point1.X, y - adjacentEdge1.Point1.Y);
                (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection); ;
                Polygon? p = context.FindPolygon(movedVertex);
                p.ApplyParallelRelation(adjacentEdge1, adjacentEdge1.Point1, sin, cos);

                adjacentEdge2.Point1 = adjacentEdge1.Point2;
                context.Canvas.Invalidate();
            }
            else if (movedVertex.fixedLenghts.Item1)
            {
                // fix p1 and rotate p2 to make it parallel to virtual segment p1 -> (x, y)
                float oldX = adjacentEdge2.Point1.X;
                float oldY = adjacentEdge2.Point1.Y;
                Point oldDirection = new(oldX - adjacentEdge2.Point2.X, oldY - adjacentEdge2.Point2.Y);
                Point newDirection = new(x - adjacentEdge2.Point2.X, y - adjacentEdge2.Point2.Y);
                (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                Polygon? p = context.FindPolygon(movedVertex);
                p.ApplyParallelRelation1(adjacentEdge2, adjacentEdge2.Point2, sin, cos);

                adjacentEdge1.Point2 = adjacentEdge2.Point1;
                context.Canvas.Invalidate();
            }
            else
            {
                (int? relId1, int? relId2) = movedVertex.relationIds;
                if (relId1 != null && relId2 != null
                    && relId1 == relId2) // inside a chain
                {
                    var chain = adjacentEdge1.chain;
                    Segment first = chain[0];
                    Segment last = chain[^1];
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                    Point oldDirection = new Point(previousPoint.X - first.Point1.X, previousPoint.Y - first.Point1.Y);

                    movedVertex.Move(displacement);

                    Point newDirection = new Point(x - first.Point1.X, y - first.Point1.Y);

                    int indx = chain.IndexOf(adjacentEdge1);
                    Polygon? p = context.FindPolygon(chain);

                    var firstAdjacentEdges = first.adjacentEdges;
                    var lastAdjacentEdges = last.adjacentEdges;

                    (float sin1, float cos1) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    p!.ApplyParallelRelation(chain.Take(indx).ToList(), first.Point1, sin1, cos1);
                    p!.ApplyParallelRelation(chain.Skip(indx + 2).ToList(), first.Point1, sin1, cos1);

                    adjacentEdge1.MoveEnd(displacement);
                    if(indx != 0)
                        adjacentEdge1.MoveStartAbs(chain[indx - 1].Point2);

                    adjacentEdge2.MoveStart(displacement);
                    if(indx <= chain.Count - 3)
                        adjacentEdge2.MoveEndAbs(chain[indx + 2].Point1);
                    else
                    {
                        // rotate adjcaentEdge.Point2
                        Point v = new Point(adjacentEdge2.Point2.X - first.Point1.X, adjacentEdge2.Point2.Y - first.Point1.Y);
                        var vRot = Geometry.Rotate(v, sin1, cos1);
                        adjacentEdge2.Point2 = new Point(vRot.X + first.Point1.X, vRot.Y + first.Point1.Y);
                        (_, Vertex v2) = adjacentEdge2.endpoints;
                        v2.MoveAbs(adjacentEdge2.Point2);
                    }
                    
                    if(firstAdjacentEdges.Item1.chain != chain) // always satisfied???
                    {
                        firstAdjacentEdges.Item1.Point2 = first.Point1;
                    }
                    if(lastAdjacentEdges.Item2.chain != chain)
                    {
                        lastAdjacentEdges.Item2.Point1 = last.Point2;
                    }

                    previousPoint = new Point(x, y);

                    InvalidateRelatedChains(chain, (int)relId1, sin1, cos1);
                }
                else if(relId2 != null)
                {
                    var chain = adjacentEdge1.chain;
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                    Segment first = chain[0];
                    Point oldDirection = new Point(previousPoint.X - first.Point1.X, previousPoint.Y - first.Point1.Y);
                    Point newDirection = new Point(x - first.Point1.X, y - first.Point1.Y);
                    (float sin1, float cos1) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                    if (chain.Count == 1)
                    {
                        movedVertex.Move(displacement);
                        adjacentEdge1.MoveEnd(displacement);
                        adjacentEdge2.MoveStart(displacement);
                        previousPoint = new Point(x, y);
                        InvalidateRelatedChains(chain, (int)relId2, sin1, cos1);
                        context.Canvas.Invalidate();
                        return;
                    }
                    movedVertex.Move(displacement);

                    Polygon? p = context.FindPolygon(chain);
                    p.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), first.Point1, sin1, cos1);

                    adjacentEdge1.MoveEnd(displacement);
                    adjacentEdge1.MoveStartAbs(chain[chain.Count - 2].Point2);
                    adjacentEdge2.MoveStart(displacement);

                    previousPoint = new Point(x, y);

                    InvalidateRelatedChains(chain, (int)relId2, sin1, cos1);
                }
                else if(relId1 != null)
                {
                    var chain = adjacentEdge2.chain;
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                    Segment last = chain[^1];
                    Point oldDirection = new Point(previousPoint.X - last.Point2.X, previousPoint.Y - last.Point2.Y);
                    Point newDirection = new Point(x - last.Point2.X, y - last.Point2.Y);
                    (float sin1, float cos1) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                    if (chain.Count == 1)
                    {
                        var d = new Point(x - previousPoint.X, y - previousPoint.Y);
                        movedVertex.Move(d);
                        adjacentEdge1.MoveEnd(d);
                        adjacentEdge2.MoveStart(d);
                        previousPoint = new Point(x, y);
                        InvalidateRelatedChains(chain, (int)relId1, sin1, cos1);
                        context.Canvas.Invalidate();
                        return;
                    }
                    movedVertex.Move(displacement);

                    Polygon? p = context.FindPolygon(chain);
                    p.ApplyParallelRelation(chain.Skip(1).ToList(), last.Point2, sin1, cos1);

                    adjacentEdge2.MoveStart(displacement);
                    adjacentEdge2.MoveEndAbs(chain[1].Point1);
                    adjacentEdge1.MoveEnd(displacement);

                    previousPoint = new Point(x, y);

                    InvalidateRelatedChains(chain, (int)relId1, sin1, cos1);
                }
                else
                {
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                    movedVertex.Move(displacement);
                    adjacentEdge1.MoveEnd(displacement);
                    adjacentEdge2.MoveStart(displacement);
                    previousPoint = new Point(x, y);
                }
                
            }
            context.Canvas.Invalidate();
        }

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta)
        {
            foreach(var relatedChain in context.relations[relationId])
            {
                if(relatedChain != chain)
                {
                    Polygon? p = context.FindPolygon(relatedChain);
                    p.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }
    }
}
