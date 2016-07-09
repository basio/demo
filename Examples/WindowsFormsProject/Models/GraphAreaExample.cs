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
        List<DataVertex> getTable(BidirectionalGraph<DataVertex, DataEdge> graph, String name)
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
            List<DataVertex> vertex_list = new List<DataVertex>();
            vertex_list.Add(source);

            foreach (var e in graph.OutEdges(source))
            {
                if (e.Weight == 1)
                    vertex_list.Add(e.Target);
            }
            return vertex_list;
        }

        public BidirectionalGraph<DataVertex, DataEdge> ProcessFilter(BidirectionalGraph<DataVertex, DataEdge> inputGraph)
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
            var vertex_control= new VertexControl(vertexData) { RootArea = FactoryRootArea };
            if (getVertexColor(vertexData) == 1)
                vertex_control.Background = System.Windows.Media.Brushes.Gold;
            else
                vertex_control.Background = System.Windows.Media.Brushes.LightBlue;
            return vertex_control;
        }
        
    }
    public class GraphAreaExample : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>
    {
    public    GraphAreaExample()
        {
            ControlFactory=new  CustomGraphControlFactory(this);
        }
        VertexFilter filter;

        public void process(Dictionary<string, DataVertex> dic)
        {
            filter = new VertexFilter();
            filter.sourceVertex = "PG_STUD_VIOL_PNLTY";
            filter.dic = dic;
            LogicCore.Filters.Enqueue(filter);
            RelayoutGraph();
        }
    

    }

}
