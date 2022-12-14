using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplicationAPI.Models
{
    public partial class ToDoListContext : DbContext
    {
        public ToDoListContext()
        {
        }

        public ToDoListContext(DbContextOptions<ToDoListContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ToDoListTable> ToDoListTables { get; set; } = null!;
        public virtual DbSet<UserLoginTable> UserLoginTables { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=ToDoList;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoListTable>(entity =>
            {
                entity.ToTable("ToDoListTable");
            });

            modelBuilder.Entity<UserLoginTable>(entity =>
            {
                entity.ToTable("UserLoginTable");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.UserApproved)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.UserName).HasMaxLength(20);

                entity.Property(e => e.UserPassword).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
