using System.Diagnostics;

namespace Polygons.States
{
    internal class MovingEdgeState : State
    {
        readonly Segment movedEdge;
        readonly Segment adjacentEdge1;
        readonly Segment adjacentEdge2;
        readonly Vertex endpoint1;
        readonly Vertex endpoint2;
        Point previousPoint;
        public MovingEdgeState(Segment movedEdge, Segment adjacentEdge1, Segment adjacentEdge2, Vertex endpoint1, Vertex endpoint2, Point point)
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
            if(movedEdge.fixedLengths.Item1 || movedEdge.fixedLengths.Item2)
            {
                Debug.WriteLine("Cannot move -- adjacent to an edge of fixed length"); // TODO
                return;
            }
            else
            {
                if (movedEdge.RelationId != null)
                {
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);

                    previousPoint = new Point(x, y);
                    var chain = movedEdge.chain;
                    Polygon? p = context.FindPolygon(chain);
                    foreach (Segment e in chain)
                    {
                        (Vertex v1, Vertex v2) = e.endpoints;
                        if (e == chain[0])
                            v1.Move(displacement);
                        e.Move(displacement);
                        v2.Move(displacement);
                        if (e == chain[0])
                        {
                            (Segment prev, _) = e.adjacentEdges;
                            prev.Point2 = e.Point1;
                        }
                        if (e == chain[^1])
                        {
                            (_, Segment next) = e.adjacentEdges;
                            next.Point1 = e.Point2;
                        }
                    }
                }
                else if (adjacentEdge2.RelationId != null || adjacentEdge1.RelationId != null)
                {
                    if(adjacentEdge2.RelationId != null)
                    {
                        var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                        //if(adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                            previousPoint = new Point(x, y);

                        var chain = adjacentEdge2.chain;
                        Polygon? p = context.FindPolygon(chain);
                        Point oldDirection = new(movedEdge.Point2.X - chain[^1].Point2.X, movedEdge.Point2.Y - chain[^1].Point2.Y);

                        Point endMoved = new(movedEdge.Point2.X + displacement.X, movedEdge.Point2.Y + displacement.Y);
                        Point newDirection = new(endMoved.X - chain[^1].Point2.X, endMoved.Y - chain[^1].Point2.Y);

                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        if (adjacentEdge2.chain.Count > 1)
                        {
                            p.ApplyParallelRelation(chain.Skip(1).ToList(), chain[^1].Point2, sin, cos);
                            chain[0].Point2 = chain[1].Point1;
                        }

                        //if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                        //{
                            movedEdge.Move(displacement);
                        if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                            adjacentEdge1.MoveEnd(displacement);
                            adjacentEdge2.MoveStart(displacement);
                            endpoint1.Move(displacement);
                            endpoint2.Move(displacement);
                        //}
                            
                        //if (adjacentEdge1.RelationId == adjacentEdge2.RelationId)
                        //    InvalidateRelatedChains(chain, (int)adjacentEdge2.RelationId, sin, cos, adjacentEdge1.chain);
                        //else
                            InvalidateRelatedChains(chain, (int)adjacentEdge2.RelationId, sin, cos, null);
                    }
                    if(adjacentEdge1.RelationId != null)
                    {
                        var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                        previousPoint = new Point(x, y);

                        var chain = adjacentEdge1.chain;
                        Polygon? p = context.FindPolygon(chain);
                        Point oldDirection = new(movedEdge.Point1.X - chain[0].Point1.X, movedEdge.Point1.Y - chain[0].Point1.Y);

                        Point startMoved = new(movedEdge.Point1.X + displacement.X, movedEdge.Point1.Y + displacement.Y);
                        Point newDirection = new(startMoved.X - chain[0].Point1.X, startMoved.Y - chain[0].Point1.Y);

                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        if (adjacentEdge1.chain.Count > 1 && adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                        {
                            p.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), chain[0].Point1, sin, cos);
                            chain[^1].Point1 = chain[^2].Point2;
                        }

                        if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                        {
                            movedEdge.Move(displacement);
                            //if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                                adjacentEdge1.MoveEnd(displacement);
                            adjacentEdge2.MoveStart(displacement);
                            endpoint1.Move(displacement);
                            endpoint2.Move(displacement);
                        }
                        
                        
                        /*if (adjacentEdge2.RelationId == adjacentEdge1.RelationId)
                            InvalidateRelatedChains(chain, (int)adjacentEdge1.RelationId, sin, cos, adjacentEdge2.chain);
                        else*/
                        if(adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                            InvalidateRelatedChains(chain, (int)adjacentEdge1.RelationId, sin, cos, null);
                    }
                }
                else
                {
                    var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
                    movedEdge.Move(displacement);
                    adjacentEdge1.MoveEnd(displacement);
                    adjacentEdge2.MoveStart(displacement);
                    endpoint1.Move(displacement);
                    endpoint2.Move(displacement);
                    previousPoint = new Point(x, y);
                }
            }

            context.Canvas.Invalidate();
        }

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta, List<Segment>? skip)
        {
            foreach (var relatedChain in context.relations[relationId])
            {
                if (relatedChain != chain && relatedChain != skip)
                {
                    Polygon? p = context.FindPolygon(relatedChain);
                    p.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }
    }
}