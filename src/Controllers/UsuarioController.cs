using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prueba1.src.DTOs;
using Prueba1.src.Interfaces;
using Prueba1.src.Models;

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

            if (!generos.Contains(genero.ToLower()))
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

        [HttpPut]
        [Route("/user/{id}")]
        public async Task<IActionResult> EditarUsuario([FromRoute] int id, [FromBody] ActualizarUsuarioDto usuarioActualizadoDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var modeloUsuario = await _usuarioRepo.ActualizarUsuario(id, usuarioActualizadoDto);
            if (modeloUsuario == null)
            {
                return NotFound();
            }
            return Ok(modeloUsuario);
        }

        [HttpPost]
        [Route("/user")]
        public async Task<IActionResult> CrearUsuario([FromBody] ActualizarUsuarioDto usuarioActualizadoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuario = new Usuario
            {
                Rut = usuarioActualizadoDto.Rut,
                Nombre = usuarioActualizadoDto.Nombre,
                Email = usuarioActualizadoDto.Email,
                Genero = usuarioActualizadoDto.Genero,
                FechaNacimiento = usuarioActualizadoDto.FechaNacimiento
            };
            
            var generos = new[] { "masculino", "femenino", "otro", "prefiero no decirlo" };

            if (string.IsNullOrEmpty(usuario.Genero) || !generos.Contains(usuario.Genero.ToLower()))
            {
                return BadRequest("El filtro para el género debe ser 'masculino', 'femenino', 'otro' o 'prefiero no decirlo'.");
            }

            if(_usuarioRepo.UsuarioExiste(usuario.Rut))
            {
                return Conflict("El RUT ya existe.");
            }
            await _usuarioRepo.CrearUsuarioAsync(usuario);
            return Created($"/product/{usuario.Rut}", usuario);
        }

        [HttpDelete("/user/{id}")]
        public async Task<IActionResult> EliminarUsuario([FromRoute] int id)
        {
            var usuario = await _usuarioRepo.EliminarUsuario(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
    }
}