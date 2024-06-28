
using Redis.Domain.Seedwork;


namespace Redis.Domain.AggregatesModel.ProductAggregate;


public class Product : Entity, IAggregateRoot
{
    private string _gender;
    private string _masterCategory;
    private string _subCategory;
    private string _articleType;
    private string _baseColour;
    private string _season;
    private int _year;
    private string _usage;
    private string _productDisplayName;
    
    protected Product()
    {}
    
    public Product(
        int id,
        string gender, 
        string masterCategory, 
        string subCategory,
        string articleType,
        string baseColour,
        string season,
        int year,
        string usage,
        string productDisplayName) : this()
    {
        Id = id;
        _gender = gender;
        _masterCategory = masterCategory;
        _subCategory = subCategory;
        _articleType = articleType;
        _baseColour = baseColour;
        _season = season;
        _year = year;
        _usage = usage;
        _productDisplayName = productDisplayName;
    }
}