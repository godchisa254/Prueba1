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
        [Route("/user")]
        public async Task<IActionResult> ObtenerTodosUsuarios(string? genero, string? sort)
        {
            var generos = new[] { "masculino", "femenino", "otro", "prefiero no decirlo" };
            var usuarios = await _usuarioRepo.ObtenerTodosASync();

            var usuarioDTO = usuarios.Select(u => new ActualizarUsuarioDto
            {
                Rut = u.Rut,
                Nombre = u.Nombre,
                Email = u.Email,
                Genero = u.Genero,
                FechaNacimiento = u.FechaNacimiento
            }).ToList();

            if (string.IsNullOrWhiteSpace(genero) && string.IsNullOrWhiteSpace(sort))
            {
                return Ok(usuarioDTO);
            }

            if (!string.IsNullOrWhiteSpace(genero) && !generos.Contains(genero.ToLower()))
            {
                return BadRequest("El filtro para el género debe ser 'masculino', 'femenino', 'otro' o 'prefiero no decirlo'.");
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                if (sort == "asc")
                {
                    usuarios = usuarios.OrderBy(u => u.Nombre).ToList();
                }
                else if (sort == "desc")
                {
                    usuarios = usuarios.OrderByDescending(u => u.Nombre).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(genero))
            {
                usuarios = usuarios
                    .Where(u => u.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            usuarioDTO = usuarios.Select(u => new ActualizarUsuarioDto
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
            if (usuarioActualizadoDto.FechaNacimiento >= DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest("La fecha de nacimiento debe ser anterior a la fecha actual.");
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
            return Created($"/product/{usuario.Rut}", usuarioActualizadoDto);
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