using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Priority_Queue;
namespace Schema
{
    public class LabeledGraph : BidirectionalGraph<DataVertex, DataEdge>
    {

        protected LabeledGraph()
        {
            dic = new Dictionary<string, List<DataVertex>>();
        }
        protected LabeledGraph(LabeledGraph inputgraph)
        {
            dic = inputgraph.dic;
        }
        private Dictionary<string, List<DataVertex>> dic;
        private void addDic(DataVertex v)
        {
            List<DataVertex> list;
            string name = v.Text.ToLower();
            if (!dic.TryGetValue(name, out list))
            {
                list = new List<DataVertex>();
                dic.Add(name, list);
            }

            list.Add(v);

        }
        public bool CheckVertex(DataVertex v)
        {

            List<DataVertex> list;
            string name = v.Text.ToLower();
            if (!dic.TryGetValue(name, out list))
            {
                return false;
            }
            else
            {
                //check if the type is the same

                foreach (DataVertex vv in list)
                {
                    if (vv.Type == v.Type)
                        return true;
                }
                return false;

            }
        }

        public override bool AddVertex(DataVertex v)
        {
            //check if the vertex exists already with the same type
            if (v.Type != VertexType.Attribute)
                if (CheckVertex(v)) return false;

            bool t = base.AddVertex(v);
            if (t)
                addDic(v);
            return t;
        }

        public List<DataVertex> findVertexbyType(string name, Type type = null)
        {
            List<DataVertex> list;
            if (!dic.TryGetValue(name.ToLower(), out list))
            {
                return null;

            }
            else
            {
                if (type == null) return list;
                else return list.FindAll(e => e.GetType() == type);
            }
        }
        public List<DataVertex> findVertexbyType(DataVertex vertex)
        {
            List<DataVertex> list;
            if (!dic.TryGetValue(vertex.Text.ToLower(), out list))
            {
                return null;
            }
            else
            {
                return list.FindAll(e => e.Type == vertex.Type);
            }
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
                    foreach (var d in dic[k])
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

        #endregion

        public class DataVertexWithDistance : FastPriorityQueueNode
        {
            public DataVertex Vertex { get; private set; }
            public DataVertexWithDistance(DataVertex vertex)
            {
                Vertex = vertex;
            }
            public DataEdge Edge { get; set; }
            public DataVertexWithDistance Source { get; set; }
        }


        public static int BFS(BidirectionalGraph<DataVertex, DataEdge> graph, List<DataVertex> sources, List<DataVertex> dests, out List<Path> paths)
        {
            //FastPriorityQueue<DataVertexWithDistance>

            SimplePriorityQueue<DataVertexWithDistance> queue = new SimplePriorityQueue<DataVertexWithDistance>();
            paths = new List<Path>();
            foreach(var src in sources)
            queue.Enqueue(new DataVertexWithDistance(src), 0);
            while (queue.Count > 0)
            {
                DataVertexWithDistance v = queue.Dequeue();
                if (dests.Contains(v.Vertex))
                {
                    DataVertex dest = v.Vertex;
                   Path  path = new Path(v.Vertex);
                    while (v.Edge != null)
                    {
                        path.AddEdge(v.Edge);                      
                        v = v.Source;
                    }
                    paths.Add(path);
                    dests.Remove(dest);
                    if (dests.Count == 0) break;
                }
                double p = v.Priority;
                foreach (DataEdge edge in graph.OutEdges(v.Vertex))
                {
                    var tmp = new DataVertexWithDistance(edge.Target);
                    tmp.Edge = edge;
                    tmp.Source = v;
                    queue.Enqueue(tmp, edge.Weight + p);
                }
            }
            return 0;
        }
        public static int BFS(BidirectionalGraph<DataVertex, DataEdge> graph, List<DataVertex> sources, DataVertex dest, out Path path)
        {
            //FastPriorityQueue<DataVertexWithDistance>

            SimplePriorityQueue<DataVertexWithDistance> queue = new SimplePriorityQueue<DataVertexWithDistance>();
            path = null;
            HashSet<DataVertex> visited = new HashSet<DataVertex>();
            foreach (var src in sources)
            {
                queue.Enqueue(new DataVertexWithDistance(src), 0);
                visited.Add(src);
            }
            while (queue.Count > 0)
            {
                DataVertexWithDistance v = queue.Dequeue();
                if (dest.Equals(v.Vertex))
                {
                  //  DataVertex dest = v.Vertex;
                     path = new Path(v.Vertex);
                    while (v.Edge != null)
                    {
                        path.AddEdge(v.Edge);
                        v = v.Source;
                    }
                    break;
                }
                double p = v.Priority;
                foreach (DataEdge edge in graph.OutEdges(v.Vertex))
                {
                    if (!visited.Contains(edge.Target))
                    {
                        var tmp = new DataVertexWithDistance(edge.Target);
                        tmp.Edge = edge;
                        tmp.Source = v;
                        queue.Enqueue(tmp, edge.Weight + p);
                        visited.Add(edge.Target);
                    }
                }
            }
            return 0;
        }
    }
    public class SchemaGraph : LabeledGraph
    {
         
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
                          Table = table.Value.ToLower().Trim(),
                          Col = col.Value.ToLower().Trim(),
                          RefTable = refTable.Value.ToLower().Trim(),
                          RefColumn = refColumn.Value.ToLower().Trim()
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

