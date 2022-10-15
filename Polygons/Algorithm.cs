namespace Polygons
{
    internal class Algorithm
    {
        public Action<Graphics, Point, Point> SegmentDrawingAlgorithm { get; set; }
        public Action<Graphics, Point, float> CircleDrawingAlgorithm { get; set; }

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
            {
                Apply(g, v);
                Font font = new Font("Arial", 10);
                SolidBrush brush = new SolidBrush(Color.Black);
                g.DrawString($"({v.Center.X},{v.Center.Y})", font, brush, new Point(v.Center.X + 5, v.Center.Y + 5));
            }
                
            foreach(var e in polygon.Edges)
            {
                Apply(g, e);
                Font font = new Font("Arial", 12);
                SolidBrush brush = new SolidBrush(Color.Black);
                if (e.fixedLength)
                    brush = new SolidBrush(Color.DarkGray);

                g.DrawString(string.Format("{0:0.00}", e.Length), font, brush, new Point((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2));
                if(e.RelationId != null)
                {
                    font = new Font("Arial", 12);
                    brush = new SolidBrush(Color.Orange);

                    g.DrawString($"{e.RelationId.Value}", font, brush, new Point((e.Point1.X + e.Point2.X) / 2 - 20, (e.Point1.Y + e.Point2.Y) / 2 - 20));
                }
            }
        }
    }
}
