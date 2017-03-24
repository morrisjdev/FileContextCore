using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Data.Entities
{
    public class Messurement : Base
    {
        public TimeSpan TimeRead { get; set; }

        public TimeSpan TimeWrite { get; set; }

        public int EntryCount { get; set; }
    }
}
