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
                    relationId = context.Relations.AddEmptyRelation();
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

            var relation = context.Relations.relations[relationId];
            if (edge.RelationId == null)
            {
                if (relation.Count == 0)
                {
                    AddToRelationInternal(relation, edge, polygon);
                }
                else
                {
                    polygon.ApplyParallelRelation(edge, relation[^1]);
                    AddToRelationInternal(relation, edge, polygon);
                }
            }
            else
            {
                var currentRelation = context.Relations.relations[edge.RelationId.Value];
                foreach (Segment s in currentRelation)
                {
                    Polygon? p = context.FindPolygon(s);
                    if (p != null)
                        p.ApplyParallelRelation(s, relation[0]);
                    else
                        throw new NullReferenceException("edge not found!");

                    AddToRelationInternal(relation, s, p);
                }
                polygon.ApplyParallelRelation(edge, relation[0]);

                AddToRelationInternal(relation, edge, polygon);
            }
        }

        private void AddToRelationInternal(List<Segment> relation, Segment edge, Polygon polygon)
        {
            relation.Add(edge);
            edge.RelationId = relationId;
            (Vertex v1, Vertex v2) = polygon.GetEndpoints(edge);
            v1.relationIds.Item1 = relationId;
            v2.relationIds.Item2 = relationId;
            (Segment e1, Segment e2) = polygon.GetAdjacentEdges(edge);
            e1.relationIds.Item1 = relationId;
            e2.relationIds.Item2 = relationId;
        }
    }
}