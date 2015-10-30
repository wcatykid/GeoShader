using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Xml;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A First Order Logic clause that describes a property about a geometric drawing.
    /// </summary>
    public abstract class GroundedClause
    {
        // A unique integer identifier (from the hypergraph)
        public int clauseId { get; private set; }
        public void SetID(int id) { clauseId = id; }

        // Intrinsic as defined theoretically: characteristics of a figure that cannot be proven.
        private bool intrinsic;
        public bool IsIntrinsic() { return intrinsic; }
        public void MakeIntrinsic() { intrinsic = true; mayBeSourceNode = false; }

        // For problem generation, indicate if this node is given 
        private bool given;
        public bool IsGiven() { return given; }
        public void MakeGiven() { given = true; }

        // Denotes: A + A -> A
        private bool purelyAlgebraic = false;
        public bool IsPurelyAlgebraic() { return purelyAlgebraic; }
        public void MakePurelyAlgebraic() { purelyAlgebraic = true; }

        // Contains all predecessors
        public List<int> generalPredecessors { get; private set; }
        // Contains only Relation-based predecessors
        public List<int> relationPredecessors { get; private set; }

        public bool HasRelationPredecessor(GroundedClause gc) { return relationPredecessors.Contains(gc.clauseId); }
        public bool HasGeneralPredecessor(GroundedClause gc) { return generalPredecessors.Contains(gc.clauseId) || relationPredecessors.Contains(gc.clauseId); }

        // Contains all figure fact predecessor / components (e.g. a triangle has 3 segments, 3 angles, and 3 points, etc) 
        public List<int> figureComponents { get; private set; }
        public void AddComponent(int component) { Utilities.AddUnique<int>(figureComponents, component); }
        public void AddComponentList(List<int> componentList) { Utilities.AddUniqueList<int>(figureComponents, componentList); }


        // Can this node be strengthened to the given node?
        public virtual bool CanBeStrengthenedTo(GroundedClause gc) { return false; }
        // For problems: if a theorem or result is obvious and should never be a real source node for a problem

        private bool mayBeSourceNode = true;
        public void SetNotASourceNode() { mayBeSourceNode = false; }
        public bool IsAbleToBeASourceNode() { return mayBeSourceNode; }

        private bool mayBeGoalNode = true;
        public void SetNotAGoalNode() { mayBeGoalNode = false; }
        public bool IsAbleToBeAGoalNode() { return !intrinsic && mayBeGoalNode; }

        private bool isObviousDefinition = false;
        public void SetClearDefinition() { isObviousDefinition = true; }
        public bool IsClearDefinition() { return isObviousDefinition; }

        public virtual void DumpXML(Action<string, List<GroundedClause>> write)
        {
            write("TBD", new List<GroundedClause>());
        }

        public void AddRelationPredecessor(GroundedClause gc)
        {
            if (gc.clauseId == -1)
            {
                Debug.WriteLine("ERROR: id is -1: " + gc.ToString());
            }
            Utilities.AddUnique(relationPredecessors, gc.clauseId);
        }
        public void AddGeneralPredecessor(GroundedClause gc)
        {
            if (gc.clauseId == -1)
            {
                Debug.WriteLine("ERROR: id is -1: " + gc.ToString());
            }
            Utilities.AddUnique(generalPredecessors, gc.clauseId);
        }
        public void AddRelationPredecessors(List<int> preds)
        {
            foreach (int pred in preds)
            {
                if (pred == -1)
                {
                    Debug.WriteLine("ERROR: id is -1: " + pred);
                }
                Utilities.AddUnique(relationPredecessors, pred);
            }
        }
        public void AddGeneralPredecessors(List<int> preds)
        {
            foreach (int pred in preds)
            {
                if (pred == -1)
                {
                    Debug.WriteLine("ERROR: id is -1: " + pred);
                }
                Utilities.AddUnique(generalPredecessors, pred);
            }
        }

        private bool axiomatic;
        public bool IsAxiomatic() { return axiomatic; }
        public void MakeAxiomatic() { axiomatic = true; mayBeSourceNode = false; }
        public virtual bool IsAlgebraic() { return false; } // Bydefault we will say a node is geometric
        public virtual bool IsGeometric() { return true; }  //  and not algebraic
        public virtual bool IsReflexive() { return false; }
        public virtual bool Strengthened() { return false; }

        public GroundedClause()
        {
            justification = "";
            multiplier = 1;
            clauseId = -1;
            axiomatic = false;
            generalPredecessors = new List<int>();
            relationPredecessors = new List<int>();
            figureComponents = new List<int>();
        }

        // The justification for when a node is deduced
        protected string justification;
        public string GetJustification() { return justification; }
        public void SetJustification(string j) { justification = j; }

        //
        // For equation simplification
        //
        public int multiplier { get; set; }
        public virtual List<GroundedClause> CollectTerms()
        {
            return new List<GroundedClause>(Utilities.MakeList<GroundedClause>(this));
        }

        public override bool Equals(object obj)
        {
            GroundedClause that = obj as GroundedClause;
            if (that == null) return false;
            return multiplier == that.multiplier; // && clauseId == that.clauseId;
        }

        public virtual bool StructurallyEquals(object obj) { return false; }

        //
        // For subsitution and algebraic Simplifications
        //
        public virtual bool ContainsClause(GroundedClause clause) { return false; }
        public virtual void Substitute(GroundedClause c1, GroundedClause c2) { }
        public virtual GroundedClause DeepCopy() { return (GroundedClause)this.MemberwiseClone(); }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override abstract string ToString();

        public virtual String ToPrettyString() { return ToString(); }
    }
}
