namespace HRM.Models;

public partial class WorkHistoryItem
{
    public string Period { get; set; }
    public string Department { get; set; }
    public string Position { get; set; }
    public string Manager { get; set; }
}

public partial class LoginHistoryItem
{
    public DateTime LoginTime { get; set; }
    public string Device { get; set; }
    public string IpAddress { get; set; }
    public string Status { get; set; }
}