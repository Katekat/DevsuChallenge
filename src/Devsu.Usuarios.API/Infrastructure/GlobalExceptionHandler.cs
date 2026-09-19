using Microsoft.AspNetCore.Diagnostics;

namespace Devsu.Usuarios.API.Infrastructure
{
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError => appError.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.ContentType = "application/json";

                    if (contextFeature.Error is ArgumentException or InvalidOperationException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new { Mensaje = contextFeature.Error.Message });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(new { Mensaje = "Error interno del servidor" });
                    }
                }
            }));
        }
    }
}
