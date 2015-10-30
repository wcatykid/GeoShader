using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;


namespace GeometryTutorLib.GenericInstantiator
{


    public class ParallelSegmentsTransitivity : Theorem
    {
        private readonly static string NAME = "Parallel segments relation";

        public ParallelSegmentsTransitivity() { }

        private static List<Parallel> candParallel = new List<Parallel>();  //All parallel sets found

        private static List<GroundedClause> antecedent;
        

        
        // Parallel(Segment(A, B), Segment(C, D))  && Parallel(Segment(A, B), Segment(E, F) -> 
        //                                              Parallel(Segment(C, D), Segment(E, F)
        //                                               
        
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            
            //Exit if c is not parallel
            if (!(c is Parallel)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<Parallel> foundCand = new List<Parallel>(); //Variable holding parallel relations that will used for theorem

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Parallel)
            {
                Parallel newParallel = (Parallel)c;
                candParallel.Add((Parallel)c);

                //Create a list of all segments part of a parallel set grouped by what other segments they are parallel to
                var query1 = candParallel.GroupBy(m => m.segment1, m => m.segment2).Concat(candParallel.GroupBy(m => m.segment1, m => m.segment2));

                //Iterate through the groups of parallel relations
                foreach (var group in query1)
                {
                    foreach (ConcreteSegment segment in group)
                    {
                        //var query2 = candParallel.Where(m => m.segment1 == 

                        //Add two parallel sets leading to third parallel set
                        //foundCand.Add()
                    }

                }
            }


            

            

            
            if (foundCand.Count() >= 1)
            {
                antecedent.AddRange((IEnumerable<GroundedClause>)(foundCand));  //Add the two intersections to antecedent
                Parallel newParallel;
                //new Parallel()
                //newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newParallel));
            
            }

            return newGrounded;
        }
    }
}