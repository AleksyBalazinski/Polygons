using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal interface IShape
    {
        bool HitTest(Point p);
        void Draw(Graphics g);
        void Move(Point d);
    }
}
