using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepositoy
    {
        private readonly ApplicationDBContext _context; // estamos criando uma variavel do tipo ApplicationDBContext
        public StockRepository(ApplicationDBContext context)  // injeção de dependencia
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAllAsync()
        {
            return await _context.Stock.Include(c => c.Comments).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stock.Include(c => c.Comments).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            _context.Stock.Add(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAync(int id, UpdateStock stockDto)
        {
            var stockExist = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);
            if (stockExist == null)
            {
                return null;
            }

            stockExist.Symbol = stockDto.Symbol;
            stockExist.CompanyName = stockDto.CompanyName;
            stockExist.Purchase = stockDto.Purchase;
            stockExist.LastDiv = stockDto.LastDiv;
            stockExist.Industry = stockDto.Industry;
            stockExist.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();
            return stockExist;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
            {
                return null;
            }
            _context.Remove(stock);
            _context.Comment.RemoveRange(_context.Comment.Where(c => c.StockId == id)); // estamos removendo os comentarios que tem o id do stockModel  
            await _context.SaveChangesAsync();
            return stock;
        }

        public Task<bool> StockExists(int stockId)
        {
            return _context.Stock.AnyAsync(s => s.Id == stockId); // Any ==> return true or false 
        }
    }
}