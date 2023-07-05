using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Novels.Models
{
    public partial class User
    {
        public User()
        {
            Authors = new HashSet<Author>();
            RefreshToknes = new HashSet<RefreshTokne>();
        }

        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public int? Age { get; set; }
        public int? UserType { get; set; }
        public string? Password { get; set; }
        [NotMapped]
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingDefault)]
        public bool isAdmin
        {
            get
            {
                if (UserTypeNavigation == null || UserTypeNavigation.UserType != "admin")
                {
                    return false;
                }
                return true;
            }
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public virtual Type? UserTypeNavigation { get; set; }
        public virtual Userdetail? Userdetail { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public virtual ICollection<RefreshTokne> RefreshToknes { get; set; }
    }
}
