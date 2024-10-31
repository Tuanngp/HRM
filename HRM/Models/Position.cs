namespace HRM.Models;

public partial class Position
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    
    public virtual Department Department { get; set; } = null!;
}