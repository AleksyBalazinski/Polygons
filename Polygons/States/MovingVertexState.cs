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
            DrawAfterVertexMoved(e.Location);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            context.TransitionTo(new MoveState());
        }

        private void DrawAfterVertexMoved(Point location)
        {
            (int? relId1, int? relId2) = movedVertex.relationIds;
            if(relId1 != null && relId1 == relId2 && (movedVertex.fixedLenghts.Item2 || movedVertex.fixedLenghts.Item1))
            {
                var chain = adjacentEdge1.chain;
                Point oldDirection = adjacentEdge1.Point2 - chain[0].Point1;
                Point newDirection = location - chain[0].Point1;
                (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                Segment virtualEdge = new(chain[0].Point1, adjacentEdge1.Point2);
                virtualEdge.endpoints.Item1 = new Vertex(chain[0].Point1);
                virtualEdge.endpoints.Item2 = new Vertex(adjacentEdge1.Point2);
                Polygon.ApplyParallelRelation(virtualEdge, virtualEdge.Point1, sin, cos);
                location = new Point(virtualEdge.Point2);
                previousPoint = adjacentEdge1.Point2;
            }
            else if (movedVertex.fixedLenghts.Item2)
            {
                Point oldDirection = adjacentEdge1.Point2 - adjacentEdge1.Point1;
                Point newDirection = location - adjacentEdge1.Point1;
                var chain = adjacentEdge1.chain;
                if (relId2 != null)
                {
                    oldDirection = adjacentEdge1.Point2 - chain[0].Point1;
                    newDirection = location - chain[0].Point1;
                }
                (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                Segment virtualEdge = new(adjacentEdge1.Point1, adjacentEdge1.Point2);
                virtualEdge.endpoints.Item1 = new Vertex(adjacentEdge1.Point1);
                virtualEdge.endpoints.Item2 = new Vertex(adjacentEdge1.Point2);
                if (relId2 != null)
                {
                    virtualEdge = new(chain[0].Point1, adjacentEdge1.Point2);
                    virtualEdge.endpoints.Item1 = new Vertex(chain[0].Point1);
                    virtualEdge.endpoints.Item2 = new Vertex(adjacentEdge1.Point2);
                }

                Polygon.ApplyParallelRelation(virtualEdge, virtualEdge.Point1, sin, cos);
                location = new Point(virtualEdge.Point2);
                previousPoint = adjacentEdge1.Point2;
            }
            else if (movedVertex.fixedLenghts.Item1)
            {
                Point oldDirection = adjacentEdge2.Point1 - adjacentEdge2.Point2;
                Point newDirection = location - adjacentEdge2.Point2;
                var chain = adjacentEdge2.chain;
                if (relId1 != null)
                {
                    oldDirection = adjacentEdge2.Point1 - chain[^1].Point2;
                    newDirection = location - chain[^1].Point2;
                }
                (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                Segment virtualEdge = new(adjacentEdge2.Point1, adjacentEdge2.Point2);
                virtualEdge.endpoints.Item1 = new Vertex(adjacentEdge2.Point1);
                virtualEdge.endpoints.Item2 = new Vertex(adjacentEdge2.Point2);
                if (relId1 != null)
                {
                    virtualEdge = new(adjacentEdge2.Point1, chain[^1].Point2);
                    virtualEdge.endpoints.Item2 = new Vertex(chain[0].Point1);
                    virtualEdge.endpoints.Item1 = new Vertex(adjacentEdge1.Point2);
                }

                Polygon.ApplyParallelRelation1(virtualEdge, virtualEdge.Point2, sin, cos);
                location = new Point(virtualEdge.Point1);
                previousPoint = adjacentEdge2.Point1;
            }

            if (relId1 != null && relId2 != null && relId1 == relId2)
            {
                MoveVertexInsideChain(location, (int)relId1);
                previousPoint = new Point(location);
            }
            else if(relId2 != null)
            {
                MoveVertexAtChainEnd(location, (int)relId2);
                previousPoint = new Point(location);
            }
            else if(relId1 != null)
            {
                MoveVertexAtChainStart(location, (int)relId1);
                previousPoint = new Point(location);
            }
            else
            {
                MoveFreeVertex(location);
                previousPoint = new Point(location);
            }

            context.Canvas.Invalidate();
        }

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta)
        {
            foreach(var relatedChain in context.relations[relationId])
            {
                if(relatedChain != chain)
                {
                    Polygon.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }

        private void MoveVertexInsideChain(Point location, int relId)
        {
            var chain = adjacentEdge1.chain;
            Segment first = chain[0];
            Segment last = chain[^1];
            var displacement = location - previousPoint;
            Point oldDirection = previousPoint - first.Point1;

            movedVertex.Move(displacement);

            Point newDirection = location - first.Point1;

            int indx = chain.IndexOf(adjacentEdge1);

            var firstAdjacentEdges = first.adjacentEdges;
            var lastAdjacentEdges = last.adjacentEdges;

            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

            Polygon.ApplyParallelRelation(chain.Take(indx).ToList(), first.Point1, sin, cos);
            Polygon.ApplyParallelRelation(chain.Skip(indx + 2).ToList(), first.Point1, sin, cos);

            adjacentEdge1.MoveEnd(displacement);
            if (indx != 0)
                adjacentEdge1.MoveStartAbs(chain[indx - 1].Point2);

            adjacentEdge2.MoveStart(displacement);
            if (indx <= chain.Count - 3)
                adjacentEdge2.MoveEndAbs(chain[indx + 2].Point1);
            else
            {
                Point v = adjacentEdge2.Point2 - first.Point1;
                var vRot = Geometry.Rotate(v, sin, cos);
                adjacentEdge2.Point2 = vRot + first.Point1;
                (_, Vertex v2) = adjacentEdge2.endpoints;
                v2.MoveAbs(adjacentEdge2.Point2);
            }

            if (firstAdjacentEdges.Item1.chain != chain) // always satisfied???
            {
                firstAdjacentEdges.Item1.Point2 = first.Point1;
            }
            if (lastAdjacentEdges.Item2.chain != chain)
            {
                lastAdjacentEdges.Item2.Point1 = last.Point2;
            }

            InvalidateRelatedChains(chain, relId, sin, cos);
        }

        private void MoveVertexAtChainStart(Point location, int relId)
        {
            var chain = adjacentEdge2.chain;
            var displacement = location - previousPoint;
            Segment last = chain[^1];
            Point oldDirection = previousPoint - last.Point2;
            Point newDirection = location - last.Point2;
            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
            if (chain.Count == 1)
            {
                movedVertex.Move(displacement);
                adjacentEdge1.MoveEnd(displacement);
                adjacentEdge2.MoveStart(displacement);
                previousPoint = new Point(location);
                InvalidateRelatedChains(chain, relId, sin, cos);
                context.Canvas.Invalidate();
                return;
            }
            movedVertex.Move(displacement);

            Polygon.ApplyParallelRelation(chain.Skip(1).ToList(), last.Point2, sin, cos);

            adjacentEdge2.MoveStart(displacement);
            adjacentEdge2.MoveEndAbs(chain[1].Point1);
            adjacentEdge1.MoveEnd(displacement);

            InvalidateRelatedChains(chain, relId, sin, cos);
        }

        private void MoveVertexAtChainEnd(Point location, int relId)
        {
            var chain = adjacentEdge1.chain;
            var displacement = location - previousPoint;
            Segment first = chain[0];
            Point oldDirection = previousPoint - first.Point1;
            Point newDirection = location - first.Point1;
            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
            if (chain.Count == 1)
            {
                movedVertex.Move(displacement);
                adjacentEdge1.MoveEnd(displacement);
                adjacentEdge2.MoveStart(displacement);
                previousPoint = new Point(location);
                InvalidateRelatedChains(chain, relId, sin, cos);
                context.Canvas.Invalidate();
                return;
            }
            movedVertex.Move(displacement);

            Polygon.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), first.Point1, sin, cos);

            adjacentEdge1.MoveEnd(displacement);
            adjacentEdge1.MoveStartAbs(chain[chain.Count - 2].Point2);
            adjacentEdge2.MoveStart(displacement);

            InvalidateRelatedChains(chain, relId, sin, cos);
        }

        private void MoveFreeVertex(Point location)
        {
            var displacement = location - previousPoint;
            movedVertex.Move(displacement);
            adjacentEdge1.MoveEnd(displacement);
            adjacentEdge2.MoveStart(displacement);
        }
    }
}
