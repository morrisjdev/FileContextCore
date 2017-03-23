using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Data.Entities
{
    public class User : Base
    {
        public string Username { get; set; }

        public string Name { get; set; }

        public virtual List<Content> Contents { get; set; }

        public virtual List<Setting> Settings { get; set; }
    }
}
