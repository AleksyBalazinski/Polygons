using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygons
{
    internal class Polygons
    {
        private List<Polygon> polygons;

        public void Add(Polygon polygon) => polygons.Add(polygon);
    }
}
