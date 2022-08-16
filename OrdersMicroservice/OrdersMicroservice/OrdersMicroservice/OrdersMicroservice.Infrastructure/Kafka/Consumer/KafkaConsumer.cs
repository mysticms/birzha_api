using Confluent.Kafka;
using OrdersMicroservice.Domain.EventsBase;
using OrdersMicroservice.Infrastructure.Kafka.Config;
using Microsoft.Extensions.Hosting;

namespace OrdersMicroservice.Infrastructure.Kafka.Consumer;

public class KafkaConsumer<TKey, TValue> : IHostedService, IDisposable
{
    private readonly IConsumer<TKey, TValue> _consumer;

    private readonly IEventHandler<TKey, TValue> _handler;

    private readonly string _topic;

    public KafkaConsumer(KafkaConsumerConfig config, IConsumer<TKey, TValue> consumer,
        IEventHandler<TKey, TValue> handler)
    {
        _consumer = consumer;
        _topic = config.Topic;
        _handler = handler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ConsumeEvents(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;


    private async Task ConsumeEvents(CancellationToken token)
    {
        _consumer.Subscribe(_topic);

        ConsumeResult<TKey, TValue> result = null;

        while (!token.IsCancellationRequested)
        {
            result = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
            if (result == null)
                continue;
            
            
            Console.WriteLine($"On {typeof(TValue).Name} kafka consumer");

            Console.WriteLine($"From consumer: message value: {result.Message.Value}");
            var processResult = await _handler.ProcessAsync(result.Message);
            if (!processResult.Ok)
                Console.WriteLine($"Error while consumer error: {processResult.Error?.Message}");
            else
                _consumer.Commit(result);
        }

        if (result == null)
            return;

        // We do commit before invoke close method (if autocommit is disable)
        // because Close() method informs consumer group coordinator about changing
        // consumers number so after that rebalancing starts 
        _consumer.Commit(result);
        _consumer.Close();
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}