using System.Diagnostics;

namespace Polygons.States
{
    abstract class State
    {
        protected Form1 context;
        protected Algorithm drawingAlgorithm;

        protected State(Form1 context, Algorithm a)
        {
            drawingAlgorithm = a;
            this.context = context;
        }

        public Form1 Context { set => context = value; }

        public abstract void canvas_MouseUp(object sender, MouseEventArgs e);
        public abstract void canvas_MouseMove(object sender, MouseEventArgs e);
        public abstract void canvas_MouseDown(object sender, MouseEventArgs e);
        public virtual void canvas_Paint(object sender, PaintEventArgs e)
        {
            Debug.WriteLine("painting");
            DrawScene(e.Graphics);
        }

        public virtual void DrawScene(Graphics g)
        {
            foreach (var polygon in context.Polygons)
                polygon.Draw(g, drawingAlgorithm);
        }
    }
}
