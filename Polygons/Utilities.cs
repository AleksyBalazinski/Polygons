using System.Diagnostics;

namespace Polygons
{
    static class Utilities
    {
        public static bool IsOnVertex(Vertex vertex, int x, int y, int d)
        {
            return x >= vertex.Center.X - d && x <= vertex.Center.X + d && y >= vertex.Center.Y - d && y <= vertex.Center.Y + d;
        }

        public static double QueryForEdgeLength(Segment edge)
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
