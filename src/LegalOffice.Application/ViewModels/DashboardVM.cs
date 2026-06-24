namespace LegalOffice.Application.ViewModels;

public class DashboardVM
{
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int ClosedCases { get; set; }
    public int TodayHearings { get; set; }
    public int WeekHearings { get; set; }
    public decimal MonthPayments { get; set; }
    public decimal TotalRemaining { get; set; }
    public int MyCases { get; set; }
}
