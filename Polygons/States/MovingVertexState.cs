using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

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
                /*Debug.WriteLine("endpoint of an edge of fixed length");
                float oldX = adjacentEdge2.Point2.X;
                float oldY = adjacentEdge2.Point2.Y;
                adjacentEdge2.SetParallelTo(new Segment(new PointF(adjacentEdge2.Point1.X, adjacentEdge2.Point1.Y), new PointF(x, y)), false);

                var displacement = new PointF(adjacentEdge2.Point2.X - oldX, adjacentEdge2.Point2.Y - oldY);
                movedVertex.Move(displacement);
                adjacentEdge1.MoveStart(displacement);
                context.Canvas.Invalidate();*/
            }
            else if (movedVertex.fixedLenghts.Item1)
            {
                // fix p1 and rotate p2 to make it parallel to virtual segment p1 -> (x, y)
                /*Debug.WriteLine("endpoint of an edge of fixed length");
                float oldX = adjacentEdge1.Point1.X;
                float oldY = adjacentEdge1.Point1.Y;
                adjacentEdge1.SetParallelTo(new Segment(new PointF(adjacentEdge1.Point2.X, adjacentEdge1.Point2.Y), new PointF(x, y)), true);

                var displacement = new PointF(adjacentEdge1.Point1.X - oldX, adjacentEdge1.Point1.Y - oldY);
                movedVertex.Move(displacement);
                adjacentEdge2.MoveEnd(displacement);
                context.Canvas.Invalidate();*/
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
                    var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
                    PointF oldDirection = new PointF(previousPoint.X - first.Point1.X, previousPoint.Y - first.Point1.Y);

                    movedVertex.Move(displacement);

                    PointF newDirection = new PointF(x - first.Point1.X, y - first.Point1.Y);

                    int indx = chain.IndexOf(adjacentEdge1);
                    Polygon? p = context.FindPolygon(chain);

                    var firstAdjacentEdges = p.GetAdjacentEdges(first);
                    var lastAdjacentEdges = p.GetAdjacentEdges(last);

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
                        PointF v = new PointF(adjacentEdge2.Point2.X - first.Point1.X, adjacentEdge2.Point2.Y - first.Point1.Y);
                        var vRot = Geometry.Rotate(v, sin1, cos1);
                        adjacentEdge2.Point2 = new PointF(vRot.X + first.Point1.X, vRot.Y + first.Point1.Y);
                        (_, Vertex v2) = p.GetEndpoints(adjacentEdge2);
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

                    previousPoint = new PointF(x, y);

                    InvalidateRelatedChains(chain, (int)relId1, sin1, cos1);
                }
                else if(relId2 != null)
                {
                    var chain = adjacentEdge1.chain;
                    Segment first = chain[0];
                    var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
                    PointF oldDirection = new PointF(previousPoint.X - first.Point1.X, previousPoint.Y - first.Point1.Y);

                    movedVertex.Move(displacement);

                    PointF newDirection = new PointF(x - first.Point1.X, y - first.Point1.Y);

                    (float sin1, float cos1) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Polygon? p = context.FindPolygon(chain);
                    p.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), first.Point1, sin1, cos1);

                    adjacentEdge1.MoveEnd(displacement);
                    adjacentEdge1.MoveStartAbs(chain[chain.Count - 2].Point2);
                    adjacentEdge2.MoveStart(displacement);

                    previousPoint = new PointF(x, y);

                    InvalidateRelatedChains(chain, (int)relId2, sin1, cos1);
                }
                else if(relId1 != null)
                {
                    var chain = adjacentEdge2.chain;
                    Segment last = chain[^1];
                    var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
                    PointF oldDirection = new PointF(previousPoint.X - last.Point2.X, previousPoint.Y - last.Point2.Y);

                    movedVertex.Move(displacement);

                    PointF newDirection = new PointF(x - last.Point2.X, y - last.Point2.Y);

                    (float sin1, float cos1) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Polygon? p = context.FindPolygon(chain);
                    p.ApplyParallelRelation(chain.Skip(1).ToList(), last.Point2, sin1, cos1);

                    adjacentEdge2.MoveStart(displacement);
                    adjacentEdge2.MoveEndAbs(chain[1].Point1);
                    adjacentEdge1.MoveEnd(displacement);

                    previousPoint = new PointF(x, y);

                    InvalidateRelatedChains(chain, (int)relId1, sin1, cos1);
                }
                else
                {
                    var displacement = new PointF(x - previousPoint.X, y - previousPoint.Y);
                    movedVertex.Move(displacement);
                    adjacentEdge1.MoveEnd(displacement);
                    adjacentEdge2.MoveStart(displacement);
                    previousPoint = new PointF(x, y);
                }
                
            }
            context.Canvas.Invalidate();
        }

        /*private void InvalidateRelatedChains(float sinTheta, float cosTheta)
        {
            (int? relId1, int? relId2) = movedVertex.relationIds;
            if (relId1 != null)
            {
                AdjustEdgesInRelation(relId1.Value, adjacentEdge1.chain, sinTheta, cosTheta);
            }
            if (relId2 != null)
            {
                AdjustEdgesInRelation(relId2.Value, adjacentEdge2.chain, sinTheta, cosTheta);
            }
        }*/

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta)
        {
            foreach(var relatedChain in context.relations[relationId])
            {
                if(relatedChain != chain)
                {
                    Polygon? p = context.FindPolygon(chain);
                    p.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }
    }
}
