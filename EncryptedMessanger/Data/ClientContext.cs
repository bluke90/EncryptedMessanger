using EncryptedMessanger.ClientNet.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedMessanger.Data
{
    public class ClientContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Packet> Packets { get; set; }
        public DbSet<EncryptedMessanger.Modules.Contact> Contacts { get; set; }
        public ClientContext() {
            SQLitePCL.Batteries_V2.Init();

            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "dataContext.db3");

            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
