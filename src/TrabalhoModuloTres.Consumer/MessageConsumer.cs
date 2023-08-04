using MassTransit;
using TrabalhoModuloTres.Shared;
using System.Text.Json;

namespace TrabalhoModuloTres.Consumer;

public class MessageConsumer : IConsumer<IMessage>
{
    public async Task Consume(ConsumeContext<IMessage> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        Console.WriteLine($"Producer: {serializedMessage}");
    }
}