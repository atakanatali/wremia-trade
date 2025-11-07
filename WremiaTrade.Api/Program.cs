using Hangfire;
using Hangfire.MemoryStorage;

using WremiaTrade.ConfigAdapter;
using WremiaTrade.MessageBrokers.Configuration;
using WremiaTrade.MessageBrokers.Interfaces;
using WremiaTrade.MessageBrokers.RabbitMq;
using WremiaTrade.Services.Trade;
using WremiaTrade.Services.Trade.Interfaces;
using WremiaTrade.Services.Trade.Jobs;

var builder = WebApplication.CreateBuilder(args);

ConfigurationAdaptor.Manage.Init(builder.Configuration);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddHangfire(configuration =>
{
    configuration.UseSimpleAssemblyNameTypeSerializer();
    configuration.UseRecommendedSerializerSettings();
    configuration.UseMemoryStorage();
});

builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddSingleton<ITradeRepository, InMemoryTradeRepository>();
builder.Services.AddSingleton<ITradeJobScheduler, TradeJobScheduler>();
builder.Services.AddSingleton<ITradeEventPublisher, TradeEventPublisher>();
builder.Services.AddTransient<TradeExecutionJob>();
builder.Services.AddScoped<ITradeService, TradeService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
app.MapControllers();

app.Run();
