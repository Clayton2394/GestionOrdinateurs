var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5096",
            "https://localhost:7293",
            "http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("Client");
app.MapReverseProxy();

app.Run();
