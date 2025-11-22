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

        try
        {

            var rs= await _httpClient.PutAsJsonAsync($"/api/order/paid", context.Message.OrderId.ToString());

            if (rs.IsSuccessStatusCode)
            {
                await context.Publish(new PaymentSuceessded(context.Message.OrderId));
            }
            else
            {
                Console.WriteLine($"Failed to mark order {context.Message.OrderId} as paid. Status Code: {rs.StatusCode}");
                await context.Publish(new PaymentFailed(context.Message.OrderId, "Failed to mark order as paid"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing OrderPlacedEvent: {ex.Message}");
            await context.Publish(new PaymentFailed(context.Message.OrderId, "Failed to mark order as paid"));
        }
    }
}
