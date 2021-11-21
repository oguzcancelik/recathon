using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyGateway.Infrastructure.Api.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses.MessageResponses;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;

namespace SpotifyGateway.ServiceClients.MessageServiceClients
{
    public class TelegramServiceClient : IMessageServiceClient
    {
        private readonly IApiWrapper<TelegramServiceOptions> _telegramApiWrapper;
        private readonly ITelegramSettings _telegramSettings;

        public TelegramServiceClient(
            IApiWrapper<TelegramServiceOptions> telegramApiWrapper,
            ITelegramSettings telegramSettings
        )
        {
            _telegramApiWrapper = telegramApiWrapper;
            _telegramSettings = telegramSettings;
        }

        public bool SendMessage(string message, MessageType messageType)
        {
            var apiResponse = _telegramApiWrapper.Get<BaseTelegramResponse<SendMessageResponse>>(x =>
                x.SendMessageAction.Format(_telegramSettings.Token, GetChatId(messageType), WebUtility.UrlEncode(message))
            );

            return apiResponse.IsSuccessResponse() &&  apiResponse.Value is { Ok: true };
        }

        public async Task<bool> SendMessageAsync(string message, MessageType messageType)
        {
            var apiResponse = await _telegramApiWrapper.GetAsync<BaseTelegramResponse<SendMessageResponse>>(x =>
                x.SendMessageAction.Format(_telegramSettings.Token, GetChatId(messageType), WebUtility.UrlEncode(message))
            );

            return apiResponse.IsSuccessResponse() &&  apiResponse.Value is { Ok: true };
        }

        public bool SendMessage<T>(T model, MessageType messageType)
        {
            return SendMessage(model.ToJson(Formatting.Indented, MessageHelpers.AllowNull(messageType)), messageType);
        }

        public async Task<bool> SendMessageAsync<T>(T model, MessageType messageType)
        {
            return await SendMessageAsync(model.ToJson(Formatting.Indented, MessageHelpers.AllowNull(messageType)), messageType);
        }

        private string GetChatId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.ErrorLog => _telegramSettings.ErrorLogChatId,
                MessageType.Information => _telegramSettings.InformationChatId,
                _ => _telegramSettings.InformationChatId
            };
        }
    }
}