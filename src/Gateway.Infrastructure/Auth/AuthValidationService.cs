using Gateway.Application.Services;
using Gateway.Domain.Models;
using System.Net.Http.Json;

namespace Gateway.Infrastructure.Auth;
public class AuthValidationService(HttpClient httpClient) : IAuthValidationService
{
    public async Task<UserInfo?> ValidateCookieAsync(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
            return null;

        try {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:3000/auth/validate");
            request.Headers.Add("Cookie", $"jwt={jwtToken}");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            return userInfo;
        }
        catch {
            return null;
        }
    }
}