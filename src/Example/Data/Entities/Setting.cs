using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Example.Data.Entities
{
    public class Setting : Base
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
