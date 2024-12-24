using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private IConnection _connection;
    private IModel _channel;

    private const string CreateQueueName = "Create-Queue";
    private const string UpdateQueueName = "Update-Queue";
    private const string DeleteQueueName = "Delete-Queue";

    public RabbitMqConsumerService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<RabbitMqConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration.GetConnectionString("RabbitMQConnection")),
            ClientProvidedName = "ProductConsumer"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: CreateQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: UpdateQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: DeleteQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.BasicQos(0, 1, false);

        _logger.LogInformation("RabbitMQ bağlantısı ve kanalları başarıyla oluşturuldu.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var createConsumer = new EventingBasicConsumer(_channel);
        createConsumer.Received += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            await HandleCreateMessage(message, ea.DeliveryTag);
        };
        _channel.BasicConsume(queue: CreateQueueName, autoAck: false, consumer: createConsumer);
        _logger.LogInformation("Create-Queue dinleyicisi başlatıldı.");

        var updateConsumer = new EventingBasicConsumer(_channel);
        updateConsumer.Received += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            await HandleUpdateMessage(message, ea.DeliveryTag);
        };
        _channel.BasicConsume(queue: UpdateQueueName, autoAck: false, consumer: updateConsumer);
        _logger.LogInformation("Update-Queue dinleyicisi başlatıldı.");

        var deleteConsumer = new EventingBasicConsumer(_channel);
        deleteConsumer.Received += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            await HandleDeleteMessage(message, ea.DeliveryTag);
        };
        _channel.BasicConsume(queue: DeleteQueueName, autoAck: false, consumer: deleteConsumer);
        _logger.LogInformation("Delete-Queue dinleyicisi başlatıldı.");

        return Task.CompletedTask;
    }

    private async Task HandleCreateMessage(string message, ulong deliveryTag)
    {
        using var scope = _scopeFactory.CreateScope();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

        try
        {
            var productCreateDto = JsonSerializer.Deserialize<ProductCreateDto>(message);
            if (productCreateDto == null)
                throw new Exception("Geçersiz Create mesajı.");

            var product = new Product
            {
                Name = productCreateDto.Name,
                Slug = productCreateDto.Slug,
                Description = productCreateDto.Description,
                Info = productCreateDto.Info,
                Price = productCreateDto.Price,
                DiscountedPrice = productCreateDto.DiscountedPrice,
                OriginalPrice = productCreateDto.OriginalPrice,
                StockQuantity = productCreateDto.StockQuantity,
                IsFeatured = productCreateDto.IsFeatured,
                CreatedBy = productCreateDto.CreatedBy,
                UpdatedBy = productCreateDto.CreatedBy,
                SearchIndex = productCreateDto.SearchIndex,
                IsSearchable = productCreateDto.IsSearchable
            };

            var esResult = await productService.CreateProductAsync(product);
            if (!esResult)
                throw new Exception("Elasticsearch'e ürün eklenirken hata oluştu.");

            _channel.BasicAck(deliveryTag, false);
            _logger.LogInformation($"Create işlemi başarıyla tamamlandı. Product Name: {product.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Create işleminde hata: {ex.Message}");

            _channel.BasicNack(deliveryTag, false, requeue: false);
        }
    }

    private async Task HandleUpdateMessage(string message, ulong deliveryTag)
    {
        using var scope = _scopeFactory.CreateScope();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

        try
        {
            var productUpdateDto = JsonSerializer.Deserialize<ProductUpdateDto>(message);
            if (productUpdateDto == null)
                throw new Exception("Geçersiz Update mesajı.");


            var esResult = await productService.UpdateProductAsync(productUpdateDto.Id, new Product
            {
                Name = productUpdateDto.Name,
                Slug = productUpdateDto.Slug,
                Description = productUpdateDto.Description,
                Info = productUpdateDto.Info,
                Price = productUpdateDto.Price,
                DiscountedPrice = productUpdateDto.DiscountedPrice,
                OriginalPrice = productUpdateDto.OriginalPrice,
                StockQuantity = productUpdateDto.StockQuantity,
                IsFeatured = productUpdateDto.IsFeatured,
                UpdatedBy = productUpdateDto.UpdatedBy,
                SearchIndex = productUpdateDto.SearchIndex,
                IsSearchable = productUpdateDto.IsSearchable
            });

            if (!esResult)
                throw new Exception("Elasticsearch'de ürün güncellenirken hata oluştu.");

            // Mesajı ACK et
            _channel.BasicAck(deliveryTag, false);
            _logger.LogInformation($"Update işlemi başarıyla tamamlandı. Product ID: {productUpdateDto.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Update işleminde hata: {ex.Message}");
            _channel.BasicNack(deliveryTag, false, requeue: false);
        }
    }

    private async Task HandleDeleteMessage(string message, ulong deliveryTag)
    {
        using var scope = _scopeFactory.CreateScope();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

        try
        {
            var productDeleteDto = JsonSerializer.Deserialize<ProductDeleteDto>(message);
            if (productDeleteDto == null)
                throw new Exception("Geçersiz Delete mesajı.");

            var esResult = await productService.DeleteProductAsync(productDeleteDto.Id);
            if (!esResult)
                throw new Exception("Elasticsearch'den ürün silinirken hata oluştu.");

            _channel.BasicAck(deliveryTag, false);
            _logger.LogInformation($"Delete işlemi başarıyla tamamlandı. Product ID: {productDeleteDto.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Delete işleminde hata: {ex.Message}");
            _channel.BasicNack(deliveryTag, false, requeue: false);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
