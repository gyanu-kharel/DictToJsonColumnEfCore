namespace store.Models;

public class CompanyProp
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Dictionary<string, object>? Values { get; set; }
}