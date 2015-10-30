using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // This class represents a descriptor applied to an edge of the hypergraph.
    // Each axiom, theorem, and definition will have ONE static instance of this class.
    //
    public class EdgeAnnotation
    {
        // The string version of the reason that the edge was created.
        public string justification { get; private set; }

        // Has the user indicated that the use of this 
        public bool active;
        
        public EdgeAnnotation()
        {
            justification = "";
            active = false;
        }

        public EdgeAnnotation(string just, bool active)
        {
            justification = just;
            this.active = active;
        }

        public bool IsActive() { return active; }

        public override string ToString()
        {
            return justification;
        }
    }
}
