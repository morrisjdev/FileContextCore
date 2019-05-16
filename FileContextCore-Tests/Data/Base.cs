using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileContextCore_Tests.Data
{
    public class Base
    {
        public Base()
        {
            Id = Guid.NewGuid();
            CreatedOn = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
