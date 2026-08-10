using System;
using System.Collections.Generic;

namespace QueueLess.Domain.Entities
{
    public class BusinessCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Business> Businesses { get; set; } = new List<Business>();
    }
}
