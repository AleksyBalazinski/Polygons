using Polygons.Shapes;
using System.Diagnostics;

namespace Polygons.States
{
    internal class DrawState : State
    {
        private Polygon constructedPolygon;
        private Segment drawnSegment;
        private bool definingNewPolygon;
        public DrawState()
        {
            definingNewPolygon = true;
            constructedPolygon = new Polygon();
            drawnSegment = new Segment();
        }

        public override void canvas_MouseDown(object sender, MouseEventArgs e)
        {
        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!definingNewPolygon)
                DrawEdgeUnderConstruction(e.X, e.Y);
        }

        public override void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            DrawPolygonUnderConstruction(e.X, e.Y);
        }

        private void DrawPolygonUnderConstruction(int x, int y)
        {
            if (!definingNewPolygon && constructedPolygon.Vertices.Count >= 3 && Utilities.IsOnVertex(constructedPolygon.Vertices[0], x, y, 10))
            {
                drawnSegment.endpoints.Item2 = constructedPolygon.Vertices[0];
                constructedPolygon.Vertices[0].adjacentEdges.Item1 = drawnSegment;
                constructedPolygon.Edges.Add(drawnSegment);
                Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                context.Polygons.Add(constructedPolygon);
                Debug.WriteLine($"Add polygon {constructedPolygon}");

                for (int i = 0; i < constructedPolygon.Edges.Count; i++)
                {
                    Segment e = constructedPolygon.Edges[i];
                    if (i == 0)
                        e.adjacentEdges.Item1 = constructedPolygon.Edges[constructedPolygon.Edges.Count - 1];
                    else
                        e.adjacentEdges.Item1 = constructedPolygon.Edges[i - 1];

                    if (i == constructedPolygon.Edges.Count - 1)
                        e.adjacentEdges.Item2 = constructedPolygon.Edges[0];
                    else
                        e.adjacentEdges.Item2 = constructedPolygon.Edges[i + 1];
                }

                constructedPolygon = new Polygon();
                definingNewPolygon = true;
            }
            else
            {
                Vertex v = new(x, y);

                Debug.WriteLine("Add vertex " + v.ToString());
                if (!definingNewPolygon)
                {
                    drawnSegment.endpoints.Item2 = v;
                    v.adjacentEdges.Item1 = drawnSegment;
                    constructedPolygon.Edges.Add(drawnSegment);
                    Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                }

                drawnSegment = new Segment();
                drawnSegment.Point1 = drawnSegment.Point2 = new Point(x, y);

                constructedPolygon.Vertices.Add(v);
                v.polygon = constructedPolygon;
                drawnSegment.endpoints.Item1 = v;
                v.adjacentEdges.Item2 = drawnSegment;
                definingNewPolygon = false;
            }

            context.Canvas.Invalidate();
        }

        private void DrawEdgeUnderConstruction(int x, int y)
        {
            drawnSegment.Point2 = new Point(x, y);
            context.Canvas.Invalidate();
        }

        public override void canvas_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics);
            if (!definingNewPolygon)
                drawnSegment.Draw(e.Graphics, drawingAlgorithm);
        }

        public override void DrawScene(Graphics g)
        {
            constructedPolygon.Draw(g, drawingAlgorithm);
            foreach (var polygon in context.Polygons)
                polygon.Draw(g, drawingAlgorithm);
        }
    }
}
