using System.Diagnostics;
using Polygons.Shapes;

namespace Polygons
{
    static class Utilities
    {
        public static bool IsOnVertex(Vertex vertex, float x, float y, float d)
        {
            return x >= vertex.Center.X - d && x <= vertex.Center.X + d && y >= vertex.Center.Y - d && y <= vertex.Center.Y + d;
        }

        public static float QueryForEdgeLength(Segment edge)
        {
            string lengthString = "";
            float length;
            using (InputForm inputForm = new())
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
                length = float.Parse(lengthString);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Invalid input: {exception}", "Polygons",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                length = -1;
            }

            return length;
        }
        public static int QueryForRelationId()
        {
            string relIdString = "";
            using (RelationInputForm inputForm = new())
            {
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    relIdString = inputForm.Input;
                }
            }

            return int.Parse(relIdString);
        }
    }
}
