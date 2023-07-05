using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Novels.Models
{
    public partial class Type
    {
        public Type()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string UserType { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
