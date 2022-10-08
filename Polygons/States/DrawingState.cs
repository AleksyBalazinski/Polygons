using System.Diagnostics;

namespace Polygons.States
{
    internal class DrawingState : AbstractDrawState
    {
        public DrawingState(Form1 context, Algorithm a, Polygon constructedPolygon, Segment drawnSegment) : base(context, a, constructedPolygon, drawnSegment)
        {
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("DrawEdgeUnderConstruction in drawing");
            DrawEdgeUnderConstruction(e.X, e.Y);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("DrawPolygonUnderConstruction in drawing");
            DrawPolygonUnderConstruction(e.X, e.Y);
        }

        protected override void DrawPolygonUnderConstruction(int x, int y)
        {
            if (constructedPolygon.Vertices.Count >= 3 && Utilities.IsOnVertex(constructedPolygon.Vertices[0], x, y, 10))
            {
                constructedPolygon.Edges.Add(drawnSegment);
                Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                context.Polygons.Add(constructedPolygon);
                Debug.WriteLine($"Add polygon {constructedPolygon}");
                context.TransitionTo(new DrawState(context, drawingAlgorithm));
            }
            else
            {
                Vertex v = new Vertex(x, y);

                Debug.WriteLine("Add vertex " + v.ToString());
                constructedPolygon.Edges.Add(drawnSegment);
                Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");

                drawnSegment = new Segment();
                drawnSegment.Point1 = drawnSegment.Point2 = new Point(x, y);

                constructedPolygon.Vertices.Add(v);
            }

            context.Canvas.Invalidate();
        }
    }
}
