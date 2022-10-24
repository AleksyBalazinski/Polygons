using Polygons.Shapes;

namespace Polygons
{
    internal class Algorithm
    {
        public Action<Graphics, Point, Point> SegmentDrawingAlgorithm { get; set; }
        public Action<Graphics, Point, float> CircleDrawingAlgorithm { get; set; }

        public Action<Graphics, Polygon, Algorithm> PolygonDrawingAlgorithm { get; set; }

        public Algorithm()
        {
            SegmentDrawingAlgorithm = DrawingAlgorithms.LineLibrary;
            CircleDrawingAlgorithm = DrawingAlgorithms.CircleLibrary;
            PolygonDrawingAlgorithm = DrawingAlgorithms.PolygonStandard;
        }

        public void Apply(Graphics g, Segment segment) => SegmentDrawingAlgorithm(g, segment.Point1, segment.Point2);
        public void Apply(Graphics g, Vertex vertex) => CircleDrawingAlgorithm(g, vertex.Center, vertex.Radius);
        public void Apply(Graphics g, Polygon polygon) => PolygonDrawingAlgorithm(g, polygon, this);
    }
}
