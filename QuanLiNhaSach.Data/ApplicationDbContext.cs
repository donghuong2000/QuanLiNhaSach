using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLiNhaSach.Models;

namespace QuanLiNhaSach.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<BookEntryTicket> BookEntryTickets { get; set; }
        public DbSet<BookEntryTicketDetail> BookEntryTicketDetails { get; set; }
        public DbSet<BookExistDetail> BookExistDetails { get; set; }
        public DbSet<BookExistHeader> BookExistHeaders { get; set; }

        public DbSet<DebitDetail> DebitDetails { get; set; }
        public DbSet<DebitHeader> DebitHeaders { get; set; }
        public DbSet<Receipt> Receipts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(450);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FullName).HasMaxLength(450);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.Property(e => e.ApplicationUserId).HasMaxLength(450);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.StaffId).HasMaxLength(450);

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.BillApplicationUser)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .HasConstraintName("FK_Bill_APPUSER");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.BillStaff)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_Bill_staff");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => new { e.BillId, e.BookId })
                    .HasName("PK__BillDeta__722CF04AE6C856BA");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetail)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_Bill");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BillDetail)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillDetail_Book");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Author).HasMaxLength(200);

                entity.Property(e => e.CategoryId).HasMaxLength(450);

                entity.Property(e => e.DatePublish).HasColumnType("date");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Book_Category");
            });

            modelBuilder.Entity<BookEntryTicket>(entity =>
            {
                entity.Property(e => e.DateEntry).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookEntryTicketDetail>(entity =>
            {
                entity.HasKey(e => new { e.BookEntryTicketId, e.BookId })
                    .HasName("PK__BookEntr__E356823B5A98F79F");

                entity.Property(e => e.Author).HasMaxLength(200);

                entity.Property(e => e.CategoryId).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.HasOne(d => d.BookEntryTicket)
                    .WithMany(p => p.BookEntryTicketDetail)
                    .HasForeignKey(d => d.BookEntryTicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookEntryTicketDetail_BookEntryTicket");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookEntryTicketDetail)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookEntryTicketDetail_Book");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.BookEntryTicketDetail)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_BookEntryTicketDetail_Category");
            });

            modelBuilder.Entity<BookExistDetail>(entity =>
            {
                entity.Property(e => e.BookExistHeaderId).HasMaxLength(450);

                entity.Property(e => e.TimeRecord).HasColumnType("datetime");

                entity.HasOne(d => d.BookExistHeader)
                    .WithMany(p => p.BookExistDetail)
                    .HasForeignKey(d => d.BookExistHeaderId)
                    .HasConstraintName("FK_BookExistDetail_BookExistHeader");
            });

            modelBuilder.Entity<BookExistHeader>(entity =>
            {
                entity.Property(e => e.BookId).HasMaxLength(450);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookExistHeader)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_BookExistHeader_Book");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<DebitDetail>(entity =>
            {
                entity.Property(e => e.DebitHeaderId).HasMaxLength(450);

                entity.Property(e => e.TimeRecord).HasColumnType("datetime");

                entity.HasOne(d => d.DebitHeader)
                    .WithMany(p => p.DebitDetail)
                    .HasForeignKey(d => d.DebitHeaderId)
                    .HasConstraintName("FK_DebitDetail_DebitHeader");
            });

            modelBuilder.Entity<DebitHeader>(entity =>
            {
                entity.Property(e => e.ApplicationUserId).HasMaxLength(450);

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.DebitHeader)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .HasConstraintName("FK_DebitHeader_APPUSER");
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.Property(e => e.ApplicationUserId).HasMaxLength(450);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.Receipt)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .HasConstraintName("FK_Receipt_APPUSER");
            });
            base.OnModelCreating(modelBuilder);
        }

    }

}
