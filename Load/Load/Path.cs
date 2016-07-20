using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema
{
   public class Path
    {
        List<DataVertex> vertices;
        List<DataEdge> edges;

        double Cost;
        public Path(DataVertex v)
        {
            vertices = new List<Schema.DataVertex>();
            edges = new List<Schema.DataEdge>();
            Cost = 0;
            vertices.Add(v);
        }
        public void AddEdge(DataEdge e)
        {
            edges.Add(e);
            vertices.Add(e.Source);
            Cost += e.Weight;
        }
    }
}
