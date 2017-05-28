using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Example.Data.Entities
{
    [DataContract]
    public class User : Base
    {
        [DataMember]
        public string Username { get; set; }

        public string Name { get; set; }

        public int? test { get; set; }

        public virtual List<Content> Contents { get; set; }

        public virtual List<Setting> Settings { get; set; }
    }
}
