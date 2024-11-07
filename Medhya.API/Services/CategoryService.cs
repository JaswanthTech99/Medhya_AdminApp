using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Medhya.API.Services;

public class CategoryService : ICategoryRepository
{
    private readonly DapperContext _dapperContext;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(DapperContext dapperContext, ILogger<CategoryService> logger)
    {
        _dapperContext = dapperContext;
        _logger = logger;
    }

    public async Task<Category> Upsert(Category category)
    {
        try
        {
            var param = new DynamicParameters();
            param.Add("Category", category.CategoryName);
            param.Add("CategoryStatus", category.CategoryStatus, dbType: DbType.String);
            if(category.Id != 0)
                param.Add("Id", category.Id, dbType: DbType.Int32);
            const string storedProcedure = "dbo.USP_MANAGE_CATEGORY";
            using (var connection = _dapperContext.CreateConnection())
            {

                var id = await connection.QueryFirstOrDefaultAsync<int>(storedProcedure, param, commandType: CommandType.StoredProcedure);
                return category = new Category { CategoryName = category.CategoryName, CategoryStatus = category.CategoryStatus, Id = id };

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
        
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        var storedProcedure = "dbo.USP_LIST_CATEGORY";
        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryAsync<Category>(storedProcedure, commandType: CommandType.StoredProcedure);
            return result;
        }
    }

    public async Task<Category> GetCategoryById(int id)
    {
        var storedProcedure = "dbo.USP_CATEGORYDETAILS_ID";
        var param = new DynamicParameters();
        param.Add("Id", id, dbType: DbType.Int32);
        using (var connection = _dapperContext.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<Category>(storedProcedure, param, commandType: CommandType.StoredProcedure);
            return result;
        }
    }

}

