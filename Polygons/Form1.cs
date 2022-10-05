namespace Polygons
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private Bitmap drawArea;
        private Pen pen;
        private const int vertexRadius = 5;
        private const int zoomFactor = 1;
        Mode mode;

        private Polygons polygons;
        private Polygon currentPolygon;
 
        public Form1()
        {
            InitializeComponent();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }
            pen = new Pen(Brushes.Black);
            mode = Mode.Move;
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if(mode == Mode.Draw && e.Button == MouseButtons.Left)
            {
                using (Graphics g = Graphics.FromImage(drawArea))
                {
                    g.FillEllipse(Brushes.Black, e.X - vertexRadius, e.Y - vertexRadius, vertexRadius * 2, vertexRadius * 2);
                }

                if(!currentPolygon.IsEmpty() && currentPolygon.GetVertexCoordinates(0) == ToCartesian(e.X, e.Y) && currentPolygon.Size >= 3)
                {
                    polygons.Add(currentPolygon);
                    Console.WriteLine("Add polygon");
                }

                canvas.Refresh();
            }
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            mode = Mode.Draw;
            currentPolygon = new Polygon();
        }

        private (double, double) ToCartesian(double x, double y)
        {
            return ((x - canvas.Width / 2) / zoomFactor, (canvas.Height / 2 - y) / zoomFactor);
        }
    }
}