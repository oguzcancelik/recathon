using System.Threading.Tasks;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Models.Responses.Server.Auth;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IAuthService
    {
        Task<string> GetAuthUrlAsync();

        Task<BaseResponse<AuthInfoResponse>> GetAuthInfoAsync();

        Task<BaseResponse<string>> AuthenticateUserAsync(SpotifyAuthorizationRequest request);
    }
}