using Microsoft.EntityFrameworkCore;
using realtime_game.Shared.Models.Entities;

namespace realtime_game.Server.Models.Contexts
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        //テーブル追加(エンティティ追加)したらここに追加していく

#if DEBUG
        readonly string connectionString = "server=localhost;port=3306;database=realtime_game;user=jobi;password=jobi;";//realtime_game
# else
        readonly string connectionString = "server=db-ge0202400.mysql.database.azure.com;port=3306;database=realtime_game241207;user=student;password=Yoshidajobi2024;SslMode=Required;";
# endif

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,
new MySqlServerVersion(new Version(8, 0)));
        }

    }

}
