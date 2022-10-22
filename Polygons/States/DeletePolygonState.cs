using System.Diagnostics;


namespace Polygons.States
{
    internal class DeletePolygonState : State
    {
        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            Polygon? toBeDeleted = FindPolygonToBeDeleted(e.Location);
            if(toBeDeleted != null)
            {
                foreach (var edge in toBeDeleted.Edges)
                {
                    if (edge.RelationId != null)
                    {
                        context.relations[(int)edge.RelationId].Remove(edge.chain!);
                    }
                }
                context.Polygons.Remove(toBeDeleted);
            }
            context.Canvas.Invalidate();
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private Polygon? FindPolygonToBeDeleted(Point p)
        {
            Polygon? hitPolygon;
            foreach (var polygon in context.Polygons)
            {
                if (polygon.HitTest(p))
                {
                    Debug.WriteLine($"Polygon {polygon} hit");
                    hitPolygon = polygon;

                    return hitPolygon;
                }
            }

            return null;
        }
    }
}
