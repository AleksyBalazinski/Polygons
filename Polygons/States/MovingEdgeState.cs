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
            DrawAfterEdgeMoved(e.Location);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterEdgeMoved(Point location)
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
                    MoveEdgeInsideChain(location);
                    previousPoint = new Point(location);
                }
                else if (adjacentEdge2.RelationId != null || adjacentEdge1.RelationId != null) // BUG doesnt work for rel1->movedEdge->rel2
                {
                    if(adjacentEdge2.RelationId != null)
                    {
                        MoveEdgeAtChainStart(location);
                        previousPoint = new Point(location);
                    }
                    if(adjacentEdge1.RelationId != null)
                    {
                        MoveEdgeAtChainEnd(location);
                        previousPoint = new Point(location);
                    }
                }
                else
                {
                    MoveFreeEdge(location);
                    previousPoint = new Point(location);
                }
            }

            context.Canvas.Invalidate();
        }

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta)
        {
            foreach (var relatedChain in context.relations[relationId])
            {
                if (relatedChain != chain)
                {
                    Polygon.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }

        private void MoveEdgeInsideChain(Point location)
        {
            var displacement = location - previousPoint;
            var chain = movedEdge.chain;
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

        private void MoveFreeEdge(Point location)
        {
            var displacement = location - previousPoint;
            movedEdge.Move(displacement);
            adjacentEdge1.MoveEnd(displacement);
            adjacentEdge2.MoveStart(displacement);
            endpoint1.Move(displacement);
            endpoint2.Move(displacement);
        }

        private void MoveEdgeAtChainStart(Point location)
        {
            var displacement = location - previousPoint;

            var chain = adjacentEdge2.chain;
            Point oldDirection = movedEdge.Point2 - chain[^1].Point2;
            Point endMoved = movedEdge.Point2 + displacement;
            Point newDirection = endMoved - chain[^1].Point2;

            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
            if (adjacentEdge2.chain.Count > 1)
            {
                Polygon.ApplyParallelRelation(chain.Skip(1).ToList(), chain[^1].Point2, sin, cos);
                chain[0].Point2 = chain[1].Point1;
            }

            movedEdge.Move(displacement);
            if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                adjacentEdge1.MoveEnd(displacement);
            adjacentEdge2.MoveStart(displacement);
            endpoint1.Move(displacement);
            endpoint2.Move(displacement);
            InvalidateRelatedChains(chain, (int)adjacentEdge2.RelationId, sin, cos);
        }

        private void MoveEdgeAtChainEnd(Point location)
        {
            var displacement = location - previousPoint;

            var chain = adjacentEdge1.chain;
            Point oldDirection = movedEdge.Point1 - chain[0].Point1;
            Point startMoved = movedEdge.Point1 + displacement;
            Point newDirection = startMoved - chain[0].Point1;

            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
            if (adjacentEdge1.chain.Count > 1 && adjacentEdge1.RelationId != adjacentEdge2.RelationId)
            {
                Polygon.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), chain[0].Point1, sin, cos);
                chain[^1].Point1 = chain[^2].Point2;
            }

            if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
            {
                movedEdge.Move(displacement);
                adjacentEdge1.MoveEnd(displacement);
                adjacentEdge2.MoveStart(displacement);
                endpoint1.Move(displacement);
                endpoint2.Move(displacement);
            }

            if (adjacentEdge1.RelationId != adjacentEdge2.RelationId)
                InvalidateRelatedChains(chain, (int)adjacentEdge1.RelationId, sin, cos);
        }
    }
}