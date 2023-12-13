using System.Text;
using System.Text.Json;
using Items.Models.DataTransferObjects.CreateOrder;
using Items.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Items.BackgroundServices;

public sealed class ReserveItemsRequestProcessingBackgroundService : BackgroundService
{
    private readonly ILogger<ReserveItemsRequestProcessingBackgroundService> _logger;
    private readonly IReserveItemsRequestProcessor _reserveItemsRequestProcessor;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;

    public ReserveItemsRequestProcessingBackgroundService(
        ILogger<ReserveItemsRequestProcessingBackgroundService> logger,
        IReserveItemsRequestProcessor reserveItemsRequestProcessor,
        IConfiguration configuration)
    {
        _logger = logger;
        _reserveItemsRequestProcessor = reserveItemsRequestProcessor;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            InitializeRabbitMq();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, eventArgs) =>
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var reserveItemsRequest = JsonSerializer.Deserialize<ReserveItemsRequest>(json)
                    ?? throw new InvalidOperationException("Json must be of type ReserveItemsRequest");

                _logger.LogInformation(
                    "Started processing of ReserveItemsRequest. OrderId: {O}",
                    reserveItemsRequest.OrderId);

                _reserveItemsRequestProcessor.ProcessReserveItemsRequest(reserveItemsRequest);

                _logger.LogInformation(
                    "ReserveItemsRequest processed. OrderId: {O}",
                    reserveItemsRequest.OrderId);

                _channel!.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel!.BasicConsume("ReserveItemsRequest", false, consumer);
        }, cancellationToken);

        return Task.CompletedTask;
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMqHostName"],
            AutomaticRecoveryEnabled = true
        };

        while (_channel is null)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(
                    queue: "ReserveItemsResponse",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Initialization error. Retrying after 120s\nMessage: {M}",
                    ex.Message);

                Thread.Sleep(120_000);
            }
        }

        _logger.LogInformation("Initialization was successful. Waiting for messages in the queue");
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        
        base.Dispose(); 
    }
}
