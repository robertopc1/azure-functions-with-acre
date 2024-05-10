using DataAccess.Models;
namespace DataAccess.Data;

public interface IStylesData
{
    Task UpdateStyle(Styles style);
}