using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Novels.Models
{
    public partial class Book
    {
        public Book()
        {
            Bookcategories = new HashSet<Bookcategory>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Speciality { get; set; } = null!;
        public double Price { get; set; }
        public int? AuthorId { get; set; }

        public virtual Author? Author { get; set; }
        public virtual ICollection<Bookcategory> Bookcategories { get; set; }
    }
}
