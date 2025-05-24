using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Test.Shop.Core.Entities;
using Test.Shop.Infrastructure.DAL.DataSeed;
using Test.Shop.Infrastructure.DAL.Utilities;

namespace Test.Shop.Infrastructure.DAL
{
    /// <summary>
    /// Aby wykonać komendy musimy mieć w Package Manager Console ustawiony projekt na Infrastructure.
    /// Komendy wykonują się w katalogu Migrations.
    /// </summary>

    /// <summary>
    /// Dodaje nową migrację do bazy danych.
    /// Użycie w Package Manager Console: Add-Migration NazwaMigracji -Context NazwaDbContextu
    /// </summary>

    /// <summary>
    /// Aktualizuje bazę danych do najnowszej migracji.
    /// Użycie w Package Manager Console: Update-Database -Context NazwaDbContextu
    /// </summary>

    /// <summary>
    /// Tworzy nową bazę danych na podstawie migracji.
    /// Użycie w Package Manager Console: Update-Database -Context NazwaDbContextu -Migration 0
    /// </summary>

    /// <summary>
    /// Wycofuje ostatnią migrację.
    /// Użycie w Package Manager Console: Update-Database -Context NazwaDbContextu -migration NazwaPoprzedniejMigracji
    /// </summary>

    /// <summary>
    /// Wyświetla listę dostępnych migracji.
    /// Użycie w Package Manager Console: Get-Migrations -Context NazwaDbContextu
    /// </summary>

    /// <summary>
    /// Tworzy nową bazę danych i dodaje początkowe dane.
    /// Użycie w Package Manager Console: Update-Database -Context NazwaDbContextu -Script
    /// </summary>

    /// <summary>
    /// Usuwa ostatnią migrację.
    /// Użycie w Package Manager Console: Remove-Migration -Context NazwaDbContextu
    /// </summary>
    public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IMigrationsModelDiffer, CustomMigrationsModelDiffer>();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShopDbContext).Assembly);

            modelBuilder.Entity<ShopDetails>(entity =>
            {
                entity.ToTable(nameof(ShopDetails));
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.ShopCategory)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId);

                entity.Property(e => e.CreatedDate).HasDefaultValue(DateTime.Now);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<ShopCategory>(entity =>
            {
                entity.ToTable(nameof(ShopCategory));
                entity.HasKey(e => e.IdShopCategory);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasData(ShopCategoryDictionarySeed.CategoryDictionary);
            });
        }
    }
}
