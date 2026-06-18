using Microsoft.AspNetCore.Mvc;

namespace BetliveApi.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationErrorHandling(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new
                {
                    statusCode = 400,
                    message    = "Validation failed.",
                    errors
                };

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }
}
