using backend.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<GalleryContext>();

// Add detailed error logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Add this around your database migration code
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<GalleryContext>();
        await context.Database.MigrateAsync();
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An error occurred during migration");
    throw; // This will help us see the actual error
}

app.MapHealthChecks("/health");
app.MapGet("/test-connection", async (GalleryContext context) =>
{
    try 
    {
        // Force the connection to open
        await context.Database.OpenConnectionAsync();
        return "Connection successful!";
    }
    catch (Exception ex)
    {
        return $"Connection failed: {ex.Message}";
    }
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
