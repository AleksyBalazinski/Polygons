namespace Polygons.States
{
    internal class DeletePolygonState : State
    {
        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            HitTester hitTester = new(context.Polygons);
            Polygon? toBeDeleted = hitTester.GetHitPolygon(e.Location);
            if (toBeDeleted != null)
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
    }
}
