using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Dtos.Comment;
using api.Mappers;


namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepositoy _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepositoy stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new Response<IEnumerable<CommentDto>>();
            try
            {
                var comment = await _commentRepo.GetAllAsync();
                response.Data = comment.Select(c => c.ToReadCommentDto());
                response.Success = true;
                response.Message = "Comentarios encontrados com sucesso";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("getCommentById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new Response<CommentDto>();
            try
            {
                var comment = await _commentRepo.GetByIdAsync(id);
                if (comment == null)
                {
                    response.Success = false;
                    response.Message = "Comment não encontrado";
                    return NotFound();
                }

                response.Data = comment.ToReadCommentDto();
                response.Success = true;
                response.Message = "Comment encontrado com sucesso";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return NotFound();
            }
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, CreateCommentDto commentDto)
        {
            var response = new Response<CommentDto>();
            try
            {
                if (!await _stockRepo.StockExists(stockId))
                {
                    response.Success = false;
                    response.Message = "Nenhum stock encontrado";
                    return NotFound();
                }

                var commentModel = commentDto.ToCreateCommentDto(stockId);
                await _commentRepo.CreateAsync(commentModel);

                response.Data = commentModel.ToReadCommentDto();
                response.Success = true;
                response.Message = "Comment criado com sucesso";
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> EditComment([FromRoute] int id, [FromBody] UpDateCoomentDto upDateComment)
        {
            var response = new Response<CommentDto>();
            try
            {
                var comment = await _commentRepo.UpDateAsync(id, upDateComment.ToUpDateCommentDto());
                if (comment == null)
                {
                    response.Success = false;
                    response.Message = "Comment não encontrado";
                    return NotFound();
                }

                response.Data = comment.ToReadCommentDto();
                response.Success = true;
                response.Message = "Comment atualizado com sucesso";
                return Ok(response);

            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var response = new Response<CommentDto>();
            try
            {
                var comment = await _commentRepo.DeleteAsync(id);

                if (comment == null)
                {
                    response.Success = false;
                    response.Message = "Comment não encontrado";
                    return NotFound();
                }

                response.Data = comment.ToReadCommentDto();
                response.Success = true;
                response.Message = "Comment deletado com sucesso";
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