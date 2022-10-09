using System.Diagnostics;

namespace Polygons
{
    internal class Relations
    {
        public Dictionary<int, List<Segment>> relations;
        public int Count { get => relations.Count; }

        public Relations()
        {
            relations = new Dictionary<int, List<Segment>>();
        }

        public int AddEmptyRelation()
        {
            relations.Add(Count, new List<Segment>());
            return Count - 1;
        }
    }
}
