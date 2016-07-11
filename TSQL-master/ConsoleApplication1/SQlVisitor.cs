using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;

namespace SqlParserSample
{
    class NullVisitor : TSqlFragmentVisitor
    {

    }
    class SQlVisitor : TSqlFragmentVisitor
    {
        StreamWriter sw;
        public SQlVisitor(StreamWriter sw)
        {
            this.sw = sw;
        }

        public override void Visit(TSqlStatement node)
        {
            base.Visit(node);
        }
        public override void Visit(SchemaObjectName node)
        {
            String s = "";
            if (node.DatabaseIdentifier != null)
                s += node.DatabaseIdentifier.Value + ".";
            if (node.BaseIdentifier != null)
                s += node.BaseIdentifier.Value;
            sw.WriteLine("<SchemaObject>" + s + "</SchemaObject>");
        }
        public override void Visit(AlterTableAddTableElementStatement node)
        {
            sw.WriteLine("<AlterTableAddTableElementStatement>");
            sw.WriteLine("<BASE>");

            node.SchemaObjectName.Accept(this);
            sw.WriteLine("</BASE>");

            node.Definition.AcceptChildren(this);
            
            sw.WriteLine("</AlterTableAddTableElementStatement>");
        }
        public override void Visit(ConstraintDefinition a)
        {
            /*
            if (a.ConstraintIdentifier == null)
                sw.WriteLine("<Constraint>");
            else
                sw.WriteLine("<Constraint id='" + a.ConstraintIdentifier.Value + "'>");
            a.AcceptChildren(this);
            sw.WriteLine("</Constraint>");*/
            a.AcceptChildren(this);
        }
        public override void Visit(MultiPartIdentifier v)
        {
            string s = "";
            foreach (Identifier i in v.Identifiers)
                s=i.Value + ",";
            sw.WriteLine(s.TrimEnd(','));
                }

       
        public override void Visit(ColumnReferenceExpression col)
        {
            col.AcceptChildren(this);
            sw.WriteLine("Col" + col);
        }

        public override void Visit(CreateTableStatement node)
        {
            sw.WriteLine("<table>");
            Visit(node.SchemaObjectName);
            foreach (ColumnDefinition coldef in node.Definition.ColumnDefinitions)
                Visit(coldef);
            sw.WriteLine("</table>");
        }
        public void Visit(CreateTableStatement node, IList<TSqlParserToken> TKs)
        {
            sw.WriteLine("<table>");
            Visit(node.SchemaObjectName);
            try
            {
                if (node.Definition.ColumnDefinitions != null)
                    if (node.Definition.ColumnDefinitions.Count > 0)
                        foreach (ColumnDefinition coldef in node.Definition.ColumnDefinitions)
                            Visit(coldef, TKs);
            }catch(Exception e) { }
                      try
            {
                if (node.Definition.TableConstraints != null)
                    if (node.Definition.TableConstraints.Count > 0)
                        foreach (UniqueConstraintDefinition constraint in node.Definition.TableConstraints)
                            if (constraint.IsPrimaryKey)
                            {
                                sw.WriteLine("<primarykeyconstraint>");
                              
                                foreach (ColumnWithSortOrder col in constraint.Columns)
                                    col.Column. MultiPartIdentifier.Accept(this);
                              
                                sw.WriteLine("</primarykeyconstraint>");
                            }
                          
            }
            catch (Exception e) { }
            
            sw.WriteLine("</table>");
        }
       
        public override void Visit(ColumnDefinition col)
        {
            String s = "<COL id='" + col.ColumnIdentifier.Value + "'";
            SqlDataTypeReference dt = (SqlDataTypeReference)col.DataType;

            s = s + " type='" + dt.SqlDataTypeOption + "'></COL>";
            sw.WriteLine(s);
            

            col.Accept(new NullVisitor());

        }
        public void Visit(ColumnDefinition col, IList<TSqlParserToken> TKs)
        {
            String s = "<COL id='" + col.ColumnIdentifier.Value + "'";
            SqlDataTypeReference dt = (SqlDataTypeReference)col.DataType;


            //search 
            int line = col.DataType.StartLine;
            int offset = col.DataType.StartOffset;
            int FragmentLength = col.DataType.FragmentLength;

            StringBuilder R = new StringBuilder("");

            int start = 0;
            for (int k = col.FirstTokenIndex; k<col.LastTokenIndex; k++)
            {
                TSqlParserToken TK = TKs.ElementAt(k);
               if (TK.Line == line)// && TK.Offset >= (offset + FragmentLength))
                {
                    if (TK.TokenType == TSqlTokenType.LeftParenthesis)
                    {
                        start = 1;
                    }
                    if (start == 1)
                    {
                        R.Append(TK.Text);
                    }
                    if (TK.TokenType == TSqlTokenType.RightParenthesis)
                    {
                        break;
                    }
                }
            //    else break; 

            }
            bool isprimary=false;
            foreach(ConstraintDefinition cd in col.Constraints)
            {
                if(cd is UniqueConstraintDefinition)
                {
                    if ((cd as UniqueConstraintDefinition).IsPrimaryKey) isprimary = true;
                }
            }
            s = s + " type='" + dt.SqlDataTypeOption  +"'" + " R='"+R+"'"+ "  primary='"+isprimary +"'></COL>";
            // TKs[col.LastTokenIndex+1]
            sw.WriteLine(s);

            col.Accept(new NullVisitor());

        }
       
        public override void Visit(ForeignKeyConstraintDefinition v)
        {
            sw.WriteLine("<FKEY>");
            sw.WriteLine("<COL>");
            String s = "";
           
            foreach (var col in v.Columns)
                s = s + col.Value + "," ;
            s = s.TrimEnd(',');
            sw.WriteLine(s);
            sw.WriteLine("</COL>");
                sw.WriteLine("<Ref>");
            v.ReferenceTableName.Accept(this);

            sw.WriteLine("<COL>");

            s = "";
            foreach (var col in v.ReferencedTableColumns)
                s = s + col.Value + ",";
            s = s.TrimEnd(',');
            sw.WriteLine(s);
            sw.WriteLine("</COL>");

            sw.WriteLine("</Ref>");
            sw.WriteLine("</FKEY>");
        }


    }

}
