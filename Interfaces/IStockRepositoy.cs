using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IStockRepositoy
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id); // ? -> pois pode vir null
        Task<Stock> CreateAsync(Stock stockModel);
        Task<Stock?> UpdateAync(int id, UpdateStock stockDto);
        Task<Stock?> DeleteAsync(int id);
        Task<bool> StockExists(int stockId);
    }
}