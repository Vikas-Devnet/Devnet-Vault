using Microsoft.AspNetCore.Http;

namespace Application.Features.Common.Interfaces;

public interface IUtilitiesService
{
    void TrimStrings<T>(T model);
    string ExtractIpAddress(HttpContext httpContext);
}
