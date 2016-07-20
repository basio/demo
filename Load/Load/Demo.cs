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
        SchemaGraph input;

        BidirectionalGraph<DataVertex, DataEdge> clone;

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
        public void distance(List<DataVertex> sources, List<DataVertex> dests)
        {
            List<Path> paths=new List<Path>();
            foreach (var dest in dests)
            {
                Path p=null;
                LabeledGraph.BFS(clone, sources, dest, out p);
                if (p != null)
                    paths.Add(p);
            }

        }
        }

    }

