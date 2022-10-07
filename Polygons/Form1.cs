using System.Diagnostics;

namespace Polygons
{
    public partial class Form1 : Form
    {
        private readonly Bitmap drawArea;
        private const int zoomFactor = 1;
        Mode mode = Mode.Draw;

        private List<Polygon> polygons;
        private Polygon constructedPolygon;
        private Polygon movedPolygon;
        private Segment? drawnSegment;

        Point previousPoint = Point.Empty;
        bool isMoved = false;
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

            drawingAlgorithm = new Algorithm();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && e.Button == MouseButtons.Left)
            {
                if(drawnSegment != null && constructedPolygon.Size >= 3 && IsOnVertex(constructedPolygon.GetVertex(0), e.X, e.Y, 10))
                {
                    constructedPolygon.AddEdge(drawnSegment);
                    polygons.Add(constructedPolygon);
                    Debug.WriteLine("Add polygon " + constructedPolygon.ToString());
                    constructedPolygon = new Polygon();
                    drawnSegment = null;
                }
                else
                {
                    Vertex v = new Vertex(e.X, e.Y);
                    
                    Debug.WriteLine("Add vertex " + v.ToString());
                    if (drawnSegment != null)
                        constructedPolygon.AddEdge(drawnSegment);
                    drawnSegment = new Segment();
                    drawnSegment.Point1 = drawnSegment.Point2 = new Point(e.X, e.Y);
                    Debug.WriteLine($"New segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                    
                    constructedPolygon?.AddVertex(v);
                }

                canvas.Invalidate();
            }

            if(mode == Mode.Move)
            {
                isMoved = false;
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
            if(mode == Mode.Draw && drawnSegment!= null && !constructedPolygon.IsEmpty())
            {
                drawnSegment.Point2 = new Point(e.X, e.Y);
                canvas.Invalidate();
            }

            if(mode == Mode.Move && isMoved == true)
            {
                var displacement = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                movedPolygon.Move(displacement);
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

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Move)
            {
                foreach(var polygon in polygons)
                {
                    if (polygon.HitTest(e.Location))
                    {
                        Debug.WriteLine($"Polygon {polygon} hit");
                        movedPolygon = polygon;
                        previousPoint = e.Location;
                        isMoved = true;

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
    }
}