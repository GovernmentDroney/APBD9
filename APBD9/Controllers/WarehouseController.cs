using APBD9.Model;
using APBD9.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace APBD09.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController:ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    public async Task<IActionResult> PostObject(Product_Warehouse product_warehouse)
    {
        int price = await _warehouseService.DoesProductExist(product_warehouse.IdProduct);
        if (price == 0)
        {
            return NotFound();
        }

        if (!await _warehouseService.DoesWarehouseExist(product_warehouse.IdWarehouse))
        {
            return NotFound();
        }

        int IdOrder = await _warehouseService.IsProductInOrder(product_warehouse);
        if (IdOrder == 0)
        {
            return NotFound();
        }
        if (!await _warehouseService.IsOrderDone(IdOrder))
        {
            return NotFound();
        }

        try
        {
            await _warehouseService.UpdateOrder(IdOrder);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        try
        {
            int result = await _warehouseService.PostObject(product_warehouse, price, IdOrder);
            return StatusCode(201, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}