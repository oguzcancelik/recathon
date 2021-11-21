using System.Threading.Tasks;
using SpotifyAPI.Web.Models;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Services.Abstractions
{
    public interface ITokenService
    {
        Task<BaseResponse<TokenResponse>> GenerateTokenAsync();

        Task<bool> DecryptTokenAsync(string token);

        Task<Token> ExchangeCodeAsync(Credential credential, string code, DeviceType deviceType);

        Task<Token> RefreshTokenAsync(Credential credential, string refreshToken);
    }
}