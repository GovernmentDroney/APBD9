using APBD9.Model;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Services;

using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

public class WarehouseService : IWarehouseService
{
    private readonly IConfiguration _configuration;

    public WarehouseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task <int> DoesProductExist(int IdProduct)
    {
        int price = 0;
        string command = "SELECT Price From Product where IdProduct = @IdProduct";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdProduct", IdProduct);
            var res = await cmd.ExecuteScalarAsync();
            if (res != null)
            {
               price = Convert.ToInt32(res); 
            }
        }

        return price;
    }

    public async Task <bool> DoesWarehouseExist(int IdWarehouse)
    {
        string command = "SELECT 1 From Warehouse where IdWarehouse = @IdWarehouse";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }     }

    public async Task<int> IsProductInOrder(Product_Warehouse product_warehouse)
    {
        int OrderId = 0;
        //mozliwe że zły sql
        string command = "SELECT IdOrder FROM Order WHERE (IdProduct = @IdProduct) AND (Amount> @Amount) AND (CreatedAt>@CreatedAt)";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdProduct", product_warehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", product_warehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt", product_warehouse.CreatedAt);
            var res = await cmd.ExecuteScalarAsync();
            if (res != null)
            {
                OrderId = Convert.ToInt32(res);
            }
        }
        return OrderId;
    }

    public async Task<bool> IsOrderDone(int IdOrder)
    {
        string command = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdOrder", IdOrder);
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        } 
    }

    public async Task UpdateOrder(int idOrder)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText = "UPDATE Order SET FullfilledAt = DateTime.Now WHERE IdOrder = @IdOrder";
            command.Parameters.AddWithValue("@IdOrder", idOrder);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<int> PostObject(Product_Warehouse product_warehouse, int price,int idOrder)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText =
                "INSERT INTO Product_Warehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder,@Amount,@Price,@CreatedAt)SELECT @@IDENTITY";
            command.Parameters.AddWithValue("@IdWarehouse", product_warehouse.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", product_warehouse.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", product_warehouse.Amount);
            command.Parameters.AddWithValue("@Price", price*product_warehouse.Amount);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            var result = Convert.ToInt32(await command.ExecuteScalarAsync());
            return result;
    
        } 
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
 