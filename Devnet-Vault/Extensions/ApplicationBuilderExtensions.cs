using Microsoft.AspNetCore.HttpOverrides;
using Presentation.Middlewares;

namespace Presentation.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UsePresentation(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("Allow_Vault_Client");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
