using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Domain.Modules.Notification.Entities;
using MessageSenderServiceApi.Infrastructure.Data.DataContext;

namespace MessageSenderServiceApi.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDataContext _dataContext;

        public NotificationRepository(NotificationDataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public Task Add(NotificationEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStatus(Guid id, bool status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasStatus(Guid id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}