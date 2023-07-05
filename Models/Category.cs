using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Novels.Models
{
    public partial class Category
    {
        public Category()
        {
            Bookcategories = new HashSet<Bookcategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Bookcategory> Bookcategories { get; set; }
    }
}
