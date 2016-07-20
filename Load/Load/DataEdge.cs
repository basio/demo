using System;
using QuickGraph;


namespace Schema
{
    /* DataEdge is the data class for the edges. It contains all custom edge data specified by the user.
     * This class also must be derived from EdgeBase class that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful EdgeBase members are:
     *  - ID property that stores unique positive identfication number. Property must be filled by user.
     *  - IsSelfLoop boolean property that indicates if this edge is self looped (eg have identical Target and Source vertices) 
     *  - RoutingPoints collection of points used to create edge routing path. If Null then straight line will be used to draw edge.
     *      In most cases it is handled automatically by GraphX.
     *  - Source property that holds edge source vertex.
     *  - Target property that holds edge target vertex.
     *  - Weight property that holds optional edge weight value that can be used in some layout algorithms.
     */
    public abstract class EdgeBase<TVertex> : QuickGraph.IEdge<TVertex>
    {
        /// <summary>
        /// Skip edge in algo calc and visualization
        /// </summary>
   

        protected EdgeBase(TVertex source, TVertex target, double weight = 1)        {

            Source = source;
            Target = target;
            Weight = weight;
            ID = -1;
        }

        /// <summary>
        /// Unique edge ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Returns true if Source vertex equals Target vertex
        /// </summary>
        public bool IsSelfLoop
        {
            get { return Source.Equals(Target); }
        }

        /// <summary>
        /// Optional parameter to bind edge to static vertex connection point
        /// </summary>
        public int? SourceConnectionPointId { get; set; }

        /// <summary>
        /// Optional parameter to bind edge to static vertex connection point
        /// </summary>
        public int? TargetConnectionPointId { get; set; }

      
        /// <summary>
        /// Source vertex
        /// </summary>
        public TVertex Source { get; set; }

        /// <summary>
        /// Target vertex
        /// </summary>
        public TVertex Target { get; set; }

        /// <summary>
        /// Edge weight that can be used by some weight-related layout algorithms
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Reverse the calculated routing path points.
        /// </summary>
        public bool ReversePath { get; set; }

       
    }

public class DataEdge : EdgeBase<DataVertex>
    {

        public enum EdgeType {  DB_attr, DB_fk, Ontology, Inverted, Reduandant,OntRel };

        public EdgeType Type { get; set; }
        /// <summary>
        /// Default constructor. We need to set at least Source and Target properties of the edge.
        /// </summary>
        /// <param name="source">Source vertex data</param>
        /// <param name="target">Target vertex data</param>
        /// <param name="weight">Optional edge weight</param>
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }
        /// <summary>
        /// Default parameterless constructor (for serialization compatibility)
        /// </summary>
        public DataEdge()
            : base(null, null, 1)
        {
        }

        /// <summary>
        /// Custom string property for example
        /// </summary>
        public string Text { get; set; }

        #region GET members
        public override string ToString()
        {
            return Text;
        }
        #endregion
        DataEdge _original = null;
        public DataEdge original
        {
            get
            {
                if (_original == null) return this;
                return _original;
            }
        }
        
        public DataEdge reverse()
        {
            DataEdge t = new DataEdge();
            t.Target = this.Source;
            t.Source = this.Target;
            t.Text = this.Text;
            t.Weight = this.Weight;
            t._original = this;
            if(t.Type!=DataEdge.EdgeType.Ontology)
            t.Type= DataEdge.EdgeType.Reduandant;
            return t;

        }
    }
}
