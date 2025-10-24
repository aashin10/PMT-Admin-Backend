using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class DeliveryUnitSeedData
    {
        public static List<DeliveryUnit> GetDeliveryUnits()
        {
            return new List<DeliveryUnit>
            {
                new DeliveryUnit { Id = 1, Name = "Engineering", Code = "ENG", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 2, Name = "Product Development", Code = "PD", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 3, Name = "Digital Services", Code = "DS", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 4, Name = "Cloud Solutions", Code = "CS", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 5, Name = "Data Analytics", Code = "DA", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 6, Name = "Enterprise Systems", Code = "ES", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 7, Name = "Mobile Solutions", Code = "MS", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DeliveryUnit { Id = 8, Name = "Infrastructure", Code = "INF", IsActive = true, CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            };
        }

        public static void SeedDeliveryUnits(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryUnit>().HasData(GetDeliveryUnits());
        }
    }
}
