using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema;
using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms;

namespace Load
{
    class Demo
    {
        public static Demo demo;
        private Demo() { }
       public SchemaGraph input;
        
        BidirectionalGraph<DataVertex, DataEdge> clone;
        public SchemaGraph query;

        public IEnumerable<DataVertex> Vertices { get { return input.Vertices; } }

        public IEnumerable<DataEdge> Edges { get { return input.Edges; } }


        public static Demo init(string filename, string mapping)
        {
            demo = new Demo(filename, mapping);
            return demo;
        }

        private Demo(string filename, string mapping)
        {
            input = new SchemaGraph();
            input.LoadGraph(filename);
            input.LoadOntology(mapping);
            clone = input.Clone();

            foreach (DataEdge e in clone.Edges)
            {
                if ((e.Type == DataEdge.EdgeType.DB_attr) || (e.Type == DataEdge.EdgeType.DB_fk))
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
        double weight(DataEdge e)
        {
            return e.Weight;
        }

        //get the 
        public SchemaGraph distance(List<DataVertex> sources, List<DataVertex> dests)
        {
           SchemaGraph g = new SchemaGraph();
            List<Path> paths = new List<Path>();
            g.AddVertexRange(sources);
            foreach (var dest in dests)
            {
                Path p = null;
                LabeledGraph.BFS(clone, sources, dest, out p);
                if (p != null)
                {
                    //add 
                   
                    try
                    {
                        p.AddRelations(clone);
                    }
                    catch (Exception ee) { }
                    g.AddVertexRange(p.getGraph().Vertices);
                    g.AddEdgeRange(p.getGraph().Edges);
                    paths.Add(p);
                }
            }


            return g;
        }
    }

}

