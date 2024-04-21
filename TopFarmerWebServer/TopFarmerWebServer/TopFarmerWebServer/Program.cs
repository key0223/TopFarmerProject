using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TopFarmerWebServer.Data;
using TopFarmerWebServer.DB;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

//builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Load DataTable
ConfigManager.LoadConfig();
DataManager.LoadData();

#region Redis
var GameRedisOptions = ConfigurationOptions.Parse(ConfigManager.Config.redisConnectionString);
GameRedisOptions.AllowAdmin = true;
GameRedisOptions.Ssl = false;
GameRedisOptions.AbortOnConnectFail = false;
GameRedisOptions.ConnectRetry = 3;
GameRedisOptions.ConnectTimeout = 5000;
GameRedisOptions.SyncTimeout = 5000;
builder.Services.AddScoped<IDatabase>(cfg =>
{
    var redis = ConnectionMultiplexer.Connect(GameRedisOptions);
    return redis.GetDatabase();
});
#endregion

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();