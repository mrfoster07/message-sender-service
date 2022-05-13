using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MessageSenderServiceApi.Contracts.Notification;
using MessageSenderServiceApi.Domain.Helpers;
using MessageSenderServiceApi.Domain.Modules.Notification;
using MessageSenderServiceApi.Domain.Modules.Notification.Models;
using MessageSenderServiceApi.Domain.Modules.NotificationDump;
using MessageSenderServiceApi.Domain.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationSender.Domain;
using NotificationSender.Models;
using Xunit;

namespace MessageSenderServiceApi.Tests.Modules.Notification.NotificationService
{
    public class ParseModelTests
    {
        private readonly Mock<ILogger<Domain.Modules.Notification.NotificationService>> loggerMock;
        private readonly Mock<INotificationRepository> notificationRepositoryMock;
        private readonly Mock<INotificationSenderProxy> notificationSenderProxyMock;
        private readonly Mock<IGuidProvider> guidProviderMock;
        private readonly Mock<IStringHashHelper> stringHashHelperMock;
        private readonly Mock<INotificationDumpingService> notificationDumpingServiceMock;
        private readonly Mock<IOptions<NotificationDumpingSettings>> notificationDumpingSettingsMock;


        private readonly Domain.Modules.Notification.NotificationService notificationService;

        private const string errorMessage = "Не доставлено";
        private const int notificationCreateModelSize = 101;

        public ParseModelTests()
        {
            this.loggerMock = new Mock<ILogger<Domain.Modules.Notification.NotificationService>>();
            this.notificationRepositoryMock = new Mock<INotificationRepository>();
            this.notificationSenderProxyMock = new Mock<INotificationSenderProxy>();
            this.guidProviderMock = new Mock<IGuidProvider>();
            this.stringHashHelperMock = new Mock<IStringHashHelper>();
            this.notificationDumpingServiceMock = new Mock<INotificationDumpingService>();
            notificationDumpingSettingsMock.Setup(s => s.Value)
                .Returns(new NotificationDumpingSettings());

            notificationService = new Domain.Modules.Notification.NotificationService(
                loggerMock.Object,
                notificationRepositoryMock.Object,
                notificationSenderProxyMock.Object,
                guidProviderMock.Object,
                stringHashHelperMock.Object,
                notificationDumpingServiceMock.Object,
                notificationDumpingSettingsMock.Object);
        }


        [Theory]
        [InlineData("targetType", 0)]
        [InlineData("", 1)]
        [InlineData("targetType", notificationCreateModelSize)]
        public async Task InputValidation_InvalidModel_ErrorMessage(string targetType, int parametersNumber)
        {
            //arrange
            var inputModel = new NotificationCreateModel();
            inputModel.TargetType = targetType;
            inputModel.Parameters =
                Enumerable.Range(0, parametersNumber)
                    .ToDictionary(s => $"key{s}", s => $"value{s}");

            //act
            var result = await notificationService.CreateNotification(inputModel);

            //assert
            result.Status.Should().Be(errorMessage);
        }

        [Fact]
        public async Task ProcessNotification_InvalidNotificationModel_ErrorMessage()
        {
            //arrange
            var inputModel = new NotificationCreateModel();
            inputModel.TargetType = "targetType";
            inputModel.Parameters = new Dictionary<string, string>() { { "key", "value" } };

            var notificationId = Guid.NewGuid();

            var notificationSenderResult = new NotificationSenderResultModel();

            //mock
            guidProviderMock.Setup(s => s.CreateGuid()).Returns(notificationId);

            notificationSenderProxyMock.Setup(s => s.ProcessNotification(It.Is<string>(s => s == inputModel.TargetType),
                It.IsAny<IDictionary<string, string>>())).ReturnsAsync(notificationSenderResult);

            //act
            var result = await notificationService.CreateNotification(inputModel);

            //assert
            result.Status.Should().Be(errorMessage);

            guidProviderMock.VerifyAll();
            notificationSenderProxyMock.VerifyAll();
        }

        [Fact]
        public async Task ProcessNotification_NotDelivered_ErrorMessage()
        {
            //arrange
            var inputModel = new NotificationCreateModel();
            inputModel.TargetType = "targetType";
            inputModel.Parameters = new Dictionary<string, string>() { { "key", "value" } };

            var notificationId = Guid.NewGuid();

            var notificationSenderResult = new NotificationSenderResultModel();
            notificationSenderResult.IsValid = true;

            var serializedModel = JsonSerializer.Serialize(inputModel);

            var hashedJsonString = "hashedJsonString";

            var repositoryAddParams = (notificationId,
                serializedModel,
                hashedJsonString,
                notificationSenderResult.IsDelivered);

            //mock
            guidProviderMock.Setup(s => s.CreateGuid()).Returns(notificationId);

            notificationSenderProxyMock.Setup(s => s.ProcessNotification(
                It.Is<string>(s => s.Equals(inputModel.TargetType)),
                It.IsAny<IDictionary<string, string>>())).ReturnsAsync(notificationSenderResult);

            stringHashHelperMock.Setup(s => s.GetStringHashSHA512(It.Is<string>(s => s.Equals(serializedModel))))
                .Returns(hashedJsonString);

            notificationRepositoryMock.Setup(s =>
                s.Add(It.Is<(Guid id, string json, string jsonHash, bool status)>(s => s.Equals(repositoryAddParams)),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            //act
            var result = await notificationService.CreateNotification(inputModel);

            //assert
            result.Status.Should().Be(errorMessage);

            guidProviderMock.VerifyAll();
            notificationSenderProxyMock.VerifyAll();
            stringHashHelperMock.VerifyAll();
            notificationRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task ProcessNotification_Delivered_SuccessMessage()
        {
            //arrange
            var inputModel = new NotificationCreateModel();
            inputModel.TargetType = "targetType";
            inputModel.Parameters = new Dictionary<string, string>() { { "key", "value" } };

            var notificationId = Guid.NewGuid();

            var notificationSenderResult = new NotificationSenderResultModel();
            notificationSenderResult.IsValid = true;
            notificationSenderResult.IsDelivered = true;

            var serializedModel = JsonSerializer.Serialize(inputModel);

            var hashedJsonString = "hashedJsonString";

            var repositoryAddParams = (notificationId,
                serializedModel,
                hashedJsonString,
                notificationSenderResult.IsDelivered);

            //mock
            guidProviderMock.Setup(s => s.CreateGuid()).Returns(notificationId);

            notificationSenderProxyMock.Setup(s => s.ProcessNotification(
                It.Is<string>(s => s.Equals(inputModel.TargetType)),
                It.IsAny<IDictionary<string, string>>())).ReturnsAsync(notificationSenderResult);

            stringHashHelperMock.Setup(s => s.GetStringHashSHA512(It.Is<string>(s => s.Equals(serializedModel))))
                .Returns(hashedJsonString);

            notificationRepositoryMock.Setup(s =>
                s.Add(It.Is<(Guid id, string json, string jsonHash, bool status)>(s => s.Equals(repositoryAddParams)),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            //act
            var result = await notificationService.CreateNotification(inputModel);

            //assert
            result.Status.Should().Be("Доставлено");

            guidProviderMock.VerifyAll();
            notificationSenderProxyMock.VerifyAll();
            stringHashHelperMock.VerifyAll();
            notificationRepositoryMock.VerifyAll();
        }
    }
}