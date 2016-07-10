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
        SchemaGraph input;

        BidirectionalGraph<DataVertex, DataEdge> clone;

        public Demo(string filename)
        {
            input = new SchemaGraph();
            input.LoadGraph(filename);
            clone = input.Clone();
            foreach (DataEdge e in clone.Edges)
            {
                clone.AddEdge(e.reverse());
            }

        }

        SchemaGraph Filter(Filter f)
        {
            return null;
        }
    }
}
