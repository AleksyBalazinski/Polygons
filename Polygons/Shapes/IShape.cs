namespace Polygons.Shapes
{
    internal interface IShape
    {
        bool HitTest(Point p);
        void Draw(Graphics g, Algorithm a);
        void Move(Point d);
    }
}