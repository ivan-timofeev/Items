using System.Text;
using System.Text.Json;
using Items.Models.DataTransferObjects.CreateOrder;
using RabbitMQ.Client;

namespace Items.Services;

public interface IOrdersMicroserviceApiClient
{
    void MakeResponseForReserveItemsRequest(ReserveItemsResponse response);
}

public sealed class OrdersMicroserviceApiClient : IOrdersMicroserviceApiClient, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _model;

    public OrdersMicroserviceApiClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void MakeResponseForReserveItemsRequest(ReserveItemsResponse response)
    {
        if (_model is null || _connection is null)
            InitializeRabbitMq();
        
        var json = JsonSerializer.Serialize(response);
        var body = Encoding.UTF8.GetBytes(json);

        _model!.BasicPublish(exchange: "",
            routingKey: "ReserveItemsResponse",
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _model?.Dispose();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory { HostName = _configuration["RabbitMqHostName"] };
        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();

        _model.QueueDeclare(queue: "ReserveItemsResponse",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
}
