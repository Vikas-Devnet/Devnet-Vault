using Application.Features.Common.Interfaces;
using Application.Features.Common.Models;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace Infrastructure.Email;

public class EmailQueue(EmailService _emailService) : BackgroundService, IEmailQueue
{
    private readonly Channel<EmailMessage> _queue = Channel.CreateUnbounded<EmailMessage>();

    public bool QueueEmail(EmailMessage message)
    {
        return _queue.Writer.TryWrite(message);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _emailService.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
