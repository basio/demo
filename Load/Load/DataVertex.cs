

using System;

namespace Schema
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  
     */
    public enum VertexType
    {
        DataVertex, Table, Attribute, Concept
    }
    public class DataVertex
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }
        public virtual VertexType Type { get { return VertexType.DataVertex; } }
        #region Calculated or static props
        public override bool Equals(object obj)
        {
            DataVertex v = (DataVertex)obj;
            if (v.Text.Equals(Text) && v.Type == Type) return true;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return Text;
        }


        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex() : this("")
        {
        }

        public DataVertex(string text = "")
        {
            Text = text;
        }
    }
    class Table : DataVertex
    {
        public Table(String name)
        {
            Text = name;
        }
        public override VertexType Type { get { return VertexType.Table; } }
    }
    class Attribute : DataVertex
    {
        public DataVertex parent;
        public bool isPrimaryKey = false;
        string datatype;
        public Attribute(DataVertex p, String name, string _datatype)
        {
            this.parent = p;
            Text = name;
            datatype = _datatype;
        }
        public override string ToString()
        {
            return parent.ToString() + " " + Text + " " + isPrimaryKey;
        }
        public override VertexType Type { get { return VertexType.Attribute; } }
    }

    class Concept : DataVertex
    {
        public Concept(String name)
        {
            Text = name;
        }
        public override VertexType Type { get { return VertexType.Concept; } }
    }

}
