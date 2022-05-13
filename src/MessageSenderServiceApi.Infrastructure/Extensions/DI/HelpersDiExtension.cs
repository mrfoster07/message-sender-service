using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Providers;
using MessageSenderServiceApi.Infrastructure.Data.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageSenderServiceApi.Infrastructure.Extensions.DI
{
    public static class HelpersDiExtension
    {
        public static IServiceCollection AddHelpers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStringHashHelper, StringHashHelper>();

            return serviceCollection;
        }
    }
}