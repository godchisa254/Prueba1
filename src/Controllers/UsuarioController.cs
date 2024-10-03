using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prueba1.src.DTOs;
using Prueba1.src.Interfaces;

namespace Prueba1.src.Controllers
{
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo;
        public UsuarioController(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        [HttpGet]
        [Route("/user/{genero}")]
        public async Task<IActionResult> ObtenerTodosUsuarios(string genero)
        {
            var generos = new[] { "masculino", "femenino", "otro", "prefiero no decirlo" };

            if (string.IsNullOrEmpty(genero) || !generos.Contains(genero.ToLower()))
            {
                return BadRequest("El filtro para el género debe ser 'masculino', 'femenino', 'otro' o 'prefiero no decirlo'.");
            }
            var usuarios = await _usuarioRepo.ObtenerTodosASync();
            
            var usuarioDTO = usuarios
            .Where(u => u.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase)) //ignora mayusculas y minusculas
            .Select(u => new ActualizarUsuarioDto
            {
                Rut = u.Rut,
                Nombre = u.Nombre,
                Email = u.Email,
                Genero = u.Genero,
                FechaNacimiento = u.FechaNacimiento
            }).ToList();
            
            return Ok(usuarioDTO);
        }
    }
}