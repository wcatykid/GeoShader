using System;
using System.Collections.Generic;

namespace GeometryTutorLib.EngineUIBridge
{
    /// <summary>
    /// This class is used to represent a mathematical assumption (axiom, definition, or theorem) that the user can enable or disable.
    /// </summary>
    public class Assumption
    {
        /// <summary>
        /// Determines if an Assumption is an axiom, a definition, or a theorem.
        /// </summary>
        public enum AssumptionType { Axiom = 0, Definition, Theorem };

        public String Name { get; private set; }
        public AssumptionType Type { get; private set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// All assumptions that are recognized by the back end.
        /// </summary>
        private static Dictionary<EngineUIBridge.JustificationSwitch.DeductionJustType, Assumption> assumptions = null;

        /// <summary>
        /// Create a new Assumption.
        /// </summary>
        /// <param name="name">The name of the Assumption (such as Segment Addition).</param>
        /// <param name="type">The type of this Assumption.</param>
        public Assumption(String name, AssumptionType type)
        {
            this.Name = name;
            this.Type = type;
            Enabled = true;
        }

        /// <summary>
        /// Return the list of all possible Assumptions. If the list does not yet exist it will be created.
        /// </summary>
        /// <returns>A list of all assumptions.</returns>
        public static Dictionary<EngineUIBridge.JustificationSwitch.DeductionJustType, Assumption> GetAssumptions()
        {
            if (assumptions == null)
            {
                assumptions = EngineUIBridge.JustificationSwitch.GetAssumptions();
            }
            return assumptions;
        }

        public override string ToString()
        {
            string assumptionType;
            switch (Type)
            {
                case Assumption.AssumptionType.Axiom:
                    assumptionType = "Axiom";
                    break;
                case Assumption.AssumptionType.Definition:
                    assumptionType = "Defintion";
                    break;
                case Assumption.AssumptionType.Theorem:
                    assumptionType = "Theorem";
                    break;
                default:
                    assumptionType = "";
                    break;
            }
            return assumptionType + ": " + Name;
        }
    }
}
