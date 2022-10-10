namespace Polygons
{
    internal interface IShape
    {
        bool HitTest(PointF p);
        void Draw(Graphics g, Algorithm a);
        void Move(PointF d);
    }
}
