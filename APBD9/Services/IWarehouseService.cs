using APBD9.Model;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Services;

public interface IWarehouseService
{
    Task <int>DoesProductExist(int IdProduct);
    Task <bool>DoesWarehouseExist(int IdWarehouse);
    Task<int> IsProductInOrder(Product_Warehouse product_warehouse);
    Task<bool> IsOrderDone(int idOrder);
    Task UpdateOrder(int idOrder);
    Task<int> PostObject(Product_Warehouse product_warehouse, int price,int idOrder);


    
}