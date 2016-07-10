using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema;

namespace Load
{
    class Program
    {
        static void Main(string[] args)
        {

            SchemaGraph s = new SchemaGraph();
            s.LoadGraph("c:\\data\\d.xml");           


            Console.WriteLine( s.getGraph().VertexCount);
            Console.WriteLine(s.getGraph().EdgeCount);
            Filter f=Filter.createPathFilter(s.dic,"ED_STUD", "ED_STUD_SCHOLASTIC");
            
            SchemaGraph g = f.ProcessFilter(s);
            Console.WriteLine(g.getGraph().VertexCount);
            Console.WriteLine(g.getGraph().EdgeCount);
        }
    }
}
