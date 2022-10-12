using System.Diagnostics;
using Polygons.States;

namespace Polygons
{
    partial class Form1 : Form
    {
        private readonly List<Polygon> polygons;
        readonly Algorithm drawingAlgorithm;
        public List<Polygon> Polygons { get => polygons; }
        // Each relation has a unique identifier associated with it and is comprised of a number of "chains",
        // where each chain is just a sequence of adjacent edges that are in the relation
        public Dictionary<int, List<List<Segment>>> relations;
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
                g.Clear(Color.WhiteSmoke);
            }
            polygons = new List<Polygon>();
            drawingAlgorithm = new Algorithm();
            relations = new Dictionary<int, List<List<Segment>>>();

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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygons.Clear();
            using (Graphics g = Graphics.FromImage(canvas.Image))
            {
                g.Clear(Color.WhiteSmoke);
            }
            relations.Clear();
            TransitionTo(new DrawState());
            this.Refresh();
        }
    }
}