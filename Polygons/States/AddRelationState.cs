using System.Diagnostics;

namespace Polygons.States
{
    internal class AddRelationState : State
    {
        bool definingNewRelation;
        int relationId = 0;
        public AddRelationState()
        {
            definingNewRelation = true;
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var edgeInfo = FindSelectedEdge(e.Location);
            if (edgeInfo != null)
            {
                (Segment edge, Polygon polygon) = edgeInfo.Value;
                if (definingNewRelation)
                {
                    relationId = AddEmptyRelation();
                    definingNewRelation = false;
                }
                AddToRelation(edge, polygon, relationId);
                context.Canvas.Invalidate();
                return;
            }
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private (Segment, Polygon)? FindSelectedEdge(PointF p)
        {
            Segment selectedEdge;
            foreach (var polygon in context.Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");
                        selectedEdge = edge;
                        (Segment edge1, Segment edge2) = polygon.GetAdjacentEdges(edge);
                        (Vertex vertex1, Vertex vertex2) = polygon.GetEndpoints(edge);
                        return (selectedEdge, polygon);
                    }
                }
            }

            return null;
        }

        public void AddToRelation(Segment edge, Polygon polygon, int relationId)
        {
            Debug.WriteLine($"Add edge {edge} to relation {relationId}");

            var relation = context.relations[relationId];
            if (edge.RelationId == null)
            {
                if (relation.Count == 0)
                {
                    List<Segment> chain = new();
                    AddToChainEnd(chain, edge, polygon);
                    relation.Add(chain);
                }
                else
                {
                    polygon.ApplyParallelRelation(edge, relation[0][0]);
                    (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
                    if(e1.RelationId != null && e1.RelationId == relationId
                        && e2.RelationId != null && e2.RelationId == relationId)
                    {
                        AddToChainEnd(e1.chain!, edge, polygon);
                        MergeChains(e1.chain!, e2.chain!, relationId);
                    }
                    else if(e1.RelationId != null && e1.RelationId == relationId)
                    {
                        AddToChainEnd(e1.chain!, edge, polygon);
                    }
                    else if(e2.RelationId != null && e2.RelationId == relationId)
                    {
                        AddToChainStart(e2.chain!, edge, polygon);
                    }
                    else
                    {
                        List<Segment> chain = new();
                        AddToChainEnd(chain, edge, polygon);
                        relation.Add(chain);
                    }
                }
            }
            else
            {
                var oldRelation = context.relations[edge.RelationId.Value];

                // move all chains from old relation to currently constructed relation
                MergeRelations(relation, oldRelation, edge.RelationId.Value, relationId);

                // invalidate each chain, i.e. for each edge check if adjacent
                foreach(var p in context.Polygons)
                {
                    foreach(var e in p.Edges)
                    {
                        (Segment e1, Segment e2) = polygon.GetAdjacentEdges(e);
                        if (e.RelationId != null && e1.RelationId != null
                            && e.RelationId == e1.RelationId && e.chain != e1.chain)
                        {
                            MergeChains(e.chain, e1.chain, e.RelationId.Value);
                        }
                        else if(e.RelationId != null && e2.RelationId != null
                            && e.RelationId == e2.RelationId && e.chain != e2.chain)
                        {
                            MergeChains(e.chain, e2.chain, e.RelationId.Value);
                        }
                    }
                }
            }
        }

        private void AddToChainEnd(List<Segment> chain, Segment edge, Polygon polygon)
        {
            Debug.WriteLine($"Adding {edge} to chain {string.Join(", ", chain)} at end");
            chain.Add(edge);
            edge.chain = chain;
            edge.RelationId = relationId;
            (Vertex v1, Vertex v2) = polygon.GetEndpoints(edge);
            v1.relationIds.Item1 = relationId;
            v2.relationIds.Item2 = relationId;
            (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
            e1.relationIds.Item1 = relationId;
            e2.relationIds.Item2 = relationId;
        }

        private void AddToChainStart(List<Segment> chain, Segment edge, Polygon polygon)
        {
            Debug.WriteLine($"Adding {edge} to chain {string.Join(", ", chain)} at start");
            chain.Insert(0, edge);
            edge.chain = chain;
            edge.RelationId = relationId;
            (Vertex v1, Vertex v2) = polygon.GetEndpoints(edge);
            v1.relationIds.Item1 = relationId;
            v2.relationIds.Item2 = relationId;
            (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
            e1.relationIds.Item1 = relationId;
            e2.relationIds.Item2 = relationId;
        }

        private void MergeChains(List<Segment> c1, List<Segment> c2, int rel)
        {
            foreach (Segment e in c2)
                e.chain = c1;
            c1.AddRange(c2);
            var chains = context.relations[rel];
            chains.Remove(c2);
        }

        private void MergeRelations(List<List<Segment>> r1, List<List<Segment>> r2, int oldRel, int newRel)
        {
            // wont work until chains are fully operational
            foreach(var chain in r2)
            {
                foreach(var e in chain)
                {
                    Polygon? p = context.FindPolygon(e);
                    p!.ApplyParallelRelation(e, r1[0][0]);
                    e.RelationId = newRel;
                    context.Canvas.Invalidate();
                }
            }
            r1.AddRange(r2);
            context.relations.Remove(oldRel);
        }

        private int AddEmptyRelation()
        {
            context.relations.Add(context.relations.Count, new List<List<Segment>>());
            return context.relations.Count - 1;
        }
    }
}