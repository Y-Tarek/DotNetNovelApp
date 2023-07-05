using System;
using System.Linq; 
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Novels.Interfaces;

namespace Novels.Models
{
    public partial class NovelStoreContext : DbContext
    {
        public NovelStoreContext()
        {
        }

        public NovelStoreContext(DbContextOptions<NovelStoreContext> options)
            : base(options)
        {
            //Pre Save Signal
            SavingChanges += (sender, args) =>
            {
                foreach (var entityEntry in ChangeTracker.Entries())
                {
                    var t1 = entityEntry.Entity.GetType().GetProperty("UpdatedAt");
                    Console.WriteLine("Has Or not" + " " + t1);

                    if (entityEntry.Entity is Bookcategory bookcategory)
                    {
                        
                        if (entityEntry.State == EntityState.Modified)
                        {
                            Console.WriteLine("Firessss");
                            bookcategory.UpdatedAt = DateTime.Now;
                        }

                    }
                    //if (entityEntry.Entity is User user)
                    //{
                    //    if (entityEntry.State == EntityState.Added)
                    //    {
                    //        // Id is 0 This Fires before Hitting database
                    //        Console.WriteLine("UserId" + " " + user.Id);
                            
                    //    }

                    //}


                }
            };
            //Post Save Signal
            SavedChanges += (sender, args) =>
            {
                foreach (var entityEntry in ChangeTracker.Entries())
                {
                    if (entityEntry.Entity is User user)
                    {
                        Console.WriteLine("Fires1111" + " " + entityEntry.State);

                        Console.WriteLine("Fires2222" + " " + user.Id);

                    }
                }
            };
        }

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Bookcategory> Bookcategories { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<RefreshTokne> RefreshToknes { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Userdetail> Userdetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=NovelStoresDB");
            }
        }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Author");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Authors)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Author__user_id__4316F928");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuthorId).HasColumnName("author_id");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Speciality)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("speciality");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK__Book__author_id__440B1D61");
            });

            modelBuilder.Entity<Bookcategory>(entity =>
            {
                entity.ToTable("bookcategory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BooKId).HasColumnName("booK_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.BooK)
                    .WithMany(p => p.Bookcategories)
                    .HasForeignKey(d => d.BooKId)
                    .HasConstraintName("FK__bookcateg__booK___5441852A");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Bookcategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__bookcateg__categ__5535A963");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<RefreshTokne>(entity =>
            {
                entity.ToTable("RefreshTokne");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.Token)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshToknes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefreshTo__user___46E78A0C");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("Type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserType)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("user_type");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("full_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.UserType).HasColumnName("user_type");

                entity.HasOne(d => d.UserTypeNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserType)
                    .HasConstraintName("FK__users__user_type__4222D4EF");
            });

            modelBuilder.Entity<Userdetail>(entity =>
            {
                entity.ToTable("userdetails");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .HasColumnName("address");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Userdetail)
                    .HasForeignKey<Userdetail>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_userdetails");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        // Save Method
        public override int SaveChanges()
        {
            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.Entity is User user)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        Console.WriteLine("Fires");

                    }
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            foreach (var entityEntry in ChangeTracker.Entries())
            {
                if (entityEntry.Entity is User user)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        //Will not work as instance not yet in db so no id
                        Console.WriteLine("Fires2222" + " " + user.Id);

                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }


    }
}
