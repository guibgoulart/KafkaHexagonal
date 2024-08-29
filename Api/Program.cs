using Core.Ports;
using Core.Services;
using Infrastructure.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
string bootstrapServers = builder.Environment.IsDevelopment() ? "localhost:9092" : "kafka:29092";
builder.Services.AddSingleton<IProducerPort, KafkaProducerAdapter>(sp => new KafkaProducerAdapter(bootstrapServers, sp.GetRequiredService<ILogger<KafkaProducerAdapter>>()));
builder.Services.AddSingleton<IConsumerPort, KafkaConsumerAdapter>(sp => new KafkaConsumerAdapter(bootstrapServers, "test-group", sp.GetRequiredService<ILogger<KafkaConsumerAdapter>>()));
builder.Services.AddSingleton<MessagingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
