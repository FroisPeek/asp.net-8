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
        public CommentController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
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
    }
}