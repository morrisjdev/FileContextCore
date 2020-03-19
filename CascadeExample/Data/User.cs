using System.Collections.Generic;

namespace CascadeExample.Data
{
    public class User : Base
    {
        public string Name { get; set; }
        
        public virtual List<Entry> Entries { get; set; }
    }
}