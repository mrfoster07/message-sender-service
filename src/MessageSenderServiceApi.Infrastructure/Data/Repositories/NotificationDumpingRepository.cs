using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Domain.Modules.NotificationDump;
using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using MessageSenderServiceApi.Infrastructure.Data.DataContext.NotificationEntities;
using Microsoft.EntityFrameworkCore;

namespace MessageSenderServiceApi.Infrastructure.Data.Repositories
{
    public class NotificationDumpingRepository : INotificationDumpingRepository
    {
        private readonly NotificationDataContext dataContext;

        public NotificationDumpingRepository(NotificationDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task AddRange((Guid id, string json, string jsonHash, bool status)[] items,
            CancellationToken cancellationToken)
        {
            await dataContext.Notifications.AddRangeAsync(
                items.Select(s => new NotificationEntity
                    { Id = s.id, IsDelivered = s.status, Json = s.json, JsonHash = s.jsonHash }),
                cancellationToken
            );

            await dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}