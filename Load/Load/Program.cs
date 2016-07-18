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
            //AntlrInputStream inputStream = new AntlrInputStream("12");

            //kqlLexer lexer = new kqlLexer(inputStream);
            //CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            //kqlParser parser = new kqlParser(commonTokenStream);

            //IParseTree tree = parser.input();
            //var visitor = new IntegerMathVisitor();

            //Console.WriteLine(visitor.Visit(tree));

        }
        static void demo(string[] args)
        {

            Demo d =Demo.init("c:\\data\\db.xml");
          
            Console.WriteLine(d.Vertices.Count());
            Console.WriteLine(d.Edges.Count());
            //Filter f = Filter.createPathFilter(d.input.dic, "ED_STUD", "AS_NODE");

            //SchemaGraph g = f.ProcessFilter(d.input);
            //Console.WriteLine(g.getGraph().VertexCount);
            //Console.WriteLine(g.getGraph().EdgeCount);
            //var l =d.input.getAllFuzzyMatch(new string[] { "AS_NODE" });
            //foreach(DataVertex v in l)
            //{
            //    Console.WriteLine(v);
            //}
            foreach(DataVertex v in d.Vertices)
            {
                Console.WriteLine(v);
            }

        }
        static void Main(string[] args)
        {

            Demo d =  Demo.init("c:\\data\\dbms.xml");

            List<Command> commands = Query.parse("            select           student[           graduate^female].grade        ");

            foreach(Command c in commands)
            {
                c.getSQL();
            }
        }
    }
}
