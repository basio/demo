using QuickGraph;
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
        public void AddRelations(BidirectionalGraph<DataVertex, DataEdge> g)
        {
            List<DataVertex> vlist = new List<DataVertex>();
            List<DataEdge> elist = new List<DataEdge>();
            foreach (var v in vertices)
            {
                if(v.Type == VertexType.Concept)
                {
                    foreach(DataEdge edge in g.OutEdges(v))
                    {
                        if (edge.Type == DataEdge.EdgeType.OntRel)
                        {
                            vlist.Add(edge.Target);
                            elist.Add(edge);
                        }
                    }
                }
            }
            vertices.AddRange(vlist);
            edges.AddRange(elist);
        }
        public BidirectionalGraph<DataVertex, DataEdge> getGraph()
        {
            BidirectionalGraph<DataVertex, DataEdge> g = new BidirectionalGraph<DataVertex, DataEdge>();
            foreach(var v in vertices)
            {
                g.AddVertex(v);
            }
            foreach (var edge in edges)
            {
                g.AddEdge(edge);
            }
            return g;
        }
    }
}
