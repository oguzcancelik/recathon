using System;
using Microsoft.AspNetCore.Http;
using SpotifyGateway.Infrastructure.Constants;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class AuthExtensions
    {
        public static string GetAuthToken(this HttpRequest request, string key)
        {
            return request.GetHeaderOrCookie(AuthConstants.AuthToken)?.Decrypt(key);
        }

        public static string GetHangfireToken(this HttpRequest request)
        {
            return request.GetCookie(AuthConstants.HangfireToken);
        }

        public static string GetSwaggerToken(this HttpRequest request)
        {
            return request.GetCookie(AuthConstants.SwaggerToken);
        }

        public static string GetAdminToken(this HttpRequest request)
        {
            return request.GetHeaderOrCookie(AuthConstants.AdminToken);
        }

        public static bool IsAdmin(this HttpRequest request, string adminToken)
        {
            return request.GetAdminToken() == adminToken;
        }

        public static string GetHeaderOrCookie(this HttpRequest request, string value)
        {
            return request.GetHeader(value) ?? request.GetCookie(value);
        }

        public static string GetCookie(this HttpRequest request, string value)
        {
            var exists = request.Cookies.TryGetValue(value, out var result);

            return exists && !string.IsNullOrEmpty(result) ? result : default;
        }

        public static string GetHeader(this HttpRequest request, string value)
        {
            var exists = request.Headers.TryGetValue(value, out var result);

            return exists && !string.IsNullOrEmpty(result) ? result.ToString() : default;
        }

        public static void SetSessionToken(this HttpResponse response, string sessionToken)
        {
            var cookieOptions = new CookieOptions
            {
                // Secure = true,
                Expires = DateTime.Now.AddDays(30)
            };

            response.Cookies.Append(AuthConstants.AuthToken, sessionToken, cookieOptions);

            response.Headers.Add(AuthConstants.AuthToken, sessionToken);
        }
    }
}