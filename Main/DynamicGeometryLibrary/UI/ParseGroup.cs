using System.Collections.Generic;
using GeometryTutorLib.EngineUIBridge;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A group of related Assumptions.
    /// </summary>
    public class ParseGroup
    {
        private static List<ParseGroup> Groups = null;

        public List<Assumption> Assumptions { get; set; }
        public string Name { get; set; }
        public bool Predefined { get; private set; }

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="Name">The name of the group.</param>
        private ParseGroup(string Name)
        {
            this.Name = Name;
            Assumptions = new List<Assumption>();
            Groups.Add(this);
            Predefined = false;
        }

        /// <summary>
        /// Add a new ParseGroup.
        /// </summary>
        /// <param name="Name">The name of the group.</param>
        /// <returns>The new group.</returns>
        public static ParseGroup AddParseGroup(string Name)
        {
            if (Groups == null)
            {
                CreatePredefGroups();
            }
            return new ParseGroup(Name);
        }

        /// <summary>
        /// Add a new ParseGroup.
        /// </summary>
        /// <param name="Name">The name of the group.</param>
        /// <param name="Assumptions">A list of assumptions to add to the group.</param>
        /// <returns>The new group.</returns>
        public static ParseGroup AddParseGroup(string Name, List<Assumption> Assumptions)
        {
            ParseGroup pg = AddParseGroup(Name);
            pg.Assumptions.AddRange(Assumptions);
            return pg;
        }

        /// <summary>
        /// Get all the current groups. Will create predefined groups if no groups exist yet.
        /// </summary>
        /// <returns>A list of all current parse groups.</returns>
        public static List<ParseGroup> GetParseGroups()
        {
            if (Groups == null)
            {
                CreatePredefGroups();
            }
            return Groups;
        }

        /// <summary>
        /// Remove the given ParseGroup as long as it is not predefined.
        /// </summary>
        /// <param name="pg">The ParseGroup to remove.</param>
        public static void RemoveGroup(ParseGroup pg)
        {
            if (!pg.Predefined)
            {
                Groups.Remove(pg);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Create the predefined groups.
        /// </summary>
        private static void CreatePredefGroups()
        {
            Groups = new List<ParseGroup>();
            List<Assumption> assumptions = new List<Assumption>(Assumption.GetAssumptions().Values);

            //All assumptions
            ParseGroup pg = new ParseGroup("All");
            pg.Predefined = true;
            pg.Assumptions.AddRange(assumptions);

            //Axioms
            pg = new ParseGroup("Axioms");
            pg.Predefined = true;
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Axiom)
                {
                    pg.Assumptions.Add(a);
                }
            }

            //Definitions
            pg = new ParseGroup("Definitions");
            pg.Predefined = true;
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Definition)
                {
                    pg.Assumptions.Add(a);
                }
            }

            //Theorems
            pg = new ParseGroup("Theorems");
            pg.Predefined = true;
            foreach (Assumption a in assumptions)
            {
                if (a.Type == Assumption.AssumptionType.Theorem)
                {
                    pg.Assumptions.Add(a);
                }
            }
        }
    }
}
