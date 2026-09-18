using Microsoft.EntityFrameworkCore;

namespace AskalePortal.Data.Models;

public partial class DBDataContext
{

    private static void ConfigureActiveProcessExtras(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveProcessChecks>(entity =>
        {
            entity.ToTable("ActiveProcessChecks");

            // The existing application model intentionally uses nullable/lower-case `id`.
            // Keep that contract intact and map it to the database identity column `Id`.
            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.HasKey(e => e.Id)
                .HasName("PK__ActivePr__3214EC073C91FC38");

            entity.Property(e => e.createdDate)
                .HasColumnType("datetime");

            entity.Property(e => e.enabled)
                .IsRequired();

            entity.Property(e => e.updatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");

            entity.Property(e => e.belnr)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.kunnr)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.name1)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.netdt)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ActiveProcessInvoice>(entity =>
        {
            entity.ToTable("ActiveProcessInvoice");

            // Same compatibility mapping as ActiveProcessChecks: preserve the
            // application's existing entity contract while mapping the real DB columns.
            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasColumnName("Id");

            entity.HasKey(e => e.Id)
                .HasName("PK__ActivePr__3214EC073FFDC4C5");

            entity.Property(e => e.createdDate)
                .HasColumnType("datetime");

            entity.Property(e => e.enabled)
                .IsRequired();

            entity.Property(e => e.updatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");

            entity.Property(e => e.belnr)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.bldat)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.bukrs)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.dagitimkanali)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.faedt)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.zterm)
                .HasMaxLength(255)
                .IsUnicode(false);
        });
    }
}
