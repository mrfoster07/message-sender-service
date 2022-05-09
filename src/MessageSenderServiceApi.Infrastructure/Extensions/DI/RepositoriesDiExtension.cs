using Microsoft.Extensions.DependencyInjection;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class RepositoriesDiExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}