using System.Diagnostics;
using Polygons.States;

namespace Polygons
{
    partial class Form1 : Form
    {
        private readonly Bitmap drawArea;
        private List<Polygon> polygons;
        readonly Algorithm drawingAlgorithm;
        public List<Polygon> Polygons { get => polygons; }
        public PictureBox Canvas { get => canvas; }

        private State state;
        public void TransitionTo(State state)
        {
            Debug.WriteLine($"Application context: Transition to {state.GetType().Name}");
            this.state = state;
            this.state.Context = this;
        }

        public Form1()
        {
            InitializeComponent();
            drawArea = new Bitmap(canvas.Size.Width, canvas.Size.Height);
            canvas.Image = drawArea;
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }
            polygons = new List<Polygon>();

            drawingAlgorithm = new Algorithm();

            state = new DrawState(this, drawingAlgorithm);
            state.Context = this;
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
            => TransitionTo(new DrawState(this, drawingAlgorithm));

        private void buttonMove_Click(object sender, EventArgs e)
            => TransitionTo(new MoveState(this, drawingAlgorithm));

        private void buttonDelete_Click(object sender, EventArgs e)
            => TransitionTo(new DeleteState(this, drawingAlgorithm));

        private void buttonAddVertex_Click(object sender, EventArgs e)
            => TransitionTo(new AddVertexState(this, drawingAlgorithm));

        private void buttonFixLength_Click(object sender, EventArgs e)
            => TransitionTo(new FixLengthState(this, drawingAlgorithm));

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
    }
}