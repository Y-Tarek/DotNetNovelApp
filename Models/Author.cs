using System;
using System.Collections.Generic;

namespace Novels.Models
{
    public partial class Author
    {
        public Author()
        {
            Books = new HashSet<Book>();
        }

        public int Id { get; set; }
        public string Phone { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Address { get; set; }
        public int? UserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
