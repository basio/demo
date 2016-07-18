using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

using QuickGraph.Algorithms;


namespace Schema
{
    public abstract class Filter /*: IGraphFilter<DataVertex, DataEdge, SchemaGraph>*/
    {
        public abstract SchemaGraph ProcessFilter(SchemaGraph inputGraph);
      
        public static Filter createPathFilter(string sourceVertex, string destVertex)
        {
            return new PathFilter(sourceVertex, destVertex);
        }
       

    }
    public class EmptyFilter : Filter
    {
        public override SchemaGraph ProcessFilter(SchemaGraph inputGraph)
        {
            return new SchemaGraph(inputGraph);
        }
    }
    public class VertexFilter : Filter
    {
        public string sourceVertex;
        public VertexFilter()
        {

        }
        public VertexFilter(string sourceVertex)
        {
            this.sourceVertex = sourceVertex;

        }

        public override SchemaGraph ProcessFilter(SchemaGraph inputGraph)
        {
            SchemaGraph filteredgraph = new SchemaGraph(inputGraph);
            List<DataVertex> l = inputGraph.getTable(sourceVertex);
            HashSet<DataVertex> hs = new HashSet<DataVertex>(l);
            filteredgraph.AddVertexRange(l);
            //add edges
            foreach (var e in inputGraph.Edges)
            {
                if (hs.Contains(e.Source) && hs.Contains(e.Target))
                    filteredgraph.AddEdge(e);
            }

            return filteredgraph;
        }

    }
    public class NoFilter : VertexFilter
    {
        public override SchemaGraph ProcessFilter(SchemaGraph inputGraph)
        {
            return inputGraph;
        }
    }

    public class PathFilter : VertexFilter
    {
        public string destVertex;
        public PathFilter(string sourceVertex, string destVertex) : base(sourceVertex) { this.destVertex = destVertex; }

        double weight(DataEdge e)
        {
            return e.Weight;
        }
        public override SchemaGraph ProcessFilter(SchemaGraph inputGraph)
        {
         
            SchemaGraph filteredgraph = new SchemaGraph(inputGraph);


            Func<DataEdge, double> distnace = weight;
            DataVertex source = inputGraph.getTable(sourceVertex).First<DataVertex>();
            DataVertex dest = inputGraph.getTable(destVertex).First<DataVertex>();
            BidirectionalGraph<DataVertex, DataEdge> clone = inputGraph.Clone();
            foreach (DataEdge e in inputGraph.Edges)
            {
                clone.AddEdge(e.reverse());
            }
            TryFunc<DataVertex, IEnumerable<DataEdge>> tryGetPath = clone.ShortestPathsDijkstra<DataVertex, DataEdge>(distnace, source);
            // enumerating path to targetCity, if any
            List<DataVertex> list = new List<DataVertex>();
            List<DataEdge> edgelist = new List<DataEdge>();
            list.Add(source);
            IEnumerable<DataEdge> path;
            if (tryGetPath(dest, out path))
                foreach (var e in path)
                {
                    list.Add(e.Target);
                    edgelist.Add(e);
                }

            filteredgraph.AddVertexRange(list);
            filteredgraph.AddEdgeRange(edgelist);

            return filteredgraph;
        }

    }

    public class QueryFilter : Filter
    {
        public string query;
        public override SchemaGraph ProcessFilter(SchemaGraph inputGraph)
        {
            return null;
        }
    }
}

