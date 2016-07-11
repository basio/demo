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
        static void Main(string[] args)
        {

            Demo d = new Demo("c:\\data\\d.xml");
          
            Console.WriteLine(d.input.getGraph().VertexCount);
            Console.WriteLine(d.input.getGraph().EdgeCount);
            //Filter f = Filter.createPathFilter(d.input.dic, "ED_STUD", "AS_NODE");

            //SchemaGraph g = f.ProcessFilter(d.input);
            //Console.WriteLine(g.getGraph().VertexCount);
            //Console.WriteLine(g.getGraph().EdgeCount);
            //var l =d.input.getAllFuzzyMatch(new string[] { "AS_NODE" });
            //foreach(DataVertex v in l)
            //{
            //    Console.WriteLine(v);
            //}
            foreach(DataVertex v in d.input.Vertices)
            {
                Console.WriteLine(v);
            }

        }
    }
}
