using System.ComponentModel.DataAnnotations;

namespace CascadeExample.Data
{
    public class Base
    {
        [Key]
        public int Id { get; set; }
    }
}