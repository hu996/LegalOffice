using LegalOffice.Application.Interfaces;
namespace LegalOffice.Infrastructure.Services;
public class MockMessageService : IMessageService
{
    public Task<(bool success, string response)> SendWhatsAppAsync(string phoneNumber, string message)
        => Task.FromResult((true, $"Mock WhatsApp sent to {phoneNumber}"));
    public Task<(bool success, string response)> SendSmsAsync(string phoneNumber, string message)
        => Task.FromResult((true, $"Mock SMS sent to {phoneNumber}"));
}
