using System.Diagnostics;
using Polygons.States;

namespace Polygons
{
    partial class Form1 : Form
    {
        private readonly List<Polygon> polygons;
        readonly Algorithm drawingAlgorithm;
        private readonly Relations relations;
        public List<Polygon> Polygons { get => polygons; }
        public Relations Relations { get => relations; }
        public PictureBox Canvas { get => canvas; }

        private State state = null!;
        public void TransitionTo(State state)
        {
            Debug.WriteLine($"Application context: Transition to {state.GetType().Name}");
            this.state = state;
            this.state.Context = this;
            this.state.Algorithm = drawingAlgorithm;
        }

        public Form1()
        {
            InitializeComponent();
            canvas.Image = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            using (Graphics g = Graphics.FromImage(canvas.Image))
            {
                g.Clear(Color.White);
            }
            polygons = new List<Polygon>();
            drawingAlgorithm = new Algorithm();
            relations = new Relations();

            TransitionTo(new DrawState());
        }
        
        //
        // event handlers
        //

        private void canvas_MouseUp(object sender, MouseEventArgs e)
            => state.canvas_MouseUp(sender, e);

        private void canvas_MouseMove(object sender, MouseEventArgs e)
            => state.canvas_MouseMove(sender, e);

        private void canvas_MouseDown(object sender, MouseEventArgs e)
            => state.canvas_MouseDown(sender, e);

        private void canvas_Paint(object sender, PaintEventArgs e)
            => state.canvas_Paint(sender, e);
        //
        // transitions
        //

        private void buttonDraw_Click(object sender, EventArgs e)
            => TransitionTo(new DrawState());

        private void buttonMove_Click(object sender, EventArgs e)
            => TransitionTo(new MoveState());

        private void buttonDelete_Click(object sender, EventArgs e)
            => TransitionTo(new DeleteState());

        private void buttonAddVertex_Click(object sender, EventArgs e)
            => TransitionTo(new AddVertexState());

        private void buttonFixLength_Click(object sender, EventArgs e)
            => TransitionTo(new FixLengthState());

        private void addRelation_Click(object sender, EventArgs e)
            => TransitionTo(new AddRelationState());

        //
        // algorithm selection
        //

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

        // polygons control
        public Polygon? FindPolygon(Segment edge)
        {
            foreach (var p in polygons)
            {
                foreach (var e in p.Edges)
                {
                    if (e == edge)
                        return p;
                }
            }
            return null;
        }
        public Polygon? FindPolygon(Vertex vertex)
        {
            foreach (var p in polygons)
            {
                foreach (var v in p.Vertices)
                {
                    if (v == vertex)
                        return p;
                }
            }
            return null;
        }
    }
}