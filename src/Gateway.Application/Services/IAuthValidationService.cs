using Gateway.Domain.Models;

namespace Gateway.Application.Services;

public interface IAuthValidationService
{
    Task<UserInfo?> ValidateCookieAsync(string cookieValue);
}