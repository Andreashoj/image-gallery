using backend.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<GalleryContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GalleryContext>();
    await context.Database.MigrateAsync();
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
