using Polygons.Shapes;

namespace Polygons
{
    internal class Chains
    {
        public static void RotateChain(List<Segment> chain, Point axis, float sinTheta, float cosTheta)
        {
            foreach (var e in chain)
            {
                (Vertex vertex1, Vertex vertex2) = e.endpoints;
                Point v = e.Point1 - axis;
                var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
                e.Point1 = vRot + axis;
                vertex1.Center = e.Point1;

                Point u = e.Point2 - axis;
                var uRot = Geometry.Rotate(u, sinTheta, cosTheta);
                e.Point2 = uRot + axis;
                vertex2.Center = e.Point2;
            }
        }

        public static void RotateChainForeward(List<Segment> chain, Point axis, float sinTheta, float cosTheta)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                Segment e = chain[i];

                Point v = e.Point1 - axis;
                var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
                e.Point1 = vRot + axis;

                Point u = e.Point2 - axis;
                var uRot = Geometry.Rotate(u, sinTheta, cosTheta);
                e.Point2 = uRot + axis;
                (Vertex vertex1, Vertex vertex2) = e.endpoints;

                vertex1.Center = e.Point1;
                if (i != chain.Count - 1)
                    vertex2.Center = e.Point2;
            }
        }

        public static void RotateChainBackward(List<Segment> chain, Point axis, float sinTheta, float cosTheta)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                Segment e = chain[i];

                Point v = e.Point1 - axis;
                var vRot = Geometry.Rotate(v, sinTheta, cosTheta);
                e.Point1 = vRot + axis;

                Point u = e.Point2 - axis;
                var uRot = Geometry.Rotate(u, sinTheta, cosTheta);
                e.Point2 = uRot + axis;
                (Vertex vertex1, Vertex vertex2) = e.endpoints;

                if (i != 0)
                    vertex1.Center = e.Point1;
                vertex2.Center = e.Point2;
            }
        }

        public static void TranslateForeward(List<Segment> chain, Point displacement)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                Segment e = chain[i];
                e.Point1 += displacement;
                e.Point2 += displacement;
                (Vertex v1, Vertex v2) = e.endpoints;
                v1.Center = e.Point1;
                if (i != chain.Count - 1)
                    v2.Center = e.Point2;
            }
        }

        public static void TranslateBackward(List<Segment> chain, Point displacement)
        {
            for (int i = 0; i < chain.Count; i++)
            {
                Segment e = chain[i];
                e.Point1 += displacement;
                e.Point2 += displacement;
                (Vertex v1, Vertex v2) = e.endpoints;
                v2.Center = e.Point2;
                if (i != 0)
                    v1.Center = e.Point1;
            }
        }
    }
}
