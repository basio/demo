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
        public Dictionary<string, List<DataVertex>> dic;
        private void addDic(DataVertex v)
        {
            List<DataVertex> list;
            if (!dic.TryGetValue(v.Text, out list))
            {
                list = new List<DataVertex>();
                dic.Add(v.Text, list);
            }

            list.Add(v);
        }
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
                        List<DataVertex> curr_list = dic[table_col];
                        string ref_table_col = fk.RefTable + "." + refcols[i];
                        //  DataVertex reff = dic[ref_table_col];
                        foreach (DataVertex curr in curr_list)
                        {
                            if (curr is Attribute)
                            {
                                Attribute attr = curr as Attribute;
                                if (attr.parent.Text.Equals(ref_table_col))
                                {
                                    DataEdge edge = new DataEdge(curr, attr.parent, 10);
                                    edge.Type = DataEdge.EdgeType.DB_fk;
                                    AddEdge(edge);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }

            }
        }
        public void LoadOntology(string filename)
        {
            XDocument xdoc = XDocument.Load(filename);
            var mappings = from lv1 in xdoc.Descendants("mapping")
                           select new
                           {
                               concept = lv1.Element("concept").Value,
                               node = lv1.Element("node").Value
                           };


            foreach (var mapping in mappings)
            {
                Concept concept = new Concept(mapping.concept);

                AddVertex(concept);
                addDic(concept);

                List<DataVertex> vs = dic[mapping.node];
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
                             Columns = lv1.Descendants("COL"),
                             PrimaryKeys = lv1.Descendants("primarykeyconstraint")
                         };

            dic = new Dictionary<string, List<DataVertex>>();

            foreach (var table in tables)
            {
                Table t = new Table(table.Table);

                AddVertex(t);
                addDic(t);

                //handling primary key
                HashSet<string> pks = new HashSet<string>();
                if (table.PrimaryKeys != null)
                {
                    foreach (var x in table.PrimaryKeys)
                    {
                        string[] xs = x.Value.Trim().Split('\n');
                        foreach (string s in xs)
                            pks.Add(s);
                    }
                }

                foreach (var column in table.Columns)
                {
                    string colname = column.Attribute("id").Value;
                    string typename = column.Attribute("type").Value;
                    Attribute col = new Attribute(t, colname, typename);
                    if (column.Attribute("primary").Value.ToLower().Equals("true")||pks.Contains(colname))
                        col.isPrimaryKey = true;
                    AddVertex(col);
                    addDic(col);
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
            List<DataVertex> list = new List<DataVertex>();
            if (!dic.ContainsKey(name)) return list;
           List<DataVertex> source = dic[name];
            List<DataVertex> srs = new List<DataVertex>();
            foreach (DataVertex v in source)
            {
                if (v is Attribute)
                {
                    //need to get the table
                    foreach (var e in OutEdges(v))
                    {
                        //TODO: change this
                        if ((e.Weight == 1) && (e.Target is Table))
                        {
                            srs.Add(e.Target);
                            break;
                        }
                    }
                }
                else
                    srs.Add(v);
            }
            list.AddRange(srs);
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
                    foreach(var d in dic[k])
                    if (d is Table) 
                        list.Add(d);

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
                for (int i = 0; i < name.Length; i++)
                {
                    if (!kk.Contains(name[i].ToLower()))
                    { match = false; break; }
                }
                if (match)
                    list.AddRange(dic[k]);

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
