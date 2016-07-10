

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

    public class DataVertex 
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        #region Calculated or static props

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

    }
    class Attribute : DataVertex
    {
        DataVertex parent;
        public Attribute(DataVertex p, String name)
        {
            this.parent = p;
            Text = parent.Text + "." + name;
        }

    }



}
