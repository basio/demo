using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WindowsFormsProject
{
    /// <summary>
    /// This is our custom data graph derived from BidirectionalGraph class using custom data types.
    /// Data graph stores vertices and edges data that is used by GraphArea and end-user for a variety of operations.
    /// Data graph content handled manually by user (add/remove objects). The main idea is that you can dynamicaly
    /// remove/add objects into the GraphArea layout and then use data graph to restore original layout content.
    /// </summary>
    public class GraphExample
    {
        BidirectionalGraph<DataVertex, DataEdge> graph;
        BidirectionalGraph<DataVertex, DataEdge> filteredgraph;

        Dictionary<String, DataVertex> dic;
        public void AddForeignKey(String filename)
        {

            XDocument xdoc = XDocument.Load(filename);

            //Find tables  and their attributes
            var FKs = from lv1 in xdoc.Descendants("AlterTableAddTableElementStatement")
                      let table = lv1.XPathSelectElement("BASE/SchemaObject")
                      let col = lv1.XPathSelectElement("FKEY/COL")
                      let refTable = lv1.XPathSelectElement("FKEY/Ref/SchemaObject")
                      let refColumn = lv1.XPathSelectElement("FKEY/Ref/COL")
                      where table != null && col != null && refTable != null && refColumn != null


                      select new
                      {
                          Table = table.Value.Trim(),
                          Col = col.Value.Trim(),
                          RefTable = refTable.Value.Trim(),
                          RefColumn = refColumn.Value.Trim()
                      };
                
            foreach (var fk in FKs)
            {
                var cols=fk.Col.Split(',');
                var refcols= fk.RefColumn.Split(',');
                for(int i = 0; i < cols.Length; i++) {
                    String table_col = fk.Table + "." + cols[i];
                    DataVertex curr = dic[table_col];
                    String ref_table_col = fk.RefTable + "." + refcols[i];
                    DataVertex  reff= dic[table_col];
                    graph.AddEdge(new DataEdge(curr, reff, 10));
                }
                
            }
        }
    

    private void LoadXml(String filename)
        {
            XDocument xdoc = XDocument.Load(filename);

            //Find tables  and their attributes
            var tables = from lv1 in xdoc.Descendants("table")
                         select new
                         {
                             Table = lv1.Element("SchemaObject").Value,
                             Columns = lv1.Descendants("COL")
                         };
            graph = new BidirectionalGraph<DataVertex, DataEdge>();
            dic = new Dictionary<string, DataVertex>();
            //Loop through tables
            foreach (var table in tables)
            {
                Table t = new Table(table.Table);

                graph.AddVertex(t);
                dic.Add(t.Text, t);

                foreach (var column in table.Columns)
                {
                    Attribute col = new Attribute(t, column.Value);


                    graph.AddVertex(col);
                    dic.Add(col.Text, col);
                    var e1 = new DataEdge(t, col, 1);

                    graph.AddEdge(e1);
                }

            }
        }

        
        public void LoadGraph(String name)
        {
            LoadXml(name);
            AddForeignKey(name);
        }
        public BidirectionalGraph<DataVertex, DataEdge> getGraph()
        {
            return graph;
        }
        List<DataVertex> getTable(String name)
        {
            DataVertex source = dic[name];
            if(source is Attribute) {
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

            foreach (var e in graph.OutEdges(source))      {
                if (e.Weight == 1)
                    vertex_list.Add(e.Target);
            }
            return vertex_list;
        }
        public void FilterGraph(String a)
        {
            filteredgraph = new BidirectionalGraph<DataVertex, DataEdge>();
            List<DataVertex> l = getTable(a);
            filteredgraph.AddVertexRange(l);
            
        }
        public BidirectionalGraph<DataVertex, DataEdge> getFGraph( )
        {
            if (filteredgraph == null)
                return graph;
            else
                return filteredgraph;
        }
    }
}
