using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prueba1.src.DTOs;
using Prueba1.src.Models;

namespace Prueba1.src.Interfaces
{
    public interface IUsuarioRepository
    {
        Task CrearUsuarioAsync(Usuario usuario);
        Task<List<Usuario>> ObtenerTodosASync();
        bool UsuarioExiste(string rut);
        public Task<Usuario?> ActualizarUsuario(int id, ActualizarUsuarioDto usuarioActualizadoDto);
        Task<Usuario?> EliminarUsuario(int id);

    }
}