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
        double weight(DataEdge e)
        {
            return e.Weight;
        }
        public void distance(List<DataVertex> sources, List<DataVertex> dests)
        {
            var g = demo.clone;
         

            Func<DataEdge, double> distnace = weight;
            //var fw = new FloydWarshallAllShortestPathAlgorithm<DataVertex, DataEdge>(g, distnace);
          //  var dijkstra =   new DijkstraShortestPathAlgorithm<DataVertex, DataEdge>(g, distnace);
            foreach (DataVertex source in sources)
            {
                TryFunc<DataVertex, IEnumerable<DataEdge>> tryGetPath = g.ShortestPathsDijkstra<DataVertex, DataEdge>(distnace, source);

                foreach (var target in dests)
                {
                    IEnumerable<DataEdge> path;
                    if (tryGetPath( target, out path))
                        foreach (var edge in path)
                            Console.WriteLine(edge);
                }
            }
        }
        
    }
}
