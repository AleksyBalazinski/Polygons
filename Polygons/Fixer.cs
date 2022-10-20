#define NEW
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;


namespace Polygons
{
    using Relations = Dictionary<int, List<List<Segment>>>;
    using Chain = List<Segment>;
    internal class Fixer
    {
        private Relations relations;
        //private HashSet<int> seenRelations = new();
        PictureBox canvas; // debug
        private Segment theUnconstrainedEdge;
        private Dictionary<int, List<Segment>> seenChains = new();
        public Fixer(Relations relations, PictureBox canvas)
        {
            this.relations = relations;
            this.canvas = canvas;
        }
#if OLD
        public void Fix(Vertex movedVertex, Point location)
        {
            (int? relId1, int? relId2) = movedVertex.relationIds;
            (Segment adjacentEdge1, Segment adjacentEdge2) = movedVertex.adjacentEdges;
            
            if (relId1 != null && relId2 != null && relId1 == relId2)
            {
                FixChainInside(adjacentEdge1.chain, location, movedVertex);
            }
            else if(relId2 != null)
            {
                FixChainAtEnd(adjacentEdge1.chain, location);
            }
            else if(relId1 != null)
            {
                FixChainAtStart(adjacentEdge2.chain, location);
            }
            else if(adjacentEdge1.fixedLength)
            {
                adjacentEdge1.Point2 = location;
                adjacentEdge1.endpoints.Item2.MoveAbs(location);
                FixEdgeLength(adjacentEdge1);
            }
            else if(adjacentEdge2.fixedLength)
            {
                adjacentEdge2.Point1 = location;
                adjacentEdge2.endpoints.Item1.MoveAbs(location);
                FixEdgeLength(adjacentEdge2);
                adjacentEdge1.Point2 = adjacentEdge2.Point1;
            }
            else
            {
                MoveFreeVertex(location, movedVertex);
            }
            
        }
#endif

        public void Fix(Vertex movedVertex, Point location)
        {
            // TODO check if at least oe unconstrained edge exists
            FixForward(movedVertex, location);
            FixBackward(movedVertex, location);
        }

        public void FixForward(Vertex movedVertex, Point location)
        {
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            int? relId = adjacentEdge2.RelationId;
            // v starts a chain
            if (relId != null)
            {
                // seeing chain from relation relId for the first time
                if(!seenChains.ContainsKey((int)relId))
                {
                    Chain chain = adjacentEdge2.chain;
                    Point oldDirection = chain[^1].Point1 - chain[^1].Point2;
                    Point newDirection = location - chain[^1].Point2;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Polygon.ApplyParallelRelation(chain.Skip(1).ToList(), chain[^1].Point2, sin, cos);

                    adjacentEdge2.MoveStartAbs(location);
                    if (chain.Count > 1)
                        adjacentEdge2.MoveEndAbs(chain[1].Point1);
                    movedVertex.MoveAbs(location);
                    seenChains.Add((int)relId, chain);
                    canvas.Refresh();
                    FixForward(chain[^1].endpoints.Item2, chain[^1].Point2);
                }
                else // already seen this relId
                {
                    var displacement = location - movedVertex.Center;
                    Chain chain = adjacentEdge2.chain;
                    Polygon.ApplyTranslation(chain, displacement);
                    Chain model = seenChains[(int)relId];

                    Point currentDirection = chain[^1].Point2 - chain[0].Point1;
                    Point modelDirection = model[^1].Point2 - model[0].Point1;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);

                    Polygon.ApplyParallelRelation(chain, chain[0].Point1, sin, cos);
                    canvas.Refresh();
                    FixForward(chain[^1].endpoints.Item2, chain[^1].Point2);
                }
            }
            // v starts a constant length edge
            else if(adjacentEdge2.fixedLength)
            {
                movedVertex.MoveAbs(location);
                adjacentEdge2.Point1 = location;
                Point p1p2 = adjacentEdge2.Point2 - adjacentEdge2.Point1;
                Point versor = p1p2 / Geometry.VectorLength(p1p2);
                adjacentEdge2.Point2 = adjacentEdge2.Point1 + adjacentEdge2.declaredLength * versor;
                canvas.Refresh();
                FixForward(adjacentEdge2.endpoints.Item2, adjacentEdge2.Point2);
            }
            // assume forawrdFirst == true TODO ...
            // v starts unconstrained edge
            else
            {
                movedVertex.MoveAbs(location);
                adjacentEdge2.Point1 = location;
                theUnconstrainedEdge = adjacentEdge2;
                canvas.Refresh();
                return;
            }
            canvas.Refresh();
        }

