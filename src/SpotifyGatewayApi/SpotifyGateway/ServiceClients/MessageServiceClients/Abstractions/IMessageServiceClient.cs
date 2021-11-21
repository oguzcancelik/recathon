using System.Threading.Tasks;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions
{
    public interface IMessageServiceClient
    {
        bool SendMessage(string message, MessageType messageType = MessageType.Information);

        Task<bool> SendMessageAsync(string message, MessageType messageType = MessageType.Information);

        bool SendMessage<T>(T model, MessageType messageType = MessageType.Information);

        Task<bool> SendMessageAsync<T>(T model, MessageType messageType = MessageType.Information);
    }
}