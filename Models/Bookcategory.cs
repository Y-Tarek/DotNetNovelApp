using System;
using System.Collections.Generic;
using Novels.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Novels.Models
{
    public partial class Bookcategory :IHasTimeStamp
    {
        public int Id { get; set; }
        public int BooKId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public virtual Book? BooK { get; set; }
        
        public virtual Category? Category { get; set; }

        public void DoTimeStamp()
        {

            NovelStoreContext dataContext = new NovelStoreContext();

            if (dataContext.Entry<Bookcategory>(this).State == EntityState.Added)
            {
                //update Updation time            
                Console.WriteLine("Added");
            }
        }
        
    }
}
