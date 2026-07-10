using backend.Models;

var builder = WebApplication.CreateBuilder(args);

// React'in bağlanabilmesi için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Bellekte bildirimleri tutacağız
var notifications = new List<Notification>();

// Bildirim ekle
app.MapPost("/api/notifications", (Notification notification) =>
{
    notifications.Add(notification);
    return Results.Ok(notification);
});

// Bildirimleri getir
app.MapGet("/api/notifications", () =>
{
    return Results.Ok(notifications);
});

app.Run();