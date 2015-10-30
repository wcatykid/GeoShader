using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

//placeholder, perpendicular intersections not correctly implemented yet

namespace GeometryTutorLib.GenericInstantiator
{


    public class PerpendicularParallelTransversal : Theorem
    {
        private readonly static string NAME = "Perpendicular traversal of parallel segments";

        public PerpendicularParallelTransversal() { }

        private static List<Intersection> candIntersection = new List<Intersection>(); //All intersections found
        private static List<Parallel> candParallel = new List<Parallel>();  //All parallel sets found
        private static List<Perpendicular> candPerpendicular = new List<Perpendicular>();  //All parallel sets found

        private static List<GroundedClause> antecedent;
        


      
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            
            //Exit if c is neither a parallel set nor an intersection
            if (!(c is Parallel) && !(c is Intersection)) return new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            List<Intersection> foundCand = new List<Intersection>(); //Variable holding intersections that will used for theorem

            // The list of new grounded clauses if they are deduced
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (c is Parallel)
            {
                Parallel newParallel = (Parallel)c;
                candParallel.Add((Parallel)c);

                //Create a list of all segments in the intersection list by individual segment and list of intersecting segments
                var query1 = candIntersection.GroupBy(m => m.lhs, m => m.rhs).Concat(candIntersection.GroupBy(m => m.rhs, m => m.lhs));

                //Iterate through all segments intersected by each key segment
                foreach (var group in query1)
                {
                    if (group.Contains(newParallel.segment1) && group.Contains(newParallel.segment2))
                    {
                        //If a segment that intersected both parallel lines was found, find the intersection objects.  
                        var query2 = candIntersection.Where(m => m.lhs.Equals(group.Key)).Concat(candIntersection.Where(m => m.rhs.Equals(group.Key)));
                        var query3 = query2.Where(m => m.lhs.Equals(newParallel.segment1) || m.lhs.Equals(newParallel.segment2) || m.rhs.Equals(newParallel.segment1) || m.rhs.Equals(newParallel.segment2));
                        if (query3.Any(m => m.isPerpendicular == true) && query3.Any(m => m.isPerpendicular == false))
                        {
                            //if there exists both an intersection that is labeled perpendicular and an intersection that is not labeled perpendicular
                            foundCand.AddRange(query3);
                        }
                        antecedent = Utilities.MakeList<GroundedClause>(newParallel); //Add parallel set to antecedents
                        
                    }
                }

            }
            else if (c is Intersection)
            {

                candIntersection.Add((Intersection)c);
                Intersection newintersect = (Intersection)c;

                var query1 = candIntersection.GroupBy(m => m.lhs, m => m.rhs).Concat(candIntersection.GroupBy(m => m.rhs, m => m.lhs));

                foreach (Parallel p in candParallel)
                {
                    foreach (var group in query1)
                    {
                        if (group.Contains(p.segment1) && group.Contains(p.segment2))
                        {
                            //list intersections involving intersecting segement and two paralell segments 
                            var query2 = candIntersection.Where(m => m.lhs.Equals(group.Key)).Concat(candIntersection.Where(m => m.rhs.Equals(group.Key)));
                            var query3 = query2.Where(m => m.lhs.Equals(p.segment1) || m.lhs.Equals(p.segment2) || m.rhs.Equals(p.segment1) || m.rhs.Equals(p.segment2));

                        if (query3.Any(m => m.isPerpendicular == true) && query3.Any(m => m.isPerpendicular == false))
                        {
                            //if there exists both an intersection that is labeled perpendicular and an intersection that is not labeled perpendicular
                            foundCand.AddRange(query3);
                        }

                            antecedent = Utilities.MakeList<GroundedClause>(p);
                           
                        }
                    }
                }

            }

            
            //TODO: Make sure there will only be one set of intersections found at a time
            if (foundCand.Count() > 1)
            {
                antecedent.AddRange((IEnumerable<GroundedClause>)(foundCand));  //Add the two intersections to antecedent
                
                int index;

                index = (foundCand[0].isPerpendicular == false) ? 0 : 1;
                foundCand[index].setPerpendicular(true);
                Perpendicular newPerpendicular = new Perpendicular(foundCand[index].lhs,foundCand[index].rhs, NAME);
                

                //Add the new perpendicular set
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newPerpendicular));
            
            }

            return newGrounded;
        }
    }
}