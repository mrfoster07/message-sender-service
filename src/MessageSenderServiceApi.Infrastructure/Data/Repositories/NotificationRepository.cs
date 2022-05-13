using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using MessageSenderServiceApi.Infrastructure.Data.DataContext.NotificationEntities;
using Microsoft.EntityFrameworkCore;

namespace MessageSenderServiceApi.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDataContext dataContext;

        public NotificationRepository(NotificationDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task Add((Guid id, string json, string jsonHash, bool status) item,
            CancellationToken cancellationToken)
        {
            await dataContext.Notifications.AddAsync(new NotificationEntity
                    { Id = item.id, IsDelivered = item.status, Json = item.json, JsonHash = item.jsonHash },
                cancellationToken
            );

            await dataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsStatus(Guid id, bool status, CancellationToken cancellationToken)
        {
            return await dataContext.Notifications
                .CountAsync(s => s.Id == id && s.IsDelivered == status, cancellationToken) > 0;
        }
    }
}