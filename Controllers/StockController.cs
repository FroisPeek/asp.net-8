using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")] // estamos dizendo que a rota do controller é api/stock
    [ApiController] // estamos dizendo que é um controller
    public class StockController : ControllerBase   // ControllerBase é uma classe que tem metodos que podemos usar
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepositoy _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepositoy stockRepo)
        {
            _context = context;
            _stockRepo = stockRepo;
        }

        [HttpGet("getStock")]
        public async Task<IActionResult> GetStock()
        {
            var response = new Response<IEnumerable<StockDto>>(); // classe criada para padronizar as respostas
            try
            {
                var stocks = await _stockRepo.GetAllAsync(); // estamos chamando o metodo GetAllAsync da interface IStockRepositoy

                response.Data = stocks.Select(s => s.ReadToStockDto()); // momento em que usamos nosso dto
                response.Success = true;
                response.Message = "Dados retornados com sucesso";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }

        }

        [HttpGet("getStockById/{stockId:int}")]
        public async Task<IActionResult> GetStockById([FromRoute] int stockId) // estamos pegando o id da rota
        {
            var response = new Response<StockDto>();
            try
            {
                // FirstOrDefault -> estamos fazendo uma busca de dados na tabela Stock, no qual vai retornar o primeiro resultado que encontrar.

                var stock = await _stockRepo.GetByIdAsync(stockId); // Usar o FindAsync para buscar diretamente esse ID

                if (stock == null)
                {
                    response.Success = false;
                    response.Message = "Nenhum Stock encontrado";
                    return NotFound(response);
                }

                response.Data = stock.ReadToStockDto();
                response.Success = true;
                response.Message = "Stock encontrado com sucesso";
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("createStock")]
        public async Task<IActionResult> Create([FromBody] CreateStockDto stockDto)  // FromBody pois os dados vão ser enviados em um JSON
        {
            var response = new Response<StockDto>();
            try
            {
                var stockModel = stockDto.CreateStockDto();
                await _stockRepo.CreateAsync(stockModel);

                response.Data = stockModel.ReadToStockDto();
                response.Success = true;
                response.Message = "Stock criado com sucesso";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }
        }

        [HttpPut("updateStock/{id:int}")]
        public async Task<IActionResult> UpDate([FromRoute] int id, [FromBody] UpdateStock upDateDto)
        {
            var response = new Response<UpdateStock>();
            try
            {
                var stockModel = await _stockRepo.UpdateAync(id, upDateDto); // estamos buscando o id na tabela Stock

                if (stockModel == null)
                {
                    return NotFound();
                }

                response.Data = upDateDto;
                response.Message = "Stock atualizado com sucesso";
                response.Success = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }
        }

        [HttpDelete("deleteStock/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = new Response<StockDto>();
            try
            {
                var stockModel = await _stockRepo.DeleteAsync(id); // estamos buscando o id na tabela Stock

                if (stockModel == null)
                {
                    response.Message = "Stock não encontrado";
                    return NotFound();
                }

                response.Success = true;
                response.Message = "Stock deletado com sucesso";
                return Ok(response);  // estamos retornando um status 204
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("searchStock")]
        public async Task<IActionResult> search([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = new Response<IEnumerable<StockDto>>();
            try
            {
                var stock = _context.Stock.AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.Symbol))
                {
                    stock = stock.Where(s => s.Symbol.Contains(query.Symbol));
                }

                if (!string.IsNullOrWhiteSpace(query.CompanyName))
                {
                    stock = stock.Where(s => s.CompanyName.Contains(query.CompanyName));
                }

                var stockFinal = await stock.ToListAsync();

                response.Data = stockFinal.Select(s => s.ReadToStockDto());
                response.Success = true;
                response.Message = "Stock encontrado com sucesso";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest();
            }
        }
    }
}