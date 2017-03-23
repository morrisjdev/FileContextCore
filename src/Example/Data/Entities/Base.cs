using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Data.Entities
{
    public class Base
    {
        public Base()
        {
            //Id = Guid.NewGuid();
            CreatedOn = DateTime.UtcNow;
        }

        [Key]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
