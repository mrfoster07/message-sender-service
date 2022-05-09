using MessageSenderServiceApi.Domain.Modules.Notification.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessageSenderServiceApi.Infrastructure.Data.DataContext;

public class NotificationDataContext : DbContext
{
    public NotificationDataContext(DbContextOptions<NotificationDataContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationEntity> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<NotificationEntity>().HasKey(s => s.Id);

        builder.Entity<NotificationEntity>().Property(s => s.Json).IsRequired();
    }
}