        public void FixBackward(Vertex movedVertex, Point location)
        {
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            int? relId = adjacentEdge1.RelationId;
            // v ends a chain
            if(relId != null)
            {
                if(!seenChains.ContainsKey((int)relId))
                {
                    Chain chain = adjacentEdge1.chain;
                    Point oldDirection = chain[0].Point2 - chain[0].Point1;
                    Point newDirection = location - chain[0].Point1;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Polygon.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), chain[0].Point1, sin, cos);

                    adjacentEdge1.MoveEndAbs(location);
                    if (chain.Count > 1)
                        adjacentEdge1.MoveStartAbs(chain[^2].Point2);
                    movedVertex.MoveAbs(location);
                    seenChains.Add((int)relId, chain);
                    canvas.Refresh();

                    FixBackward(chain[0].endpoints.Item1, chain[0].Point1);
                }
                else // already seen this relId
                {
                    var displacement = location - movedVertex.Center;
                    Chain chain = adjacentEdge1.chain;
                    Polygon.ApplyTranslation(chain, displacement);
                    Chain model = seenChains[(int)relId];

                    Point currentDirection = chain[0].Point1 - chain[^1].Point2;
                    Point modelDirection = model[0].Point1 - model[^1].Point2;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);

                    Polygon.ApplyParallelRelation(chain, chain[^1].Point2, sin, cos);

                    canvas.Refresh();
                    FixBackward(chain[0].endpoints.Item1, chain[0].Point1);
                }
            }
            // v ends a constant length edge
            else if (adjacentEdge1.fixedLength)
            {
                movedVertex.MoveAbs(location);
                adjacentEdge1.Point2 = location;
                Point p2p1 = adjacentEdge1.Point1 - adjacentEdge1.Point2;
                Point versor = p2p1 / Geometry.VectorLength(p2p1);
                adjacentEdge1.Point1 = adjacentEdge1.Point2 + adjacentEdge1.declaredLength * versor;

                canvas.Refresh();
                FixBackward(adjacentEdge1.endpoints.Item1, adjacentEdge1.Point1);
            }

            // assume forwardFirst == true TODO
            // v ends unconstrained edge -> proceed until unconstrained edge that triggered FixBackward is found
            else if (adjacentEdge1 != theUnconstrainedEdge)
            {
                movedVertex.MoveAbs(location);
                adjacentEdge1.Point2 = location;
                canvas.Refresh();
                FixBackward(adjacentEdge1.endpoints.Item1, adjacentEdge1.Point1);
            }
            else
            {
                canvas.Refresh();
                return;
            }
            canvas.Refresh();
        }


