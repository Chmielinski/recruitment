using Commons.Repositories.Implementation;
using Commons.Repositories.Interface;
using Commons.Utils.Database;
using Microsoft.EntityFrameworkCore;
using Worker.BusCommunication;
using Worker.Services.Implementation;
using Worker.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
builder.Services.AddSingleton(configuration);

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RidelyDbContext>(x => x.UseNpgsql(connectionString), ServiceLifetime.Singleton);

builder.Services.AddTransient<IVisitorsRepository, VisitorsRepository>();
builder.Services.AddTransient<IVisitorsService, VisitorsService>();
builder.Services.AddHostedService<FileUploadMessageReceiver>();

builder.Logging.AddLog4Net();

var app = builder.Build();

app.Run();
