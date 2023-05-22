namespace store.ViewModels;

public class SetPropertyDto
{
    public List<Dict> Properties { get; set; }
}

public class Dict
{
    public string Key { get; set; }
    public object Value { get; set; }
}
