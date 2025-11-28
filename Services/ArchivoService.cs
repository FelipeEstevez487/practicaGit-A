using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using JuegoDeMemoria.Models;

namespace JuegoDeMemoria.Services
{
    public class ArchivoService
    {
        private readonly string basePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MemoryGame");
        private readonly string usuariosFile;
        private readonly string resultadosFile;

        public ArchivoService()
        {
            usuariosFile = Path.Combine(basePath, "usuarios.json");
            resultadosFile = Path.Combine(basePath, "resultados.json");
            Directory.CreateDirectory(basePath);
        }

        public async Task<List<Usuario>> LeerUsuariosAsync()
        {
            if (!File.Exists(usuariosFile)) return new List<Usuario>();
            var txt = await File.ReadAllTextAsync(usuariosFile);
            return JsonSerializer.Deserialize<List<Usuario>>(txt) ?? new List<Usuario>();
        }

        public async Task GuardarUsuariosAsync(List<Usuario> usuarios)
        {
            var txt = JsonSerializer.Serialize(usuarios, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(usuariosFile, txt);
        }

        public async Task<List<ResultadoPartida>> LeerResultadosAsync()
        {
            if (!File.Exists(resultadosFile)) return new List<ResultadoPartida>();
            var txt = await File.ReadAllTextAsync(resultadosFile);
            return JsonSerializer.Deserialize<List<ResultadoPartida>>(txt) ?? new List<ResultadoPartida>();
        }

        public async Task GuardarResultadosAsync(ResultadoPartida resultado)
        {
            var lista = await LeerResultadosAsync();
            lista.Add(resultado);
            var txt = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(resultadosFile, txt);
        }

        public void ExportarResultadosACSV(string rutaDestino)
        {
            var lista = LeerResultadosAsync().Result;
            using var sw = new StreamWriter(rutaDestino, false);
            sw.WriteLine("Usuario,Puntaje,Movimientos,DuracionSegundos,Nivel,Fecha");
            foreach (var r in lista)
            {
                sw.WriteLine($"\"{r.NombreUsuario}\",{r.Puntaje},{r.Movimientos},{r.DuracionSegundos},{r.Nivel},{r.Fecha:O}");
            }
        }
    }
}
