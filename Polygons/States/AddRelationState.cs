using Polygons.Shapes;
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
            HitTester hitTester = new(context.Polygons);
            var edge = hitTester.GetHitEdge(e.Location);
            if (edge != null)
            {
                Polygon polygon = edge.endpoints.Item1.polygon;
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

        public void AddToRelation(Segment edge, Polygon polygon, int relationId)
        {
            Debug.WriteLine($"Add edge {edge} to relation {relationId}");

            var relation = context.relations[relationId];
            if (edge.RelationId == null) // adding an edge that hasn't been in any relation up to this point
            {
                if (relation.Count == 0) // this edge is the first one from this relation
                {
                    List<Segment> chain = new();
                    chain.Add(edge);
                    edge.chain = chain;
                    edge.RelationId = relationId;
                    (Vertex v1, Vertex v2) = edge.endpoints;
                    v1.relationIds.Item1 = relationId;
                    v2.relationIds.Item2 = relationId;
                    (Segment e1, Segment e2) = edge.adjacentEdges;
                    e1.relationIds.Item1 = relationId;
                    e2.relationIds.Item2 = relationId;
                    relation.Add(chain);
                }
                else
                {
                    (Segment e1, Segment e2) = edge.adjacentEdges;
                    if (e1.RelationId != null && e1.RelationId == relationId
                        && e2.RelationId != null && e2.RelationId == relationId)
                    {
                        List<Segment> tempChain = new();
                        tempChain.Add(edge);
                        edge.chain = tempChain;
                        edge.RelationId = relationId;
                        (Vertex v1, Vertex v2) = edge.endpoints;
                        v1.relationIds.Item1 = relationId;
                        v2.relationIds.Item2 = relationId;
                        (Segment e11, Segment e21) = edge.adjacentEdges;
                        e11.relationIds.Item1 = relationId;
                        e21.relationIds.Item2 = relationId;

                        Fixer fixer = new();
                        fixer.Fix(e1.chain![0].endpoints.Item1, e1.chain[0].Point1);
                        foreach (var p in context.Polygons)
                        {
                            if (p != e1.chain[0].endpoints.Item1.polygon)
                            {
                                fixer.FixOffshoot(p);
                            }
                        }

                        e1.chain.Add(tempChain[0]);
                        edge.chain = e1.chain;

                        MergeChains(e1.chain!, e2.chain!, relationId);
                    }
                    else if (e1.RelationId != null && e1.RelationId == relationId)
                    {
                        List<Segment> tempChain = new();
                        tempChain.Add(edge);
                        edge.chain = tempChain;
                        edge.RelationId = relationId;
                        (Vertex v1, Vertex v2) = edge.endpoints;
                        v1.relationIds.Item1 = relationId;
                        v2.relationIds.Item2 = relationId;
                        (Segment e11, Segment e21) = edge.adjacentEdges;
                        e11.relationIds.Item1 = relationId;
                        e21.relationIds.Item2 = relationId;

                        Fixer fixer = new();
                        fixer.Fix(e1.chain![0].endpoints.Item1, e1.chain[0].Point1);
                        foreach (var p in context.Polygons)
                        {
                            if (p != e1.chain[0].endpoints.Item1.polygon)
                            {
                                fixer.FixOffshoot(p);
                            }
                        }

                        e1.chain.Add(tempChain[0]);
                        edge.chain = e1.chain;
                    }
                    else if (e2.RelationId != null && e2.RelationId == relationId)
                    {
                        List<Segment> tempChain = new();
                        tempChain.Add(edge);
                        edge.chain = tempChain;
                        edge.RelationId = relationId;
                        (Vertex v1, Vertex v2) = edge.endpoints;
                        v1.relationIds.Item1 = relationId;
                        v2.relationIds.Item2 = relationId;
                        (Segment e11, Segment e21) = edge.adjacentEdges;
                        e11.relationIds.Item1 = relationId;
                        e21.relationIds.Item2 = relationId;

                        Fixer fixer = new();
                        fixer.Fix(e2.chain![^1].endpoints.Item2, e2.chain[^1].Point2);
                        foreach (var p in context.Polygons)
                        {
                            if (p != e2.chain[^1].endpoints.Item2.polygon)
                            {
                                fixer.FixOffshoot(p);
                            }
                        }

                        e2.chain.Insert(0, tempChain[0]);
                        edge.chain = e2.chain;
                    }
                    else
                    {
                        List<Segment> chain = new();
                        chain.Add(edge);
                        edge.chain = chain;
                        edge.RelationId = relationId;
                        (Vertex v1, Vertex v2) = edge.endpoints;
                        v1.relationIds.Item1 = relationId;
                        v2.relationIds.Item2 = relationId;
                        (Segment e11, Segment e21) = edge.adjacentEdges;
                        e11.relationIds.Item1 = relationId;
                        e21.relationIds.Item2 = relationId;
                        relation.Add(chain);

                        Fixer fixer = new();
                        fixer.Fix(relation[0][0].endpoints.Item1, relation[0][0].Point1);
                        foreach (var p in context.Polygons)
                        {
                            if (p != relation[0][0].endpoints.Item1.polygon)
                            {
                                fixer.FixOffshoot(p);
                            }
                        }
                    }
                }
            }
            else
            {
                var oldRelation = context.relations[edge.RelationId.Value];

                // move all chains from old relation to currently constructed relation
                MergeRelations(relation, oldRelation, edge.RelationId.Value, relationId);

                foreach (var p in context.Polygons)
                {
                    foreach (var e in p.Edges)
                    {
                        (Segment e1, Segment e2) = e.adjacentEdges;
                        if (e.RelationId != null && e1.RelationId != null
                            && e.RelationId == e1.RelationId && e.chain != e1.chain)
                        {
                            MergeChains(e.chain!, e1.chain!, e.RelationId.Value);
                        }
                        else if (e.RelationId != null && e2.RelationId != null
                            && e.RelationId == e2.RelationId && e.chain != e2.chain)
                        {
                            MergeChains(e.chain!, e2.chain!, e.RelationId.Value);
                        }
                    }
                }
            }
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
            foreach (var chain in r2)
            {
                foreach (var e in chain)
                {
                    (Vertex v1, Vertex v2) = e.endpoints;
                    v1.relationIds.Item1 = newRel;
                    v2.relationIds.Item2 = newRel;
                    (Segment e1, Segment e2) = e.adjacentEdges;
                    e1.relationIds.Item1 = newRel;
                    e2.relationIds.Item2 = newRel;
                    e.RelationId = newRel;
                    Fixer fixer = new();
                    fixer.Fix(r1[0][0].endpoints.Item1, r1[0][0].Point1);
                    foreach (var p in context.Polygons)
                    {
                        if (p != r1[0][0].endpoints.Item1.polygon)
                        {
                            fixer.FixOffshoot(p);
                        }
                    }

                    context.Canvas.Invalidate();
                }
            }
            r1.AddRange(r2);
            context.relations.Remove(oldRel);
        }

        private int AddEmptyRelation()
        {
            int freshId = context.relations.Count > 0 ? context.relations.Keys.Max() + 1 : 0;
            context.relations.Add(freshId, new List<List<Segment>>());
            return freshId;
        }
    }
}