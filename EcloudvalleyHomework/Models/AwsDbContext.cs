using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EcloudvalleyHomework.Models
{
    public partial class AwsDbContext : DbContext
    {
        public AwsDbContext()
        {
        }

        public AwsDbContext(DbContextOptions<AwsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<UsageReport> UsageReports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsageReport>(entity =>
            {
                entity.ToTable("usage_report");

                entity.Property(e => e.UsageReportId).HasColumnName("usage_report_id");

                entity.Property(e => e.PayerAccountId)
                    .HasColumnType("decimal(13, 0)")
                    .HasColumnName("payer_account_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("product_name");

                entity.Property(e => e.UnblendedCost)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("unblended_cost");

                entity.Property(e => e.UnblendedRate)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("unblended_rate");

                entity.Property(e => e.UsageAccountId)
                    .HasColumnType("decimal(13, 0)")
                    .HasColumnName("usage_account_id");

                entity.Property(e => e.UsageAmount)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("usage_amount");

                entity.Property(e => e.UsageEndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("usage_end_date");

                entity.Property(e => e.UsageStartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("usage_start_date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
