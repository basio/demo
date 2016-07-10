using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Load
{
    class Program
    {

        static void o()
        {
            AntlrInputStream inputStream = new AntlrInputStream("12");

            kqlLexer lexer = new kqlLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            kqlParser parser = new kqlParser(commonTokenStream);

            IParseTree tree = parser.compileUnit();
            var visitor = new IntegerMathVisitor();

            Console.WriteLine(visitor.Visit(tree));

        }
        static void Main(string[] args)
        {

            SchemaGraph s = new SchemaGraph();
            s.LoadGraph("c:\\data\\b.xml");

            if ("abc".Contains("abc"))
                Console.WriteLine("ok");

            Console.WriteLine(s.getGraph().VertexCount);
            Console.WriteLine(s.getGraph().EdgeCount);
            Filter f = Filter.createPathFilter(s.dic, "ED_STUD", "AS_NODE");

            SchemaGraph g = f.ProcessFilter(s);
            Console.WriteLine(g.getGraph().VertexCount);
            Console.WriteLine(g.getGraph().EdgeCount);
            var l = s.getAllFuzzyMatch(new string[] { "AS_NODE" });
            foreach(DataVertex v in l)
            {
                Console.WriteLine(v);
            }

        }
    }
}
