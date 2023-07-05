using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Novels.Models
{
    public partial class Userdetail
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        [JsonIgnore]
        public virtual User IdNavigation { get; set; } = null!;
    }
}
