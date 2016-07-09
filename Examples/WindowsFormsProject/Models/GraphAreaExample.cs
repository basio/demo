using GraphX;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using GraphX.Controls;
using GraphX.PCL.Common.Interfaces;
using System.Drawing;
using GraphX.Controls.Models;
using System.Windows.Media;
using QuickGraph.Algorithms;

namespace WindowsFormsProject
{
    /// <summary>
    /// This is custom GraphArea representation using custom data types.
    /// GraphArea is the visual panel component responsible for drawing visuals (vertices and edges).
    /// It is also provides many global preferences and methods that makes GraphX so customizable and user-friendly.
    /// </summary>
    /// 
    public class EmptyFilter : IGraphFilter<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {
        public BidirectionalGraph<DataVertex, DataEdge> ProcessFilter(BidirectionalGraph<DataVertex, DataEdge> inputGraph)
        {
            return new BidirectionalGraph<DataVertex, DataEdge>();
        }
    }
    public class VertexFilter : IGraphFilter<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {
        public Dictionary<String, DataVertex> dic;
        public String sourceVertex;
        List<DataVertex> getExactMatch(BidirectionalGraph<DataVertex, DataEdge> graph, string name)
        {
            DataVertex source = dic[name];
            if (source is Attribute)
            {
                //need to get the table
                foreach (var e in graph.OutEdges(source))
                {
                    if ((e.Weight == 1) && (e.Target is Table))
                    {
                        source = e.Target;
                        break;
                    }
                }
            }
            List<DataVertex> list = new List<DataVertex>();
            list.Add(source);
            return list;
        }
        List<DataVertex> getFuzzyMatch(BidirectionalGraph<DataVertex, DataEdge> graph, string name)
        {
            List<DataVertex> list = new List<DataVertex>();
            foreach (string k in dic.Keys)
            {
                if (k.Contains(name))
                {
                    if (dic[k] is Table)
                        list.Add(dic[k]);

                }
            }
            return list;
        }


        List<DataVertex> getMatching(BidirectionalGraph<DataVertex, DataEdge> graph, List<DataVertex> sources)
        {
            List<DataVertex> vertex_list = new List<DataVertex>();
            vertex_list.AddRange(sources);
            foreach (DataVertex source in sources)
                foreach (var e in graph.OutEdges(source))
                {
                    //   if (e.Weight == 1)
                    vertex_list.Add(e.Target);
                }
            return vertex_list;
        }


        protected List<DataVertex> getTable(BidirectionalGraph<DataVertex, DataEdge> graph, String name, bool exact = true)
        {
            if (exact)
                return getMatching(graph, getExactMatch(graph, name));
            else
                return getMatching(graph, getFuzzyMatch(graph, name));
        }

        public virtual BidirectionalGraph<DataVertex, DataEdge> ProcessFilter(BidirectionalGraph<DataVertex, DataEdge> inputGraph)
        {
            BidirectionalGraph<DataVertex, DataEdge> filteredgraph = new BidirectionalGraph<DataVertex, DataEdge>();
            List<DataVertex> l = getTable(inputGraph, sourceVertex);
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
        public String destVertex;
       
        public override BidirectionalGraph<DataVertex, DataEdge> ProcessFilter(BidirectionalGraph<DataVertex, DataEdge> inputGraph)
        {
            return inputGraph;
        }
    }

    public class PathFilter : VertexFilter
    {
       public String destVertex;
        public double weight(DataEdge e)
        {
            return e.Weight;
        }
        public override BidirectionalGraph<DataVertex, DataEdge> ProcessFilter(BidirectionalGraph<DataVertex, DataEdge> inputGraph)
        {

            BidirectionalGraph<DataVertex, DataEdge> filteredgraph = new BidirectionalGraph<DataVertex, DataEdge>();
            Func<DataEdge, double> distnace = weight;
            DataVertex source = getTable(inputGraph, sourceVertex).First<DataVertex>();
            DataVertex dest = getTable(inputGraph, destVertex).First<DataVertex>();
            BidirectionalGraph<DataVertex, DataEdge> clone = inputGraph.Clone();
            foreach(DataEdge e in inputGraph.Edges)
            {
                clone.AddEdge(e.reverse());
            }
            TryFunc<DataVertex, IEnumerable<DataEdge>> tryGetPath = clone.ShortestPathsDijkstra<DataVertex,DataEdge>(distnace, source);
            // enumerating path to targetCity, if any
            List<DataVertex> list = new List<DataVertex>();
            List<DataEdge> edgelist = new List<DataEdge>();
            list.Add(source);
            IEnumerable<DataEdge> path;
            if (tryGetPath(dest, out path))
                foreach (var e in path)  {
                    list.Add(e.Target);
                    edgelist.Add(e);
                }
            
            filteredgraph.AddVertexRange(list);
            filteredgraph.AddEdgeRange(edgelist);

            return filteredgraph;
        }

    }



    public class CustomGraphControlFactory : GraphControlFactory
    {
        public CustomGraphControlFactory(GraphAreaBase graphArea) : base(graphArea)
        {
        }

        int getVertexColor(object v)
        {
            if (v is Table)
                return 1;
            else return 0;
        }
        public override VertexControl CreateVertexControl(object vertexData)
        {
            var vertex_control = new VertexControl(vertexData) { RootArea = FactoryRootArea };
            if (getVertexColor(vertexData) == 1)
                vertex_control.Background = System.Windows.Media.Brushes.Gold;
            else
                vertex_control.Background = System.Windows.Media.Brushes.LightBlue;
            return vertex_control;
        }

    }

    public class GraphAreaExample : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {
        public GraphAreaExample()
        {
            ControlFactory = new CustomGraphControlFactory(this);
        }
        PathFilter filter;

        public void process(Dictionary<string, DataVertex> dic)
        {
            filter = new PathFilter();
            filter.sourceVertex = "ED_STUD";
            filter.destVertex = "ED_STUD_SCHOLASTIC";
            filter.dic = dic;
            LogicCore.Filters.Enqueue(filter);
            RelayoutGraph();
        }


    }

}
