using System.ComponentModel.DataAnnotations;

namespace APBD9.Model;

public class Product_Warehouse
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    [Range(1, Int32.MaxValue)]
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}