#if OLD
        public void FixChainAtEnd(List<Segment> chain, Point location)
        {
            int relId = (int)chain[0].RelationId;
            Point oldDirection = chain[0].Point2 - chain[0].Point1;
            Point newDirection = location - chain[0].Point1;
            Vertex movedVertex = chain[^1].endpoints.Item2;
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

            Polygon.ApplyParallelRelation(chain.Take(chain.Count - 1).ToList(), chain[0].Point1, sin, cos);

            adjacentEdge1.MoveEndAbs(location);
            if(chain.Count > 1)
                adjacentEdge1.MoveStartAbs(chain[^2].Point2);
            movedVertex.MoveAbs(location);
            InvalidateRelatedChains(chain, relId, sin, cos);
            seenRelations.Add(relId);
            FixLengthsInChain(chain);
            if (adjacentEdge2.RelationId != null && !seenRelations.Contains((int)adjacentEdge2.RelationId))
            {
                FixChainAtStart(adjacentEdge2.chain, chain[^1].Point2);
            }
            else if (adjacentEdge2.fixedLength)
            {
                adjacentEdge2.MoveStartAbs(chain[^1].Point2);
                FixEdgeLength(adjacentEdge2);
            }
            else
            {
                adjacentEdge2.MoveStartAbs(chain[^1].Point2);
            }
        }

        public void FixChainAtStart(List<Segment> chain, Point location)
        {
            int relId = (int)chain[0].RelationId;
            Point oldDirection = chain[^1].Point1 - chain[^1].Point2;
            Point newDirection = location - chain[^1].Point2;
            Vertex movedVertex = chain[0].endpoints.Item1;
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

            Polygon.ApplyParallelRelation(chain.Skip(1).ToList(), chain[^1].Point2, sin, cos);

            adjacentEdge2.MoveStartAbs(location);
            if (chain.Count > 1)
                adjacentEdge2.MoveEndAbs(chain[1].Point1);
            movedVertex.MoveAbs(location);
            InvalidateRelatedChains(chain, relId, sin, cos);
            seenRelations.Add(relId);
            FixLengthsInChain(chain);

            if (adjacentEdge1.RelationId != null && !seenRelations.Contains((int)adjacentEdge1.RelationId))
            {
                FixChainAtEnd(adjacentEdge1.chain, chain[0].Point1);
            }
            else if (adjacentEdge1.fixedLength)
            {
                adjacentEdge1.MoveEndAbs(chain[0].Point1);
                FixEdgeLength2(adjacentEdge1);
            }
            else
            {
                adjacentEdge1.MoveEndAbs(chain[0].Point1);
            }
        }

        public void FixChainInside(List<Segment> chain, Point location, Vertex movedVertex)
        {
            int relId = (int)chain[0].RelationId;
            Segment first = chain[0];
            Segment last = chain[^1];
            Point oldDirection = last.Point2 - first.Point1;
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            movedVertex.MoveAbs(location);

            Point newDirection = location - first.Point1;

            int indx = chain.IndexOf(adjacentEdge1);

            var firstAdjacentEdges = first.adjacentEdges;
            var lastAdjacentEdges = last.adjacentEdges;

            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

            Polygon.ApplyParallelRelation(chain.Take(indx).ToList(), first.Point1, sin, cos);
            Polygon.ApplyParallelRelation(chain.Skip(indx + 2).ToList(), first.Point1, sin, cos);

            adjacentEdge1.MoveEndAbs(location);
            if (indx != 0)
                adjacentEdge1.MoveStartAbs(chain[indx - 1].Point2);


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
            adjacentEdge2.MoveStartAbs(adjacentEdge1.Point2);

            firstAdjacentEdges.Item1.Point2 = first.Point1;

            InvalidateRelatedChains(chain, relId, sin, cos);
            seenRelations.Add(relId);
            FixLengthsInChain(chain);

            if (last.adjacentEdges.Item2.RelationId != null)
            {
                FixChainAtStart(last.adjacentEdges.Item2.chain, chain[^1].Point2);
            }
            else if (last.adjacentEdges.Item2.fixedLength)
            {
                last.adjacentEdges.Item2.MoveStartAbs(chain[^1].Point2);
                FixEdgeLength(last.adjacentEdges.Item2);
            }
            else
            {
                last.adjacentEdges.Item2.MoveStartAbs(chain[^1].Point2);
            }
        }

        public void FixLengthsInChain(List<Segment> chain)
        {
            foreach(var edge in chain)
            {
                if(edge.fixedLength && edge.Length != edge.declaredLength)
                {
                    FixEdgeLength(edge);
                }
            }
        }

        public void FixEdgeLength(Segment edge)
        {
            Point p1p2 = edge.Point2 - edge.Point1;
            Point versor = p1p2 / Geometry.VectorLength(p1p2);
            edge.Point2 = edge.Point1 + edge.declaredLength * versor;
            edge.endpoints.Item2.MoveAbs(edge.Point2);
            if(edge.adjacentEdges.Item2.RelationId == null) // don't touch edge in a relation
                edge.adjacentEdges.Item2.Point1 = edge.Point2;
            else
            {
                FixChainAtStart(edge.adjacentEdges.Item2.chain, edge.Point2);
                return;
            }
            if(edge.adjacentEdges.Item2.fixedLength)
            {
                FixEdgeLength(edge.adjacentEdges.Item2);
            }
        }

        public void FixEdgeLength2(Segment edge)
        {
            Point p2p1 = edge.Point1 - edge.Point2;
            Point versor = p2p1 / Geometry.VectorLength(p2p1);
            edge.Point1 = edge.Point2 + edge.declaredLength * versor;
            edge.endpoints.Item1.MoveAbs(edge.Point1);
            if (edge.adjacentEdges.Item1.RelationId == null) // don't touch edge in a relation
                edge.adjacentEdges.Item1.Point2 = edge.Point1;
            else if(!seenRelations.Contains((int)edge.adjacentEdges.Item1.RelationId))
            {
                FixChainAtEnd(edge.adjacentEdges.Item1.chain, edge.Point1);
                return;
            }
            if (edge.adjacentEdges.Item1.fixedLength)
            {
                FixEdgeLength(edge.adjacentEdges.Item1);
            }
        }

        private void MoveFreeVertex(Point location, Vertex movedVertex)
        {
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            movedVertex.MoveAbs(location);
            adjacentEdge1.MoveEndAbs(location);
            adjacentEdge2.MoveStartAbs(location);
        }

        private void InvalidateRelatedChains(List<Segment> chain, int relationId, float sinTheta, float cosTheta)
        {
            foreach (var relatedChain in relations[relationId])
            {
                if (relatedChain != chain)
                {
                    Polygon.ApplyParallelRelation(relatedChain, relatedChain[0].Point1, sinTheta, cosTheta);
                }
            }
        }
#endif
    }
}