using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema;
using QuickGraph;

namespace Load
{
    class Demo
    {
        public static Demo demo;
        private Demo() { }
        SchemaGraph input;

        BidirectionalGraph<DataVertex, DataEdge> clone;

        public IEnumerable<DataVertex> Vertices { get { return input.Vertices; } }

        public IEnumerable<DataEdge> Edges {  get{ return input.Edges; } }

        public static Demo init(string filename) { demo = new Demo(filename); return demo; }
        private Demo(string filename)
        {
            input = new SchemaGraph();
            input.LoadGraph(filename);
            clone = input.Clone();
            foreach (DataEdge e in clone.Edges)
            {
                clone.AddEdge(e.reverse());
            }
            
        }
        public List<DataVertex> getCandidateMatch(string id, bool fuzzy = false)
        {
            if (fuzzy == true)
            {
                return input.getAllFuzzyMatch(id);
            }
            else return input.getExactMatch(id);
        }
        SchemaGraph Filter(Filter f)
        {
            return null;
        }
    }
}
