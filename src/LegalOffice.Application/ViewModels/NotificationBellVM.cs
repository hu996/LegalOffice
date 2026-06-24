namespace LegalOffice.Application.ViewModels;

public class NotificationBellVM
{
    public int UnreadCount { get; set; }
    public IReadOnlyList<NotificationItemVM> Items { get; set; } = Array.Empty<NotificationItemVM>();
}

public class NotificationItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
