using LiveWaitlist.Data;
using LiveWaitlist.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.AddSingleton<IWaitlistManager, WaitlistManager>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
            builder.AllowCredentials();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
    });
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.MapHub<LiveWaitlistHub>("/hubs/live-waitlist");

app.Run();