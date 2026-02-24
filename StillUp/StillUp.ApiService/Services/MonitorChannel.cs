using System.Threading.Channels;
using StillUp.ApiService.Entities;

namespace StillUp.ApiService.Services;

public class MonitorChannel
{
    private readonly Channel<MonitorEntry> _channel = Channel.CreateUnbounded<MonitorEntry>(
        new UnboundedChannelOptions { SingleWriter = false, SingleReader = false }
    );

    public ChannelWriter<MonitorEntry> Writer => _channel.Writer;
    public ChannelReader<MonitorEntry> Reader => _channel.Reader;
}
