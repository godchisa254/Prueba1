using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba1.src.DTOs
{
    public class ActualizarUsuarioDto
    {
        
        public required string Rut { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public required string Nombre { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        public required string Email { get; set; }
        [RegularExpression("^(masculino|femenino|otro|prefiero no decirlo)$", ErrorMessage = "El género debe ser 'masculino', 'femenino', 'otro' o 'prefiero no decirlo'.")]
        public required string Genero { get; set; }
        [DataType(DataType.Date, ErrorMessage = "La fecha debe ser anterior a la actual.")]
        public required DateTime FechaNacimiento { get; set; }
    }
}