using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MessageSenderServiceApi.Contracts.Notification;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NotificationSender.IosProvider;
using NotificationSender.Models;
using Xunit;

namespace MessageSenderServiceApi.Tests.Integration.Modules.Notification
{
    public class CreateNotificationScenarios
    {
        private const string errorMessage = "Не доставлено";
        private const string successMessage = "Доставлено";

        [Fact]
        public async Task GetNotificationStatus_IdNotExists_ErrorMessage()
        {
            //arrange
            var application = PrepareApplication();
            var client = application.CreateClient();

            //act
            var notificationStatusResult =
                await client.GetAsync($"api/v1/notifications/{Guid.NewGuid()}/status");
            var notificationStatus =
                await notificationStatusResult.Content.ReadFromJsonAsync<NotificationStatusModel>();

            //assert 
            notificationStatusResult.StatusCode.Should().Be(200);
            notificationStatus.Status.Should().Be(errorMessage);
        }

        [Fact]
        public async Task CreateNotification_CheckStatus_SuccessMessage()
        {
            //arrange
            var application = PrepareApplication();
            var client = application.CreateClient();
            var notificationCreateModel = PrepareNotificationIosModel();
            var serializedModel = JsonSerializer.Serialize(notificationCreateModel);

            //act POST CreateNotification
            var createNotificationResult =
                await client.PostAsync($"api/v1/notifications",
                    new StringContent(serializedModel, Encoding.UTF8, MediaTypeNames.Application.Json));

            var createNotificationModel =
                await createNotificationResult.Content.ReadFromJsonAsync<NotificationCreateResultModel>();

            //assert CreateNotification result
            createNotificationResult.StatusCode.Should().Be(200);
            createNotificationModel.Status.Should().Be(successMessage);

            //act GET GetNotificationStatus
            var notificationStatusResult =
                await client.GetAsync($"api/v1/notifications/{createNotificationModel.Id}/status");

            var notificationStatusModel =
                await notificationStatusResult.Content.ReadFromJsonAsync<NotificationStatusModel>();

            //assert GetNotificationStatus result
            notificationStatusResult.StatusCode.Should().Be(200);
            notificationStatusModel.Status.Should().Be(successMessage);
        }

        [Fact]
        public async Task CreateNotification_5_CheckStatus_EachFifthWithError()
        {
            //arrange
            var numberOfExpectedErrors = 5;
            var application = PrepareApplication();
            var client = application.CreateClient();
            var notificationCreateModel = PrepareNotificationIosModel();
            var serializedModel = JsonSerializer.Serialize(notificationCreateModel);

            var errorMessageIndex = 0;
            var tasks = Enumerable.Range(1, numberOfExpectedErrors * 5).Select(s => Task.Run(async () =>
            {
                //act POST CreateNotification
                var createNotificationResult =
                    await (await client.PostAsync($"api/v1/notifications",
                            new StringContent(serializedModel, Encoding.UTF8, MediaTypeNames.Application.Json)))
                        .Content.ReadFromJsonAsync<NotificationCreateResultModel>();
                await Task.Delay(100);

                //act GET GetNotificationStatus
                var notificationStatus =
                    await (await client.GetAsync($"api/v1/notifications/{createNotificationResult.Id}/status"))
                        .Content.ReadFromJsonAsync<NotificationStatusModel>();
                await Task.Delay(100);

                //assert  
                notificationStatus.Status.Should().Be(createNotificationResult.Status);
                if (notificationStatus.Status == errorMessage)
                {
                    Interlocked.Increment(ref errorMessageIndex);
                }
            })).ToArray();

            await Task.WhenAll(tasks);
            errorMessageIndex.Should().Be(numberOfExpectedErrors);
        }

