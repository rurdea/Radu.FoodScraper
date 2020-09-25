using System;

namespace Radu.FoodScraper.Models
{
    public class Model<TKey> where TKey:IComparable<TKey>
    {
        public TKey Id { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }
    }
}
