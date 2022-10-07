using System.Diagnostics;

namespace Polygons
{
    public partial class Form1 : Form
    {
        private readonly Bitmap drawArea;
        private const int zoomFactor = 1;
        Mode mode;

        private List<Polygon> polygons;
        private Polygon constructedPolygon;
        private Polygon movedPolygon;
        private Vertex movedVertex;
        private Segment movedEdge;
        private Segment segment1;
        private Segment segment2;
        private Vertex vertex1; // vertices moved when an edge is moved
        private Vertex vertex2;
        private Segment? drawnSegment;

        Point previousPoint = Point.Empty;
        bool isPolygonMoved = false;
        bool isVertexMoved = false;
        bool isEdgeMoved = false;
        readonly Algorithm drawingAlgorithm;
 
        public Form1()
        {
            InitializeComponent();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }

            mode = Mode.Move;
            polygons = new List<Polygon>();
            constructedPolygon = new Polygon();
            movedPolygon = new Polygon();
            movedVertex = new Vertex(0, 0); // TODO change

            drawingAlgorithm = new Algorithm();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && e.Button == MouseButtons.Left)
            {
                CreatePolygon(e.X, e.Y);
            }

            if(mode == Mode.Move)
            {
                isPolygonMoved = false;
                isVertexMoved = false;
                isEdgeMoved = false;
            }
        }

        private void CreatePolygon(int x, int y)
        {
            if (drawnSegment != null && constructedPolygon.Vertices.Count >= 3 && IsOnVertex(constructedPolygon.Vertices[0], x, y, 10))
            {
                constructedPolygon.Edges.Add(drawnSegment);
                Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                polygons.Add(constructedPolygon);
                Debug.WriteLine($"Add polygon {constructedPolygon}");
                constructedPolygon = new Polygon();
                drawnSegment = null;
            }
            else
            {
                Vertex v = new Vertex(x, y);

                Debug.WriteLine("Add vertex " + v.ToString());
                if (drawnSegment != null)
                {
                    constructedPolygon.Edges.Add(drawnSegment);
                    Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                }

                drawnSegment = new Segment();
                drawnSegment.Point1 = drawnSegment.Point2 = new Point(x, y);

                constructedPolygon?.Vertices.Add(v);
            }

            canvas.Invalidate();
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            mode = Mode.Draw;
        }

        private bool IsOnVertex(Vertex vertex, int x, int y, int d)
        {
            return x >= vertex.Center.X - d && x <= vertex.Center.X + d && y >= vertex.Center.Y - d && y <= vertex.Center.Y + d;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && drawnSegment != null && constructedPolygon.Vertices.Count != 0)
            {
                DrawEdgeUnderConstruction(e.X, e.Y);
            }

            if(mode == Mode.Move && isPolygonMoved == true)
            {
                DrafAfterPolygonMoved(e.X, e.Y);
            }

            if(mode == Mode.Move && isVertexMoved == true)
            {
                DrawAfterVertexMoved(e.X, e.Y);
            }

            if(mode == Mode.Move && isEdgeMoved == true)
            {
                DrawAfterEdgeMoved(e.X, e.Y);
            }
        }

        private void DrawEdgeUnderConstruction(int x, int y)
        {
            if(drawnSegment != null)
            {
                drawnSegment.Point2 = new Point(x, y);
                canvas.Invalidate();
            }
        }

        private void DrafAfterPolygonMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedPolygon.Move(displacement);
            previousPoint = new Point(x, y);
            canvas.Invalidate();
        }

        private void DrawAfterVertexMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedVertex.Move(displacement);
            segment1.MoveStart(displacement);
            segment2.MoveEnd(displacement);
            previousPoint = new Point(x, y);
            canvas.Invalidate();
        }

        private void DrawAfterEdgeMoved(int x, int y)
        {
            var displacement = new Point(x - previousPoint.X, y - previousPoint.Y);
            movedEdge.Move(displacement);
            segment1.MoveEnd(displacement);
            segment2.MoveStart(displacement);
            vertex1.Move(displacement);
            vertex2.Move(displacement);
            previousPoint = new Point(x, y);
            canvas.Invalidate();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics);
            if(drawnSegment != null)
            {
                drawnSegment.Draw(e.Graphics, drawingAlgorithm);
            }
        }

        private void DrawScene(Graphics g)
        {
            constructedPolygon.Draw(g, drawingAlgorithm);
            foreach(var polygon in polygons)
                polygon.Draw(g, drawingAlgorithm);
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            mode = Mode.Move;
        }

        private void canvas_MouseDown(object sender, MouseEventArgs eventArgs)
        {
            if(mode == Mode.Move)
            {
                FindVertexToBeMoved(eventArgs.Location);
                if (isVertexMoved) return;
                FindEdgeToBeMoved(eventArgs.Location);
                if (isEdgeMoved) return;
                FindPolygonToBeMoved(eventArgs.Location);
            }

            if(mode == Mode.Delete)
            {
                DeleteVertex(eventArgs.Location);
            }

            if(mode == Mode.AddVertex)
            {
                AddVertex(eventArgs.Location);
            }

            if(mode == Mode.FixLength)
            {
                FixLength(eventArgs.Location);
            }
        }

        private void FindVertexToBeMoved(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} hit");
                        movedVertex = vertex;
                        (segment1, segment2) = polygon.GetAdjacentEdges(vertex);

                        previousPoint = p;
                        isVertexMoved = true;

                        return;
                    }
                }
            }
        }

        private void FindEdgeToBeMoved(Point p)
        {
            foreach(var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} hit");
                        movedEdge = edge;
                        (segment1, segment2) = polygon.GetAdjacentEdges(edge);
                        (vertex1, vertex2) = polygon.GetEndpoints(edge);

                        previousPoint = p;
                        isEdgeMoved = true;

                        return;
                    }
                }
            }
        }

        private void FindPolygonToBeMoved(Point p)
        {
            foreach(var polygon in polygons)
            {
                if (polygon.HitTest(p))
                {
                    Debug.WriteLine($"Polygon {polygon} hit");
                    movedPolygon = polygon;
                    previousPoint = p;
                    isPolygonMoved = true;

                    return;
                }
            }
        }

        private void DeleteVertex(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var vertex in polygon.Vertices)
                {
                    if (vertex.HitTest(p))
                    {
                        Debug.WriteLine($"Vertex {vertex} designated for deletion");
                        polygon.Delete(vertex);

                        canvas.Invalidate();
                        return;
                    }
                }
            }
        }

        private void AddVertex(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} designated for adding a vertex");
                        polygon.Subdivide(edge);

                        canvas.Invalidate();
                        return;
                    }
                }
            }
        }

        private void FixLength(Point p)
        {
            foreach (var polygon in polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (edge.HitTest(p))
                    {
                        Debug.WriteLine($"Edge {edge} will have fixed length");
                        // query user for length
                        double length = QueryForEdgeLength(edge);
                        if (length <= 0)
                            MessageBox.Show($"Invalid input: Positive real values only.",
                                "Polygons", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // adjust length
                        polygon.SetLength(edge, length);
                        polygon.FixedLengthEdges.Add(edge);

                        canvas.Invalidate();
                        return;
                    }
                }
            }
        }

        private void radioButtonLineLibrary_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLineLibrary.Checked == true)
                drawingAlgorithm.SegmentDrawingAlgorithm = DrawingAlgorithms.LineLibrary;
        }

        private void radioButtonLineBresenham_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLineBresenham.Checked == true)
                drawingAlgorithm.SegmentDrawingAlgorithm = DrawingAlgorithms.LineBresenham;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            mode = Mode.Delete;
        }

        private void buttonAddVertex_Click(object sender, EventArgs e)
        {
            mode = Mode.AddVertex;
        }

        private void buttonFixLength_Click(object sender, EventArgs e)
        {
            mode = Mode.FixLength;
        }

        private double QueryForEdgeLength(Segment edge)
        {
            string lengthString = "";
            double length;
            using (InputForm inputForm = new InputForm())
            {
                inputForm.Input = edge.Length.ToString();
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    lengthString = inputForm.Input;
                    Debug.WriteLine("Form1 received length " + lengthString);
                }
            }
            try
            {
                length = double.Parse(lengthString);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Invalid input: {exception}", "Polygons",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                length = -1;
            }

            return length;
        }
    }
}