        [Fact]
        public async Task CreateNotification_NotificationValidation_iOS()
        {
            //arrange
            var application = PrepareApplication();
            var client = application.CreateClient();

            var checkList = new List<(string errorReason, Action<Dictionary<string, string>> checkAction)>()
            {
                { ("PushToken too short", (dictionary) => { dictionary["PushToken"] = ""; }) },
                { ("PushToken too long", (dictionary) => { dictionary["PushToken"] = new string('0', 50); }) },
                { ("Alert too short", (dictionary) => { dictionary["Alert"] = ""; }) },
                { ("Alert too long", (dictionary) => { dictionary["Alert"] = new string('0', 2000); }) },
            };

            var tasks = Enumerable.Range(0, checkList.Count).Select(s => Task.Run(async () =>
            {
                var notificationCreateModel = PrepareNotificationIosModel();
                checkList[s].checkAction(notificationCreateModel.Parameters);

                var serializedModel = JsonSerializer.Serialize(notificationCreateModel);
                //act POST CreateNotification
                var createNotificationResult =
                    await (await client.PostAsync($"api/v1/notifications",
                            new StringContent(serializedModel, Encoding.UTF8, MediaTypeNames.Application.Json)))
                        .Content.ReadFromJsonAsync<NotificationCreateResultModel>();
                await Task.Delay(50);

                //assert CreateNotification 
                createNotificationResult.Status.Should().Be(errorMessage, checkList[s].errorReason);
            })).ToArray();

            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task CreateNotification_NotificationValidation_Android()
        {
            //arrange
            var application = PrepareApplication();
            var client = application.CreateClient();

            var checkList =
                new List<(string errorReason, string statusMessage, Action<Dictionary<string, string>> checkAction)>()
                {
                    { ("DeviceToken too short", errorMessage, (dictionary) => { dictionary["DeviceToken"] = ""; }) },
                    {
                        ("DeviceToken too long", errorMessage,
                            (dictionary) => { dictionary["DeviceToken"] = new string('0', 50); })
                    },
                    { ("Title too short", errorMessage, (dictionary) => { dictionary["Title"] = ""; }) },
                    {
                        ("Title too long", errorMessage,
                            (dictionary) => { dictionary["Title"] = new string('0', 255); })
                    },
                    { ("Message too short", errorMessage, (dictionary) => { dictionary["Message"] = ""; }) },
                    {
                        ("Message too long", errorMessage,
                            (dictionary) => { dictionary["Message"] = new string('0', 2000); })
                    },
                    { ("Condition too short", successMessage, (dictionary) => { dictionary["Condition"] = ""; }) },
                    {
                        ("Condition too long", errorMessage,
                            (dictionary) => { dictionary["Condition"] = new string('0', 2000); })
                    },
                };

            var tasks = Enumerable.Range(0, checkList.Count).Select(s => Task.Run(async () =>
            {
                var notificationCreateModel = PrepareNotificationAndroidModel();
                checkList[s].checkAction(notificationCreateModel.Parameters);

                var serializedModel = JsonSerializer.Serialize(notificationCreateModel);
                //act POST CreateNotification
                var createNotificationResult =
                    await (await client.PostAsync($"api/v1/notifications",
                            new StringContent(serializedModel, Encoding.UTF8, MediaTypeNames.Application.Json)))
                        .Content.ReadFromJsonAsync<NotificationCreateResultModel>();
                await Task.Delay(50);

                //assert CreateNotification 
                createNotificationResult.Status.Should().Be(checkList[s].statusMessage, checkList[s].errorReason);
            })).ToArray();

            await Task.WhenAll(tasks);
        }

        private WebApplicationFactory<Program> PrepareApplication()
        {
            var result = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    var fi = File.ReadAllText("appsettings.json");
                    builder.ConfigureAppConfiguration(s => s.AddJsonFile("appsettings.json"));
                    builder.UseUrls("http://localhost:7050");
                });

            return result;
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

        private NotificationCreateModel PrepareNotificationAndroidModel() => new NotificationCreateModel
        {
            TargetType = "Android",
            Parameters = new Dictionary<string, string>()
            {
                { "DeviceToken", "DeviceToken Text" },
                { "Message", "Message Text" },
                { "Title", "Title" },
                { "Condition", "ConditionString" },
            }
        };
    }
}