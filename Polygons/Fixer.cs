namespace Polygons
{
    using Chain = List<Segment>;
    internal class Fixer
    {
        private Vertex startVertex;
        private Dictionary<int, Chain> seenChains = new();

        public void Fix(Vertex movedVertex, Point location)
        {
            if (!UnconstrainedEdgeExists(movedVertex.polygon))
            {
                var displacement = location - movedVertex.Center;
                movedVertex.polygon.Move(displacement);
                return;
            }
            if (movedVertex.relationIds.Item1 != null && movedVertex.relationIds.Item2 != null
                && movedVertex.relationIds.Item1 == movedVertex.relationIds.Item2)
            {
                Vertex v = FixChainInside(movedVertex.adjacentEdges.Item1.chain, movedVertex, location);
                FixForward(v, v.Center, true);
                FixBackward(v, v.Center, true);
            }
            else
            {
                FixForward(movedVertex, location, true);
                FixBackward(movedVertex, location, true);
            }
        }

        public void Fix(Segment movedEdge, Point displacement)
        {
            if (!UnconstrainedEdgeExists(movedEdge.endpoints.Item1.polygon))
            {
                movedEdge.endpoints.Item1.polygon.Move(displacement);
                return;
            }
            if (movedEdge.RelationId != null)
            {
                Chains.TranslateForeward(movedEdge.chain, displacement);
                FixForward(movedEdge.chain[^1].endpoints.Item2, movedEdge.chain[^1].Point2, true);
                FixBackward(movedEdge.chain[0].endpoints.Item1, movedEdge.chain[0].Point1, true);
            }
            else
            {
                movedEdge.Point1 += displacement;
                movedEdge.Point2 += displacement;
                FixForward(movedEdge.endpoints.Item2, movedEdge.Point2, true);
                FixBackward(movedEdge.endpoints.Item1, movedEdge.Point1, true);
            }
        }

        public void FixOffshoot(Polygon affectedPolygon)
        {
            Vertex movedVertex = affectedPolygon.Vertices[0];
            Point location = movedVertex.Center;
            if (!UnconstrainedEdgeExists(affectedPolygon))
            {
                var displacement = location - movedVertex.Center;
                affectedPolygon.Move(displacement);
                return;
            }

            if (movedVertex.relationIds.Item1 != null && movedVertex.relationIds.Item2 != null
                && movedVertex.relationIds.Item1 == movedVertex.relationIds.Item2)
            {
                Vertex v = FixChainInside(movedVertex.adjacentEdges.Item1.chain, movedVertex, location);
                FixForward(v, v.Center, false);
                FixBackward(v, v.Center, false);
            }
            else
            {
                FixForward(movedVertex, location, false);
                FixBackward(movedVertex, location, false);
            }
        }

        private void FixForward(Vertex movedVertex, Point location, bool isUserDefined)
        {
            Segment adjacentEdge2 = movedVertex.adjacentEdges.Item2;
            int? relId = adjacentEdge2.RelationId;
            // v starts a chain
            if (relId != null)
            {
                // seeing chain from relation relId for the first time
                if (!seenChains.ContainsKey((int)relId) || isUserDefined)
                {
                    Chain chain = adjacentEdge2.chain;
                    Point oldDirection = chain[^1].Point1 - chain[^1].Point2;
                    Point newDirection = location - chain[^1].Point2;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Chains.RotateChainForeward(chain.Skip(1).ToList(), chain[^1].Point2, sin, cos);

                    adjacentEdge2.MoveStartAbs(location);
                    if (chain.Count > 1)
                        adjacentEdge2.MoveEndAbs(chain[1].Point1);
                    movedVertex.MoveAbs(location);
                    if (!seenChains.TryAdd((int)relId, chain))
                    {
                        seenChains[(int)relId] = chain;
                    }

                    FixLengthsInChain(chain);
                    FixForward(chain[^1].endpoints.Item2, chain[^1].Point2, false);
                }
                else // already seen this relId
                {
                    var displacement = location - movedVertex.Center;
                    Chain chain = adjacentEdge2.chain;
                    Chains.TranslateForeward(chain, displacement);
                    Chain model = seenChains[(int)relId];

                    Point currentDirection = chain[^1].Point2 - chain[0].Point1;
                    Point modelDirection = model[^1].Point2 - model[0].Point1;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);
                    if (cos < 0)
                    {
                        modelDirection = -modelDirection;
                        (sin, cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);
                    }

                    Chains.RotateChainForeward(chain, chain[0].Point1, sin, cos);
                    movedVertex.MoveAbs(location);
                    adjacentEdge2.Point1 = location;

                    FixForward(chain[^1].endpoints.Item2, chain[^1].Point2, false);
                }
                // TODO invalidate other polygons
            }
            // v starts a constant length edge
            else if (adjacentEdge2.fixedLength)
            {
                movedVertex.MoveAbs(location);
                adjacentEdge2.Point1 = location;
                Point p1p2 = adjacentEdge2.Point2 - adjacentEdge2.Point1;
                Point versor = p1p2 / Geometry.VectorLength(p1p2);
                adjacentEdge2.Point2 = adjacentEdge2.Point1 + adjacentEdge2.declaredLength * versor;

                FixForward(adjacentEdge2.endpoints.Item2, adjacentEdge2.Point2, false);
            }
            // v starts unconstrained edge
            else
            {
                movedVertex.MoveAbs(location);
                adjacentEdge2.Point1 = location;
                startVertex = adjacentEdge2.endpoints.Item1;
            }
        }

        private void FixBackward(Vertex movedVertex, Point location, bool isUserDefined)
        {
            Segment adjacentEdge1 = movedVertex.adjacentEdges.Item1;
            int? relId = adjacentEdge1.RelationId;
            // v ends a chain
            if (relId != null)
            {
                if (!seenChains.ContainsKey((int)relId) || isUserDefined)
                {
                    Chain chain = adjacentEdge1.chain;
                    Point oldDirection = chain[0].Point2 - chain[0].Point1;
                    Point newDirection = location - chain[0].Point1;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

                    Chains.RotateChainBackward(chain.Take(chain.Count - 1).ToList(), chain[0].Point1, sin, cos);

                    adjacentEdge1.MoveEndAbs(location);
                    if (chain.Count > 1)
                        adjacentEdge1.MoveStartAbs(chain[^2].Point2);
                    movedVertex.MoveAbs(location);
                    if (!seenChains.TryAdd((int)relId, chain))
                    {
                        seenChains[(int)relId] = chain;
                    }

                    FixLengthsInChain(chain);

                    if (movedVertex == startVertex && !isUserDefined)
                    {
                        return;
                    }
                    FixBackward(chain[0].endpoints.Item1, chain[0].Point1, false);
                }
                else // already seen this relId
                {
                    if (movedVertex == startVertex && !isUserDefined)
                    {
                        var displacement1 = location - movedVertex.Center;
                        Chain chain1 = adjacentEdge1.chain;
                        Chains.TranslateBackward(chain1, displacement1);
                        Chain model1 = seenChains[(int)relId];

                        Point currentDirection1 = chain1[^1].Point2 - chain1[0].Point1;
                        Point modelDirection1 = model1[^1].Point2 - model1[0].Point1;

                        (float sin1, float cos1) = Geometry.AngleBetweenVectors(currentDirection1, modelDirection1);
                        if (cos1 < 0)
                        {
                            modelDirection1 = -modelDirection1;
                            (sin1, cos1) = Geometry.AngleBetweenVectors(currentDirection1, modelDirection1);
                        }

                        Chains.RotateChainBackward(chain1, chain1[0].Point1, sin1, cos1);

                        movedVertex.MoveAbs(chain1[^1].Point2);
                        movedVertex.adjacentEdges.Item2.Point1 = chain1[^1].Point2;
                        return;
                    }
                    var displacement = location - movedVertex.Center;
                    Chain chain = adjacentEdge1.chain;
                    Chains.TranslateBackward(chain, displacement);
                    Chain model = seenChains[(int)relId];

                    Point currentDirection = chain[0].Point1 - chain[^1].Point2;
                    Point modelDirection = model[0].Point1 - model[^1].Point2;

                    (float sin, float cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);
                    if (cos < 0)
                    {
                        modelDirection = -modelDirection;
                        (sin, cos) = Geometry.AngleBetweenVectors(currentDirection, modelDirection);
                    }

                    Chains.RotateChainBackward(chain, chain[^1].Point2, sin, cos);

                    FixBackward(chain[0].endpoints.Item1, chain[0].Point1, false);

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

                if (movedVertex == startVertex && !isUserDefined)
                {
                    return;
                }
                FixBackward(adjacentEdge1.endpoints.Item1, adjacentEdge1.Point1, false);
            }

            // v ends unconstrained edge -> proceed until unconstrained edge that triggered FixBackward is found
            else if (movedVertex == startVertex && !isUserDefined)
            {
                return;
            }
            else
            {
                movedVertex.MoveAbs(location);
                adjacentEdge1.Point2 = location;

                FixBackward(adjacentEdge1.endpoints.Item1, adjacentEdge1.Point1, false);

            }
        }

        private Vertex FixChainInside(Chain chain, Vertex movedVertex, Point location)
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

            (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);

            Chains.RotateChain(chain.Take(indx).ToList(), first.Point1, sin, cos);
            Chains.RotateChain(chain.Skip(indx + 2).ToList(), first.Point1, sin, cos);

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

            seenChains.Add(relId, chain);

            FixLengthsInChain(chain);

            return chain[^1].endpoints.Item2;
        }

        private void FixLengthsInChain(Chain chain)
        {
            foreach (var edge in chain)
            {
                if (edge.fixedLength && edge.Length != edge.declaredLength)
                {
                    Point p1p2 = edge.Point2 - edge.Point1;
                    Point versor = p1p2 / Geometry.VectorLength(p1p2);
                    edge.Point2 = edge.Point1 + edge.declaredLength * versor;
                    edge.endpoints.Item2.MoveAbs(edge.Point2);
                    edge.adjacentEdges.Item2.Point1 = edge.Point2;
                }
            }
        }

        private bool UnconstrainedEdgeExists(Polygon polygon)
        {
            foreach (var e in polygon.Edges)
            {
                if (!e.fixedLength && e.RelationId == null)
                    return true;
            }
            return false;
        }
    }
}