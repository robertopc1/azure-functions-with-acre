using MediatR;
using System.Runtime.Serialization;

namespace Redis.API.Application.Commands;

[DataContract]
public class CreateProductCommand : IRequest<bool>
{
    [DataMember]
    public int Id { get; private set; }
    [DataMember]
    public string Gender { get; private set; }
    
    [DataMember]
    public string MasterCategory { get; private set; }
    
    [DataMember]
    public string SubCategory { get; private set; }
    
    [DataMember]
    public string ArticleType { get; private set; }
    
    [DataMember]
    public string BaseColour { get; private set; }
    
    [DataMember]
    public string Season { get; private set; }
    
    [DataMember]
    public int Year { get; private set; }
    
    [DataMember]
    public string Usage { get; private set; }
    
    [DataMember]
    public string ProductDisplayName { get; private set; }

    public CreateProductCommand(string gender, 
        string masterCategory, 
        string subCategory, 
        string articleType, 
        string baseColour,
        string season,
        int year,
        string usage,
        string productDisplayName)
    {
        Gender = gender;
        MasterCategory = masterCategory;
        SubCategory = subCategory;
        ArticleType = articleType;
        BaseColour = baseColour;
        Season = season;
        Year = year;
        Usage = usage;
        ProductDisplayName = productDisplayName;
    }
}