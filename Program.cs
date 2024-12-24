using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var elasticUri = configuration.GetSection("Elasticsearch:Uri").Value;
builder.Services.AddSingleton(provider =>
{
    var settings = new ElasticsearchClientSettings(new Uri(elasticUri));
    return new ElasticsearchClient(settings);
});

builder.Services.AddHostedService<RabbitMqConsumerService>();

builder.Services.AddScoped<IProductService, ProductService>();


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

app.MapControllers();

// 8. Uygulamayı Çalıştırma
app.Run();
