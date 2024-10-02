using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prueba1.src.Data;
using Prueba1.src.DTOs;
using Prueba1.src.Interfaces;
using Prueba1.src.Models;

namespace Prueba1.src.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDBContext _context;
        public UsuarioRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Usuario?> ActualizarUsuario(int id, ActualizarUsuarioDto usuarioActualizadoDto)
        {
            var modeloUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (modeloUsuario == null)
            {
                throw new Exception("Product not found");
            }
            modeloUsuario.Nombre = usuarioActualizadoDto.Nombre;
            modeloUsuario.Rut = usuarioActualizadoDto.Rut;
            modeloUsuario.Email = usuarioActualizadoDto.Email;
            modeloUsuario.Genero = usuarioActualizadoDto.Genero;
            modeloUsuario.FechaNacimiento = usuarioActualizadoDto.FechaNacimiento;

            await _context.SaveChangesAsync();
            return modeloUsuario;
        }

        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> EliminarUsuario(int id)
        {
            var modeloUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (modeloUsuario == null)
            {
                return null;
            }

            _context.Usuarios.Remove(modeloUsuario);
            await _context.SaveChangesAsync();
            return modeloUsuario;
        }

        public async Task<List<Usuario>> ObtenerTodosASync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public bool UsuarioExiste(string rut)
        {
            return _context.Usuarios.Any(p => p.Rut == rut);
        }
    }
}