                        string ref_table_col = fk.RefTable + "." + refcols[i];
                        //  DataVertex reff = dic[ref_table_col];
                        foreach (DataVertex curr in findVertexbyType(table_col).OfType<Attribute>())
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
                               concept = lv1.Element("concept").Value.ToLower(),
                               node = lv1.Element("node").Value.ToLower()
                           };


            foreach (var mapping in mappings)
            {
                Concept con = new Concept(mapping.concept);
                AddVertex(con);

                List<DataVertex> l = findVertexbyType(con);
                foreach (DataVertex concept in l)
                {
                    List<DataVertex> vs = findVertexbyType(mapping.node);
                    foreach (DataVertex vertex in vs)
                    {
                        DataEdge edge = new DataEdge(concept, vertex);
                        edge.Type = DataEdge.EdgeType.OntRel;
                        edge.Weight = 0;
                        AddEdge(edge);

                        DataEdge revedge = new DataEdge(vertex, concept);
                        revedge.Type = DataEdge.EdgeType.OntRel;
                        revedge.Weight = 0;
                        AddEdge(revedge);
                    }
                }
            }
            var links = from lv1 in xdoc.Descendants("link")
                        select new
                        {
                            a = lv1.Element("a").Value,
                            b = lv1.Element("b").Value
                        };
            foreach (var link in links)
            {
                Concept src_concept = new Concept(link.a);
                AddVertex(src_concept);

                List<DataVertex> sources = findVertexbyType(src_concept);
                foreach (DataVertex source in sources)
                {
                    Concept dest_concept = new Concept(link.b);
                    AddVertex(dest_concept);


                    List<DataVertex> dests = findVertexbyType(dest_concept);
                    foreach (DataVertex dest in dests)
                    {
                        DataEdge e = new DataEdge(source, dest);
                        e.Type = DataEdge.EdgeType.Ontology;
                        e.Weight = 0;
                        AddEdge(e);
                        DataEdge rev_e = new DataEdge(dest, source);
                        rev_e.Type = DataEdge.EdgeType.Ontology;
                        rev_e.Weight = 0;
                        AddEdge(rev_e);
                    }
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
                             Table = lv1.Element("SchemaObject").Value.ToLower(),
                             Columns = lv1.Descendants("COL"),
                             PrimaryKeys = lv1.Descendants("primarykeyconstraint")
                         };

            foreach (var table in tables)
            {
                Table t = new Table(table.Table);
                var add_t = AddVertex(t);

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
                    string colname = column.Attribute("id").Value.ToLower();
                    string typename = column.Attribute("type").Value.ToLower();
                    Attribute col = new Attribute(t, colname, typename);
                    if (column.Attribute("primary").Value.Equals("true") || pks.Contains(colname))
                        col.isPrimaryKey = true;
                    var col_v = AddVertex(col);

                    var edge = new DataEdge(t, col, 1);
                    edge.Type = DataEdge.EdgeType.DB_attr;
                    AddEdge(edge);
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
        public SchemaGraph(SchemaGraph inputGraph) :

                base(inputGraph)
        { }

        public SchemaGraph()
        {

        }

    }
}
