using Microsoft.EntityFrameworkCore;
using BetliveApi.Data;
using BetliveApi.Extensions;
using BetliveApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddValidationErrorHandling();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Auto-run migrations and seed on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();   // applies any pending migrations
    await DataSeeder.SeedAsync(context);     // seeds if empty
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Betlive API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
