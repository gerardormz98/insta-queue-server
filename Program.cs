using LiveWaitlist.Data;
using LiveWaitlist.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.AddSingleton<IWaitlistManager, WaitlistManager>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<LiveWaitlistHub>("/live-waitlist");

app.Run();