using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using JuegoDeMemoria.Models;

namespace JuegoDeMemoria.Services
{
    public class UsuarioService
    {
        private readonly AuthService _auth;
        private readonly ArchivoService _archivo;

        public UsuarioService(AuthService auth, ArchivoService archivo)
        {
            _auth = auth;
            _archivo = archivo;
        }

        public async Task<bool> CrearCuentaAsync(string nombre, string contrasena)
        {
            var usuarios = await _archivo.LeerUsuariosAsync();
            if (usuarios.Any(u => u.NombreUsuario.Equals(nombre, StringComparison.OrdinalIgnoreCase))) return false;
            usuarios.Add(new Usuario { NombreUsuario = nombre, HashContrasena = _auth.CalcularHashSHA256(contrasena), FechaCreacion = DateTime.Now });
            await _archivo.GuardarUsuariosAsync(usuarios);
            return true;
        }

        public async Task<Usuario?> LoginAsync(string nombre, string contrasena)
        {
            var usuarios = await _archivo.LeerUsuariosAsync();
            var hash = _auth.CalcularHashSHA256(contrasena);
            return usuarios.FirstOrDefault(u => u.NombreUsuario.Equals(nombre, StringComparison.OrdinalIgnoreCase) && u.HashContrasena == hash);
        }
    }
}
