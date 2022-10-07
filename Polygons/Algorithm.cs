namespace Polygons
{
    internal class Algorithm
    {
        public Action<Graphics, Point, Point> SegmentDrawingAlgorithm { get; set; }
        public Action<Graphics, Point, int> CircleDrawingAlgorithm { get; set; }

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
                Apply(g, e);
        }
    }
}
