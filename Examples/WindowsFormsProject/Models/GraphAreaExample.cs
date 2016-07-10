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
using GraphX.PCL.Logic.Models;

namespace Schema
{
    /// <summary>
    /// This is custom GraphArea representation using custom data types.
    /// GraphArea is the visual panel component responsible for drawing visuals (vertices and edges).
    /// It is also provides many global preferences and methods that makes GraphX so customizable and user-friendly.
    /// </summary>
    /// 
   

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
    public  class customGXLogicCore : GXLogicCore<DataVertex, DataEdge, SchemaGraph>
    {

    }
    public class GraphAreaExample : GraphArea<DataVertex, DataEdge, SchemaGraph>
    {
       
        public GraphAreaExample()
        {
            ControlFactory = new CustomGraphControlFactory(this);
        }
        Filter filter;

      

        public void process()
        {
            Dictionary<string, DataVertex> dic;
            var graphschema = LogicCore.Graph as SchemaGraph;
            dic = graphschema.dic;
            filter = Filter.createPathFilter("ED_STUD","ED_STUD_SCHOLASTIC");
            filter.dic = dic;      
            LogicCore.Filters.Enqueue(filter);

            RelayoutGraph();
      }
        
    }

}
