using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Schema;

namespace Load
{

    //Alogrithm to convert to sql 
    //build a graph with all possible candit ie
    //determine XOR nodes (i.e., if x in graph then y is redudant) 
    // determine needed vertices
    //find All pathes which contains all nodes
    //find the best path [cheapest to execute]
    //

    //what about the correntess
    //Do this for all data systems 
    //what if the data are accross systems

        
    //class token
    //{
    //    enum TokenType { ID, VERB, VAR, IDLIST, CONDITION };
    //    string value;
    //    TokenType type;
    //}

    //repsentes enity with conidtion classes
      class Entity
    {
        string id;
        string conditions;
       public Entity(string id, string condition)
        {
            this.id = id;
            this.conditions = condition;
        }
        public string getSql()
        {
            //id may be either table, attribute,  or variable 
            //I have two options
            //(1) handle the variable as a graph
            //(2) use the variable as a whole
            //for now we keep the things simple


            List<DataVertex> id_candidates = Demo.demo.getCandidateMatch(id);
            //for each conditi
            List<string> conds = conditions.Split('^').ToList<string>();
            //List<DataVertex> cond_condition
            List<DataVertex> candidates = new List<DataVertex>();
            foreach(string cond in conds) {
                List<DataVertex> cond_candidates = Demo.demo.getCandidateMatch(cond);
                candidates.AddRange(cond_candidates);
            }
            Demo.demo.distance(id_candidates, candidates);
            return "";
            
        }
    }
    class IDList     {
        public List<Entity> enities;
        public string condition=""; //this is at the end condition
        public static IDList parse(string line)
        {
            IDList idlist = new IDList();
            string[] parts = line.Split('.');

            idlist.enities = new List<Entity>();
            

            foreach (string part in parts)
            {
                string id = part;
                string condition = "";
                int i = part.IndexOf('[');
                if (i > 0)
                {
                    id = part.Substring(0, i );
                    condition = part.Substring(i + 1, part.Length - 2 - i);
                }

                idlist.enities.Add(new Entity(id,condition));
                
            }
            return idlist;
        }
        public string getSQL()
        {
            return enities[0].getSql();
        }
    }
    class Command
    {
        public string variable;
        public string verb;
        public List<IDList> lists;
        static int id = 0;
        public static Command parse(string line)
        {
            Command c = new Command();
            int i = line.IndexOf("<-");
            if (i > 0)
            {
                c.variable = line.Substring(0, i).Trim().ToLower();
                line = line.Substring(i + 2);
            } else
            {
                c.variable = "temp_" +id.ToString();
                id++;
            }
            line = line.Trim();
            i = line.IndexOf(" ");
            if (i > 0)
            {
                c.verb = line.Substring(0, i).Trim().ToLower();
                line = line.Substring(i + 1);
               // line = Regex.Replace(line, @"\s+", "");
            }
            string[] idlists = line.Split(',');
            c.lists = new List<IDList>();
            foreach (string idlist in idlists)
            {
                c.lists.Add(IDList.parse(idlist));
            }
            return c;
        }
      public string getSQL()
        {
            //choose the best queries by joinin
            //so far we have only one IDLIST 
            //it should not be a problem yet
            if (verb.Equals("select"))
            {
                return  lists[0].getSQL();
            }
            else
            if (verb.Equals("count"))
            {
                return lists[0].getSQL();
            } else
            {
                throw new Exception("Not supported");
            }
        }
    }
    class Query
    {
        private Query() { }
        public static List<Command> parse(string lines)
        {
            try
            {
                lines = Regex.Replace(lines, @"\s+", " ");
                string[] ls = lines.Split('\n');
                List<Command> commands = new List<Command>();
                foreach (string s in ls)
                {
                    commands.Add(Command.parse(s));
                }
                return commands;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
