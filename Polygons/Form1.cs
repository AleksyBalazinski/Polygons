using Polygons.States;
using System.Diagnostics;

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
            InitializePolygons();
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

        private void buttonDeleteRelation_Click(object sender, EventArgs e)
        {
            int relId = Utilities.QueryForRelationId();
            foreach (var chain in relations[relId])
            {
                foreach (var edge in chain)
                {
                    edge.RelationId = null;
                    edge.chain = null;
                    edge.adjacentEdges.Item1.relationIds.Item1 = null;
                    edge.endpoints.Item1.relationIds.Item1 = null;
                    edge.adjacentEdges.Item2.relationIds.Item2 = null;
                    edge.endpoints.Item2.relationIds.Item2 = null;
                }
            }
            relations.Remove(relId);
            canvas.Invalidate();
        }

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

        private void InitializePolygons()
        {
            TransitionTo(new DrawState());

            // hexagon
            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 195, 83, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 93, 158, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 93, 158, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 94, 269, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 94, 269, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 203, 321, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 203, 321, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 293, 272, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 293, 272, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 291, 160, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 291, 160, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 197, 83, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 197, 83, 0));

            // pentagon
            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 455, 212, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 455, 295, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 455, 295, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 636, 294, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 636, 294, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 635, 213, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 635, 213, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 544, 144, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 544, 144, 0));
            canvas_MouseMove(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 458, 211, 0));

            canvas_MouseUp(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 458, 211, 0));

            // rel 0
            TransitionTo(new AddRelationState());
            // hexagon, 0
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 94, 198, 0));
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 291, 204, 0));
            // pentagon, 0
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 455, 247, 0));
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 636, 249, 0));

            // rel 1
            TransitionTo(new AddRelationState());
            // hexagon, 1
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 134, 128, 0));
            // pentagon, 1
            canvas_MouseDown(Canvas, new MouseEventArgs(MouseButtons.Left, 1, 500, 178, 0));

            // fixing lengths
            Polygons[0].Edges[3].fixedLength = true;
            Polygons[0].Edges[3].declaredLength = Polygons[0].Edges[3].Length;

            Polygons[1].Edges[4].fixedLength = true;
            Polygons[1].Edges[4].declaredLength = Polygons[1].Edges[4].Length;
        }
    }
}