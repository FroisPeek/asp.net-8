using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class UpDateCoomentDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Titulo deve conter mais que 3 caracteres")]
        [MaxLength(20, ErrorMessage = "Titulo deve conter no máximo 20 caracteres")]
        public string Title { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Deve conter mais que 3 caracteres")]
        [MaxLength(280, ErrorMessage = "Deve conter no máximo 280 caracteres")]
        public string Content { get; set; }
    }
}