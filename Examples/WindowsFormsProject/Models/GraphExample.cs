using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
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

      public  Dictionary<string, DataVertex> dic;
        public void AddForeignKey(string filename)
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
                var cols = fk.Col.Split(',');
                var refcols = fk.RefColumn.Split(',');
                for (int i = 0; i < cols.Length; i++)
                {
                    try
                    {
                        string table_col = fk.Table + "." + cols[i];
                        DataVertex curr = dic[table_col];
                        string ref_table_col = fk.RefTable + "." + refcols[i];
                        DataVertex reff = dic[ref_table_col];
                        graph.AddEdge(new DataEdge(curr, reff, 10));
                    }
                    catch (Exception e) { }
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
            StreamWriter sw = new StreamWriter("c:\\data\\aa.txt");
            foreach (var table in tables)
            {
                Table t = new Table(table.Table);

                graph.AddVertex(t);
                dic.Add(t.Text, t);

                sw.WriteLine(t.Text);

                foreach (var column in table.Columns)
                {
                    string colname = column.Attribute("id").Value;
                    Attribute col = new Attribute(t, colname);
                    graph.AddVertex(col);
                    dic.Add(col.Text, col);
                    var e1 = new DataEdge(t, col, 1);
                    graph.AddEdge(e1);
                }
            }
            sw.Close();
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


    }
}
