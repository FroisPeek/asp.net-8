using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
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
        public Task<List<Stock>> GetAllAsync()
        {
            return _context.Stock.ToListAsync();
        }
    }
}