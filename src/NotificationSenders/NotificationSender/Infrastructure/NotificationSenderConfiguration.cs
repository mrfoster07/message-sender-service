using Microsoft.Extensions.DependencyInjection;
using NotificationSender.Domain;

namespace NotificationSender.Infrastructure
{
    public sealed class NotificationSenderConfiguration
    {
        private readonly IServiceCollection serviceCollection;

        private readonly ProxySendersModel _proxySendersModel;

        public NotificationSenderConfiguration(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
            this._proxySendersModel = new ProxySendersModel();
        }

        public void Bind<TService, TImplementation>()
        {
            var attributes = Attribute
                .GetCustomAttributes(typeof(TImplementation))
                .OfType<NotificationSenderTitleAttribute>().FirstOrDefault();

            if (attributes != null
                && attributes.Name.Length > 0
                && !_proxySendersModel.NotificationSenders.ContainsKey(attributes.Name))
            {
                _proxySendersModel.NotificationSenders.Add(attributes.Name, typeof(TService));
            } else
            {
                throw new ArgumentException(
                    $"Type of {typeof(TImplementation)} has no attribute {nameof(NotificationSenderTitleAttribute)}");
            }
        }

        public void AddProvidersProxy()
        {
            serviceCollection.AddSingleton(_proxySendersModel);
        }

        public void AddProvider(Action<IServiceCollection> func) => func(serviceCollection);
    }
}