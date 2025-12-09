using Microsoft.EntityFrameworkCore;
using System;

namespace Safety_Equipment_Control
{
    // 1. The Database Manager
    public class SafetyContext : DbContext
    {
        public DbSet<EquipmentItem> EquipmentInventory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection String for SQL Express
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=SafetyDB;Integrated Security=True;TrustServerCertificate=True;");
        }
    }

    // 2. The Data Model
    public class EquipmentItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        public int Quantity { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastIssueDate { get; set; }
        public string Status { get; set; }
    }
}