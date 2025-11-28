using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JuegoDeMemoria.Services
{
    public class ImagenService
    {
        private readonly string imagenesPath;

        public ImagenService()
        {
            imagenesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MemoryGame", "imagenes");
            Directory.CreateDirectory(imagenesPath);
        }

        public async Task<List<string>> CopiarImagenesSeleccionadasAsync(IEnumerable<string> rutas)
        {
            var result = new List<string>();
            foreach (var r in rutas)
            {
                var dest = Path.Combine(imagenesPath, Path.GetFileName(r));
                int i = 1;
                var baseName = Path.GetFileNameWithoutExtension(dest);
                var ext = Path.GetExtension(dest);
                while (File.Exists(dest))
                {
                    dest = Path.Combine(imagenesPath, $"{baseName}_{i}{ext}");
                    i++;
                }
                File.Copy(r, dest);
                result.Add(dest);
            }
            return await Task.FromResult(result);
        }

        public List<string> ObtenerRutasImagenesDisponibles()
        {
            var files = Directory.GetFiles(imagenesPath, "*.*");
            return new List<string>(files);
        }

        public BitmapImage ObtenerMiniatura(string ruta, int ancho, int alto)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(ruta);
            bmp.DecodePixelWidth = ancho;
            bmp.DecodePixelHeight = alto;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            return bmp;
        }
    }
}
