namespace LegalOffice.Application.Interfaces;
public interface IMessageService
{
    Task<(bool success, string response)> SendWhatsAppAsync(string phoneNumber, string message);
    Task<(bool success, string response)> SendSmsAsync(string phoneNumber, string message);
}
