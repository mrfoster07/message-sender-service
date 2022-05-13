using FluentAssertions;
using MessageSenderServiceApi.Contracts.Notification;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationSender.Models;
using Xunit;

namespace MessageSenderServiceApi.Tests.Modules.NotificationSenders.IosProvider.NotificationSenderIosProvider
{
    public class ParseModelTests
    {
        private readonly Mock<ILogger<NotificationSender.IosProvider.NotificationSenderIosProvider>> loggerMock;


        private readonly NotificationSender.IosProvider.NotificationSenderIosProvider notificationSenderIosProvider;

        public ParseModelTests()
        {
            this.loggerMock = new Mock<ILogger<NotificationSender.IosProvider.NotificationSenderIosProvider>>();

            notificationSenderIosProvider =
                new NotificationSender.IosProvider.NotificationSenderIosProvider(loggerMock.Object);
        }

        private NotificationCreateModel PrepareNotificationIosModel() => new NotificationCreateModel
        {
            TargetType = "iOS",
            Parameters = new Dictionary<string, string>()
            {
                { "Alert", "Alert Text" },
                { "IsBackground", "true" },
                { "Priority", "110" },
                { "PushToken", "PushToken Text" },
            }
        };

        [Fact]
        public async Task ParseModel_PropertiesWithoutValues_GetDefaultParametres()
        {
            //arrange
            var inputModel = PrepareNotificationIosModel();
            inputModel.Parameters["IsBackground"] = String.Empty;
            inputModel.Parameters["Priority"] = String.Empty;

            //act
            var parsedModel = notificationSenderIosProvider.ParseModel(inputModel.Parameters);

            //assert
            parsedModel.IsBackground.Should().Be(true);
            parsedModel.Priority.Should().Be(10);
        }
    }
}