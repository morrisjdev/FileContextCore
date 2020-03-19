using System.ComponentModel.DataAnnotations.Schema;

namespace CascadeExample.Data
{
    public class Entry : Base
    {
        public string Content { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}