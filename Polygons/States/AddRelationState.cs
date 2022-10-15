using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

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
                    (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
                    if(e1.RelationId != null && e1.RelationId == relationId
                        && e2.RelationId != null && e2.RelationId == relationId)
                    {
                        Polygon? p = context.FindPolygon(edge);
                        PointF oldDirection = new(edge.Point2.X - edge.MidPoint.X, edge.Point2.Y - edge.MidPoint.Y);
                        PointF newDirection = new(relation[0][0].Point2.X - relation[0][0].Point1.X, relation[0][0].Point2.Y - relation[0][0].Point1.Y);
                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        p.ApplyParallelRelation12(edge, edge.MidPoint, sin, cos);

                        PointF newDirRot = Geometry.Rotate(newDirection, 1, 0);
                        PointF castingVersor2 = new(newDirRot.X / Geometry.VectorLength(newDirection), newDirRot.Y / Geometry.VectorLength(newDirection));
                        float length = Geometry.VectorLength(oldDirection) * sin;
                        PointF castingVector2 = new(castingVersor2.X * length, castingVersor2.Y * length); 
                        foreach(var e in e2.chain)
                        {
                            e.Point1 = new PointF(castingVector2.X + e.Point1.X, castingVector2.Y + e.Point1.Y);
                            e.Point2 = new PointF(castingVector2.X + e.Point2.X, castingVector2.Y + e.Point2.Y);
                            (Vertex v1, Vertex v2) = p.GetEndpoints(e);
                            v2.Center = new PointF(castingVector2.X + v2.Center.X, castingVector2.Y + v2.Center.Y);
                        }
                        PointF castingVector1 = new(-castingVector2.X, -castingVector2.Y);
                        foreach (var e in e1.chain)
                        {
                            e.Point1 = new PointF(castingVector1.X + e.Point1.X, castingVector1.Y + e.Point1.Y);
                            e.Point2 = new PointF(castingVector1.X + e.Point2.X, castingVector1.Y + e.Point2.Y);
                            (Vertex v1, Vertex v2) = p.GetEndpoints(e);
                            v1.Center = new PointF(castingVector1.X + v1.Center.X, castingVector1.Y + v1.Center.Y);
                        }
                        // fix ends of both chains
                        Segment last = e2.chain[^1];
                        (_, Segment sLast) = p.GetAdjacentEdges(last);
                        sLast.Point1 = last.Point2;

                        Segment first = e1.chain[0];
                        (Segment sFirst, _) = p.GetAdjacentEdges(first);
                        sFirst.Point2 = first.Point1;

                        e2.Point1 = edge.Point2;
                        e1.Point2 = edge.Point1;

                        AddToChainEnd(e1.chain!, edge, polygon);
                        MergeChains(e1.chain!, e2.chain!, relationId);
                    }
                    else if(e1.RelationId != null && e1.RelationId == relationId)
                    {
                        Polygon? p = context.FindPolygon(edge);
                        PointF oldDirection = new(edge.Point2.X - edge.Point1.X, edge.Point2.Y - edge.Point1.Y);
                        PointF newDirection = new(relation[0][0].Point2.X - relation[0][0].Point1.X, relation[0][0].Point2.Y - relation[0][0].Point1.Y);
                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        if (cos < 0)
                        {
                            newDirection = new(-newDirection.X, -newDirection.Y);
                            (sin, cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        }
                        p.ApplyParallelRelation(edge, edge.Point1, sin, cos);
                        e2.Point1 = edge.Point2;

                        AddToChainEnd(e1.chain!, edge, polygon);
                    }
                    else if(e2.RelationId != null && e2.RelationId == relationId)
                    {
                        Polygon? p = context.FindPolygon(edge);
                        PointF oldDirection = new(edge.Point1.X - edge.Point2.X, edge.Point1.Y - edge.Point2.Y);
                        PointF newDirection = new(relation[0][0].Point1.X - relation[0][0].Point2.X, relation[0][0].Point1.Y - relation[0][0].Point2.Y);
                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        if (cos < 0)
                        {
                            newDirection = new(-newDirection.X, -newDirection.Y);
                            (sin, cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        }
                        p.ApplyParallelRelation1(edge, edge.Point2, sin, cos);
                        e1.Point2 = edge.Point1;

                        AddToChainStart(e2.chain!, edge, polygon);
                    }
                    else
                    {
                        // TODO lacks some decision making
                        Polygon? p = context.FindPolygon(edge);
                        PointF oldDirection = new(edge.Point2.X - edge.Point1.X, edge.Point2.Y - edge.Point1.Y);
                        PointF newDirection = new(relation[0][0].Point2.X - relation[0][0].Point1.X, relation[0][0].Point2.Y - relation[0][0].Point1.Y);
                        (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        if(cos < 0)
                        {
                            newDirection = new(-newDirection.X, -newDirection.Y);
                            (sin, cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                        }
                        p.ApplyParallelRelation(edge, edge.Point1, sin, cos);
                        e2.Point1 = edge.Point2;

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
                        (Segment e1, Segment e2) = p.GetAdjacentEdges(e);
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
            foreach(var chain in r2)
            {
                foreach(var e in chain)
                {
                    Polygon? p = context.FindPolygon(e);
                    PointF oldDirection = new(e.Point2.X - e.Point1.X, e.Point2.Y - e.Point1.Y);
                    PointF newDirection = new(r1[0][0].Point2.X - r1[0][0].Point1.X, r1[0][0].Point2.Y - r1[0][0].Point1.Y);
                    (float sin, float cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                    if (cos < 0)
                    {
                        newDirection = new(-newDirection.X, -newDirection.Y);
                        (sin, cos) = Geometry.AngleBetweenVectors(oldDirection, newDirection);
                    }
                    p!.ApplyParallelRelation(e, e.Point1, sin, cos);
                    (Segment s1, Segment s2) = p.GetAdjacentEdges(e);
                    s1.Point2 = e.Point1;
                    s2.Point1 = e.Point2;
                    e.RelationId = newRel;
                    (Vertex v1, Vertex v2) = p.GetEndpoints(e);
                    v1.relationIds.Item1 = newRel;
                    v2.relationIds.Item2 = newRel;
                    (Segment e1, Segment e2) = p.GetAdjacentEdges(e);
                    e1.relationIds.Item1 = newRel;
                    e2.relationIds.Item2 = newRel;
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