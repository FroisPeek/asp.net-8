using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")] // estamos dizendo que a rota do controller é api/stock
    [ApiController] // estamos dizendo que é um controller
    public class StockController : ControllerBase   // ControllerBase é uma classe que tem metodos que podemos usar
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("getStock")]
        public async Task<IActionResult> GetStock()
        {
            var stocks = await _context.Stock.ToListAsync();

            var response = stocks.Select(s => s.ReadToStockDto()); // momento em que usamos nosso dto
            return Ok(stocks);
        }

        [HttpGet("getStockById/{id}")]
        public IActionResult GetStockById([FromRoute] int id) // estamos pegando o id da rota
        {
            // FirstOrDefault -> estamos fazendo uma busca de dados na tabela Stock, no qual vai retornar o primeiro resultado que encontrar.
            var stock = _context.Stock.Find(id); // Usar o find para buscar diretamente esse ID

            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock.ReadToStockDto());
        }

        [HttpPost("createStock")]
        public IActionResult Create([FromBody] CreateStockDto stockDto)  // FromBody pois os dados vão ser enviados em um JSON
        {
            var stockModel = stockDto.CreateStockDto();
            _context.Stock.Add(stockModel);  // estamos adicionando o stockModel na tabela Stock
            _context.SaveChanges();  // alterações sendo salvas

            return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ReadToStockDto());
        }

        [HttpPut("updateStock/{id}")]
        public IActionResult UpDate([FromRoute] int id, [FromBody] UpdateStock upDateDto)
        {
            var stockModel = _context.Stock.FirstOrDefault(s => s.Id == id); // estamos buscando o id na tabela Stock

            if (stockModel == null)
            {
                return NotFound();
            }

            stockModel.Symbol = upDateDto.Symbol;
            stockModel.CompanyName = upDateDto.CompanyName;
            stockModel.Purchase = upDateDto.Purchase;
            stockModel.LastDiv = upDateDto.LastDiv;
            stockModel.Industry = upDateDto.Industry;
            stockModel.MarketCap = upDateDto.MarketCap;

            _context.SaveChanges(); // alterações sendo salvas

            return Ok(stockModel.ReadToStockDto());
        }

        [HttpDelete("deleteStock/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var stockModel = _context.Stock.FirstOrDefault(s => s.Id == id); // estamos buscando o id na tabela Stock

            if (stockModel == null)
            {
                return NotFound();
            }

            _context.Stock.Remove(stockModel); // estamos removendo o stockModel da tabela Stock
            _context.Comment.RemoveRange(_context.Comment.Where(c => c.StockId == id)); // estamos removendo os comentarios que tem o id do stockModel  
            _context.SaveChanges(); // alterações sendo salvas

            return StatusCode(204);  // estamos retornando um status 204
        }
    }
}