using System;
using System.Collections.Generic;

namespace GeometryTutorLib.GenericInstantiator
{
    //
    // An aggregation class of information to pass back from instantiation.
    //
    public class EdgeAggregator
    {
        public List<ConcreteAST.GroundedClause> antecedent { get; private set; }
        public ConcreteAST.GroundedClause consequent { get; private set; }
        public Hypergraph.EdgeAnnotation annotation { get; private set; }
        
        public EdgeAggregator(List<ConcreteAST.GroundedClause> antes, ConcreteAST.GroundedClause c, Hypergraph.EdgeAnnotation ann)
        {
            antecedent = new List<ConcreteAST.GroundedClause>(antes);
            consequent = c;
            annotation = ann;
        }
    }
}
