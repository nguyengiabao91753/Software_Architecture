using Integrations.Messaging.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Messaging.Consumer;
public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly HttpClient _httpClient;
    public OrderPlacedConsumer(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrdersApi");
    }
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        Console.WriteLine($"OrderPlacedEvent received: {context.Message}");

       
        //await _httpClient.PutAsJsonAsync($"/api/orders/paid", context.Message.OrderId.ToString());
    }
}
