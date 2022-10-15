using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal static class Geometry
    {
        // directed towards b
        public static (float, float) AngleBetweenVectors(Point a, Point b)
        {
            float cosTheta = DotProduct(a, b) / (VectorLength(a) * VectorLength(b));
            float sinTheta = CrossProduct(a, b) / (VectorLength(a) * VectorLength(b));

            return (sinTheta, cosTheta);
        }

        public static float DotProduct(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static float CrossProduct(Point a, Point b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        public static float VectorLength(Point a)
        {
            return MathF.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        public static Point Rotate(Point v, float sinTheta, float cosTheta)
        {
            return new Point(v.X * cosTheta - v.Y * sinTheta, v.X * sinTheta + v.Y * cosTheta);
        }

        // a onto b
        public static Point OrthogonalProjection(Point a, Point b)
        {
            float c = DotProduct(a, b) / (VectorLength(b) * VectorLength(b));
            return new Point(c * b.X, c * b.Y);
        }
    }
}
