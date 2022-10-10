namespace Polygons
{
    internal class Algorithm
    {
        public Action<Graphics, PointF, PointF> SegmentDrawingAlgorithm { get; set; }
        public Action<Graphics, PointF, float> CircleDrawingAlgorithm { get; set; }

        public Algorithm()
        {
            SegmentDrawingAlgorithm = DrawingAlgorithms.LineLibrary;
            CircleDrawingAlgorithm = DrawingAlgorithms.CircleLibrary;
        }

        public void Apply(Graphics g, Segment segment) => SegmentDrawingAlgorithm(g, segment.Point1, segment.Point2);
        public void Apply(Graphics g, Vertex vertex) => CircleDrawingAlgorithm(g, vertex.Center, vertex.Radius);
        public void Apply(Graphics g, Polygon polygon)
        {
            foreach(var v in polygon.Vertices)
                Apply(g, v);
            foreach(var e in polygon.Edges)
            {
                Apply(g, e);
                if(polygon.FixedLengthEdges.Contains(e))
                {
                    Font font = new Font("Arial", 12);
                    SolidBrush brush = new SolidBrush(Color.Black);

                    g.DrawString(string.Format("{0:0.00}", e.Length), font, brush, new PointF((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2));
                }
                if(e.RelationId != null)
                {
                    Font font = new Font("Arial", 12);
                    SolidBrush brush = new SolidBrush(Color.Black);

                    g.DrawString($"{e.RelationId.Value}", font, brush, new PointF((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2));
                }
            }
        }
    }
}
