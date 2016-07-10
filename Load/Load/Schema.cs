using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Schema
{
    public class SchemaGraph : BidirectionalGraph<DataVertex, DataEdge>
    {

        public Dictionary<string, DataVertex> dic;
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
                        DataEdge edge = new DataEdge(curr, reff, 10);
                        edge.Type = DataEdge.EdgeType.DB_fk;
                        AddEdge(edge);
                    }
                    catch (Exception ex) { }
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

            dic = new Dictionary<string, DataVertex>();
     
            foreach (var table in tables)
            {
                Table t = new Table(table.Table);

                AddVertex(t);
                dic.Add(t.Text, t);
                foreach (var column in table.Columns)
                {
                    string colname = column.Attribute("id").Value;
                    Attribute col = new Attribute(t, colname);
                    AddVertex(col);
                    dic.Add(col.Text, col);
                    var e1 = new DataEdge(t, col, 1);
                    e1.Type = DataEdge.EdgeType.DB_attr;
                    AddEdge(e1);
                }
            }
     
        }

        public void LoadGraph(String name)
        {
            LoadXml(name);
            AddForeignKey(name);

        }
        public SchemaGraph getGraph()
        {
            return this;
        }
        public SchemaGraph(SchemaGraph inputGraph)
        {
            dic = inputGraph.dic;
        }
        public SchemaGraph()
        {

        }
        #region search
        public List<DataVertex> getExactMatch(string name)
        {
            DataVertex source = dic[name];
            if (source is Attribute)
            {
                //need to get the table
                foreach (var e in OutEdges(source))
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
        public List<DataVertex> getTableFuzzyMatch(string name)
        {
            return getTableFuzzyMatch(new string[] { name });
        }
        public List<DataVertex> getAllFuzzyMatch(string name)
        {
            return getAllFuzzyMatch(new string[] { name });
        }
        public List<DataVertex> getTableFuzzyMatch(string[] name)
        {
            List<DataVertex> list = new List<DataVertex>();
            foreach (string k in dic.Keys)
            {
                bool match = true;
                string kk = k.ToLower();
                for (int i = 0; i < name.Length; i++)
                {
                    if (!kk.Contains(name[i].ToLower()))
                    { match = false; break; }
                }
                if (match)
                    if (dic[k] is Table)
                        list.Add(dic[k]);

            }
            return list;
        }

        public List<DataVertex> getAllFuzzyMatch(string[] name)
        {
            List<DataVertex> list = new List<DataVertex>();
            foreach (string k in dic.Keys)
            {
                bool match = true;
                string kk = k.ToLower();
                for(int i = 0; i < name.Length; i++)
                {
                    if (!kk.Contains(name[i].ToLower()))
                    { match = false; break; }                    
                }
                if (match)
                    list.Add(dic[k]);
               
            }
            return list;
        }
        public List<DataVertex> getMatching(List<DataVertex> sources)
        {
            List<DataVertex> vertex_list = new List<DataVertex>();
            vertex_list.AddRange(sources);
            foreach (DataVertex source in sources)
                foreach (var e in OutEdges(source))
                {
                    vertex_list.Add(e.Target);
                }
            return vertex_list;
        }
        public List<DataVertex> getTable(String name, bool exact = true)
        {
            if (exact)
                return getMatching(getExactMatch(name));
            else
                return getMatching(getTableFuzzyMatch(name));
        }
    }
    #endregion

}
