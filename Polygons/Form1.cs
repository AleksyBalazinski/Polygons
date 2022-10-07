using System;
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
                if(drawnSegment != null && constructedPolygon.Vertices.Count >= 3 && IsOnVertex(constructedPolygon.Vertices[0], e.X, e.Y, 10))
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
                    Vertex v = new Vertex(e.X, e.Y);
                    
                    Debug.WriteLine("Add vertex " + v.ToString());
                    if (drawnSegment != null)
                    {
                        constructedPolygon.Edges.Add(drawnSegment);
                        Debug.WriteLine($"Add segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                    }
                        
                    drawnSegment = new Segment();
                    drawnSegment.Point1 = drawnSegment.Point2 = new Point(e.X, e.Y);

                    constructedPolygon?.Vertices.Add(v);
                }

                canvas.Invalidate();
            }

            if(mode == Mode.Move)
            {
                isPolygonMoved = false;
                isVertexMoved = false;
                isEdgeMoved = false;
            }
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            mode = Mode.Draw;
        }

        /*private Vertex ToCartesian(int x, int y)
        {
            return new Vertex((x - canvas.Width / 2) / zoomFactor, (canvas.Height / 2 - y) / zoomFactor);
        }

        private (int, int) ToScreen(Vertex vertex)
        {
            return ((int)(vertex.X * zoomFactor + canvas.Width / 2), (int)(canvas.Height / 2 - vertex.Y * zoomFactor));
        }*/

        private bool IsOnVertex(Vertex vertex, int x, int y, int d)
        {
            return x >= vertex.Center.X - d && x <= vertex.Center.X + d && y >= vertex.Center.Y - d && y <= vertex.Center.Y + d;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && drawnSegment!= null && constructedPolygon.Vertices.Count != 0)
            {
                drawnSegment.Point2 = new Point(e.X, e.Y);
                canvas.Invalidate();
            }

            if(mode == Mode.Move && isPolygonMoved == true)
            {
                var displacement = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                movedPolygon.Move(displacement);
                previousPoint = e.Location;
                canvas.Invalidate();
            }

            if(mode == Mode.Move && isVertexMoved == true)
            {
                var displacement = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                movedVertex.Move(displacement);
                segment1.MoveStart(displacement);
                segment2.MoveEnd(displacement);
                previousPoint = e.Location;
                canvas.Invalidate();
            }

            if(mode == Mode.Move && isEdgeMoved == true)
            {
                var displacement = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                movedEdge.Move(displacement);
                segment1.MoveEnd(displacement);
                segment2.MoveStart(displacement);
                vertex1.Move(displacement);
                vertex2.Move(displacement);
                previousPoint = e.Location;
                canvas.Invalidate();
            }
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
                foreach(var polygon in polygons)
                {
                    for(int vi = 0; vi < polygon.Vertices.Count; vi++) // moving a single vertex
                    {
                        Vertex v = polygon.Vertices[vi];
                        if (v.HitTest(eventArgs.Location))
                        {
                            Debug.WriteLine($"Vertex {v} hit");
                            movedVertex = v;
                            segment1 = polygon.Edges[vi];
                            if(vi == 0)
                                segment2 = polygon.Edges[polygon.Vertices.Count - 1];
                            else
                                segment2 = polygon.Edges[vi - 1];

                            previousPoint = eventArgs.Location;
                            isVertexMoved = true;

                            return;
                        }
                    }

                    for(int ei = 0; ei < polygon.Vertices.Count; ei++) // moving a single edge
                    {
                        Segment e = polygon.Edges[ei];
                        if(e.HitTest(eventArgs.Location))
                        {
                            Debug.WriteLine($"Edge {e} hit");
                            movedEdge = e;
                            if (ei == 0)
                            {
                                segment1 = polygon.Edges[^1];
                                vertex1 = polygon.Vertices[0];
                            }
                                
                            else
                            {
                                segment1 = polygon.Edges[ei - 1];
                                vertex1 = polygon.Vertices[ei];
                            }
                                

                            if (ei == polygon.Edges.Count - 1)
                            {
                                segment2 = polygon.Edges[0];
                                vertex2 = polygon.Vertices[0];
                            }
                                
                            else
                            {
                                segment2 = polygon.Edges[ei + 1];
                                vertex2 = polygon.Vertices[ei + 1];
                            }
                                

                            previousPoint = eventArgs.Location;
                            isEdgeMoved = true;

                            return;
                        }
                    }

                    if (polygon.HitTest(eventArgs.Location)) // moving entire polygon
                    {
                        Debug.WriteLine($"Polygon {polygon} hit");
                        movedPolygon = polygon;
                        previousPoint = eventArgs.Location;
                        isPolygonMoved = true;

                        return;
                    }
                        
                }
            }

            if(mode == Mode.Delete)
            {
                foreach(var polygon in polygons)
                {
                    for(int vi = 0; vi < polygon.Vertices.Count; vi++)
                    {
                        Vertex v = polygon.Vertices[vi];
                        if(v.HitTest(eventArgs.Location)) // deleting a single vertex
                        {
                            Debug.WriteLine($"Vertex {v} designated for deletion");
                            Segment segment2 = polygon.Edges[vi];
                            Segment segment1;
                            if (vi == 0)
                                segment1 = polygon.Edges[polygon.Vertices.Count - 1];
                            else
                                segment1 = polygon.Edges[vi - 1];

                            if (vi == 0)
                                polygon.Edges.Add(new Segment(segment1.Point1, segment2.Point2));
                            else
                                polygon.Edges.Insert(vi - 1, new Segment(segment1.Point1, segment2.Point2));

                            polygon.Vertices.Remove(v);
                            polygon.Edges.Remove(segment1);
                            polygon.Edges.Remove(segment2);
                        }
                    }
                }
                canvas.Invalidate();
            }

            if(mode == Mode.AddVertex)
            {
                foreach(var polygon in polygons)
                {
                    for(int ei = 0; ei < polygon.Edges.Count; ei++)
                    {
                        Segment e = polygon.Edges[ei];
                        if(e.HitTest(eventArgs.Location))
                        {
                            Debug.WriteLine($"Edge {e} designated for adding a vertex");
                            Vertex v = new Vertex((e.Point1.X + e.Point2.X) / 2, (e.Point1.Y + e.Point2.Y) / 2);
                            polygon.Vertices.Insert(ei + 1, v);
                            polygon.Edges.Insert(ei, new Segment(e.Point1, v.Center));
                            polygon.Edges.Insert(ei + 1, new Segment(v.Center, e.Point2));
                            polygon.Edges.Remove(e);

                            canvas.Invalidate();
                            return;
                        }
                    }
                }
            }

            if(mode == Mode.FixLength)
            {
                foreach(var polygon in polygons)
                {
                    for(int ei = 0; ei < polygon.Edges.Count; ei++)
                    {
                        Segment e = polygon.Edges[ei];
                        if(e.HitTest(eventArgs.Location))
                        {
                            Debug.WriteLine($"Edge {e} will have fixed length");
                            // query user for length
                            string lengthString = "";
                            double length = 0;
                            using (InputForm inputForm = new InputForm())
                            {
                                inputForm.Input = e.Length.ToString();
                                if(inputForm.ShowDialog() == DialogResult.OK)
                                {
                                    lengthString = inputForm.Input;
                                    Debug.WriteLine("Form1 received length " + lengthString);
                                }
                            }
                            try
                            {
                                length = double.Parse(lengthString);
                            } 
                            catch(Exception exception)
                            {
                                MessageBox.Show($"Invalid input: {exception}", "Polygons", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if(length <= 0)
                                MessageBox.Show($"Invalid input: Positive real values only.", 
                                    "Polygons", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            // adjust length
                            e.Length = length;
                            polygon.Vertices[ei] = new Vertex(e.Point1.X, e.Point1.Y);

                            if (ei == polygon.Edges.Count - 1)
                                polygon.Vertices[0] = new Vertex(e.Point2.X, e.Point2.Y);
                            else
                                polygon.Vertices[ei + 1] = new Vertex(e.Point2.X, e.Point2.Y);

                            if (ei == 0)
                                polygon.Edges[^1].MoveEndAbs(new Point(e.Point1.X, e.Point1.Y));
                            else
                                polygon.Edges[ei - 1].MoveEndAbs(new Point(e.Point1.X, e.Point1.Y));

                            if (ei == polygon.Edges.Count - 1)
                                polygon.Edges[0].MoveStartAbs(new Point(e.Point2.X, e.Point2.Y));
                            else
                                polygon.Edges[ei + 1].MoveStartAbs(new Point(e.Point2.X, e.Point2.Y));

                            polygon.FixedLengthEdges.Add(e);

                            canvas.Invalidate();
                            return;
                        }
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
    }
}