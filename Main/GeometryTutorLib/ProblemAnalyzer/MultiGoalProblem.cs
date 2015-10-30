using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // A problem is defined as a the sub-hypergraph from a set of source nodes to a goal node
    //
    public class MultiGoalProblem<A>
    {
        public List<int> givens { get; private set; }
        public List<int> goals { get; private set; }

        public List<Problem<A>> problems { get; private set; }

        public MultiGoalProblem()
        {
            givens = new List<int>();
            goals = new List<int>();
            problems = new List<Problem<A>>();
        }

        public MultiGoalProblem(MultiGoalProblem<A> thatProblem)
        {
            givens = new List<int>(thatProblem.givens);
            goals = new List<int>(thatProblem.goals);
            problems = new List<Problem<A>>(thatProblem.problems);
        }


        public bool AddProblem(Problem<A> problem)
        {

            // Don't allow addition of the same problem.
            if (Contains(problem)) return false;

            // We don't want a new problem with a goal we already have.
            if (this.goals.Contains(problem.goal)) return false;

            //
            // Only add if the new problem adds an assumption to the mix
            //
            bool newGiven = false;
            foreach (int thatGiven in problem.givens)
            {
                if (!this.givens.Contains(thatGiven))
                {
                    newGiven = true;
                    break;
                }
            }
            if (!newGiven) return false;

            Utilities.AddUniqueList<int>(givens, problem.givens);
            Utilities.AddUnique<int>(goals, problem.goal);

            problems.Add(problem);

            return true;
        }

        public bool Contains(Problem<A> thatProblem)
        {
            if (!problems.Any()) return false;

            return problems.Contains(thatProblem);
        }

        //
        // Problems are equal only if the givens and goals are the same
        //
        public bool HasSameGivensGoals(MultiGoalProblem<A> thatProblem)
        {
            if (this.givens.Count != thatProblem.givens.Count) return false;

            if (this.goals.Count != thatProblem.goals.Count) return false;

            if (!Utilities.EqualSets<int>(this.givens, thatProblem.givens)) return false;

            if (!Utilities.EqualSets<int>(this.goals, thatProblem.goals)) return false;

            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }
        
        //
        // Problems are equal only if the givens and goals are the same
        //
        public override bool Equals(object obj)
        {
            MultiGoalProblem<A> thatProblem = obj as MultiGoalProblem<A>;

            if (thatProblem == null) return false;

            if (this.givens.Count != thatProblem.givens.Count) return false;

            if (this.goals.Count != thatProblem.goals.Count) return false;

            // Union the sets; if the union is the same size as the original, they are the same
            List<int> union = new List<int>(this.givens);
            Utilities.AddUniqueList<int>(union, thatProblem.givens);
            if (union.Count != this.givens.Count) return false;

            union = new List<int>(this.goals);
            Utilities.AddUniqueList<int>(union, thatProblem.goals);
            if (union.Count != this.goals.Count) return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Given { ");
            foreach (int g in givens)
            {
                str.Append(g + " ");
            }
            str.Append("} ");

            //foreach (int p in path)
            //{
            //    str.Append(p + " ");
            //}
            //str.Append("} -> " + goal);

            str.Append("Goals { ");
            foreach (int g in goals)
            {
                str.Append(g + " ");
            }
            str.Append("}");

            return str.ToString();
        }
    }
}