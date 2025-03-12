using Microsoft.EntityFrameworkCore;

class PlayerDB : DbContext {
    public PlayerDB(DbContextOptions<PlayerDB> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();



}