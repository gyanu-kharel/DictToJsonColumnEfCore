namespace store.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public int? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
}