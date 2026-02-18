using Application.Features.Common.Models;

namespace Application.Features.Common.Interfaces;

public interface IEmailQueue
{
    bool QueueEmail(EmailMessage message);
}
