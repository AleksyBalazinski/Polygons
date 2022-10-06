using System.Diagnostics;

namespace Polygons
{
    public partial class Form1 : Form
    {
        private Bitmap drawArea;
        private const int zoomFactor = 1;
        Mode mode;

        private List<Polygon> polygons;
        private Polygon currentPolygon;
        private Segment? drawnSegment;

        Point previousPoint = Point.Empty;
 
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
            currentPolygon = new Polygon();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && e.Button == MouseButtons.Left)
            {
                if(drawnSegment != null && currentPolygon.Size >= 3 && IsOnVertex(currentPolygon.GetVertex(0), e.X, e.Y, 10))
                {
                    currentPolygon.AddEdge(drawnSegment);
                    polygons.Add(currentPolygon);
                    Debug.WriteLine("Add polygon " + currentPolygon.ToString());
                    currentPolygon = new Polygon();
                }
                else
                {
                    Vertex v = new Vertex(e.X, e.Y);
                    
                    Debug.WriteLine("Add vertex " + v.ToString());
                    if (drawnSegment != null)
                        currentPolygon.AddEdge(drawnSegment);
                    drawnSegment = new Segment();
                    drawnSegment.Point1 = drawnSegment.Point2 = new Point(e.X, e.Y);
                    Debug.WriteLine($"New segment ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                    
                    currentPolygon?.AddVertex(v);
                }

                canvas.Invalidate();
            }

            if(mode == Mode.Move)
            {
                // release

                // TODO last edge is probably assigned to wrong polygon
            }
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            mode = Mode.Draw;
        }

        private Vertex ToCartesian(int x, int y)
        {
            return new Vertex((x - canvas.Width / 2) / zoomFactor, (canvas.Height / 2 - y) / zoomFactor);
        }

        /*private (int, int) ToScreen(Vertex vertex)
        {
            return ((int)(vertex.X * zoomFactor + canvas.Width / 2), (int)(canvas.Height / 2 - vertex.Y * zoomFactor));
        }*/

        /*private bool IsOnVertex(Vertex vertex, int x, int y, int d)
        {
            (int vertexX, int vertexY) = ToScreen(vertex);
            return x >= vertexX - d && x <= vertexX + d && y >= vertexY - d && y <= vertexY + d;
        }*/

        private bool IsOnVertex(Vertex vertex, int x, int y, int d)
        {
            return x >= vertex.Center.X - d && x <= vertex.Center.X + d && y >= vertex.Center.Y - d && y <= vertex.Center.Y + d;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && drawnSegment!= null && !currentPolygon.IsEmpty())
            {
                drawnSegment.Point2 = new Point(e.X, e.Y);
                //Debug.WriteLine($"segment end moved ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                canvas.Invalidate();
            }

            if(mode == Mode.Move)
            {
                var displacement = new Point(e.X - previousPoint.X, e.Y - previousPoint.Y);
                currentPolygon.Move(displacement);
                previousPoint = e.Location;
                canvas.Invalidate();
            }
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics);
            if(drawnSegment != null)
            {
                //Debug.WriteLine($"segment drawn ({drawnSegment.Point1.X},{drawnSegment.Point1.Y})->({drawnSegment.Point2.X},{drawnSegment.Point2.Y})");
                drawnSegment.Draw(e.Graphics);
            }
        }

        private void DrawScene(Graphics g)
        {
            currentPolygon.Draw(g);
            foreach(var polygon in polygons)
                polygon.Draw(g);
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
                        currentPolygon = polygon;
                        previousPoint = e.Location;
                    }
                        
                }
            }
        }
    }
}