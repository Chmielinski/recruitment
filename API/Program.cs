using Commons.Utils.Database;
using Microsoft.EntityFrameworkCore;
using API.BusCommunication;
using API.Services.Interface;
using API.Services.Implementation;
using Commons.Repositories.Interface;
using Commons.Repositories.Implementation;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
builder.Services.AddSingleton<IConfigurationRoot>(configuration);

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<RidelyDbContext>(x => x.UseNpgsql(connectionString));
builder.Services.AddTransient<IFilesUploadMessageProducer, FilesUploadMessageProducer>();
builder.Services.AddTransient<IVisitorsRepository, VisitorsRepository>();
builder.Services.AddTransient<IVisitorsService, VisitorsService>();

builder.Logging.AddLog4Net();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();

// initialize db and apply migrations if necessary
await using var scope = app.Services.CreateAsyncScope();
using var db = scope.ServiceProvider.GetService<RidelyDbContext>();
await db.Database.MigrateAsync();

app.Run();