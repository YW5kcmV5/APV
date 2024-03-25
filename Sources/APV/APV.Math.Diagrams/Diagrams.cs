using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace APV.Math.Diagrams
{
    public class Diagrams : List<Diagram>
    {
        public Diagram this[string name]
        {
            get { return this.FirstOrDefault(item => string.Compare(item.Name, name, true, CultureInfo.InvariantCulture) == 0); }
        }

        public string[] Names
        {
            get { return this.Select(item => item.Name).ToArray(); }
        }
    }
}
