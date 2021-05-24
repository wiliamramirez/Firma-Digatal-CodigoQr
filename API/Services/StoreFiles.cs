using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace API.Services
{
    public class StoreFiles : IStoreFiles
    {
        private readonly IWebHostEnvironment _environment;

        public StoreFiles(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveFile(byte[] content, string extension, string container, string contentType = " ")
        {
            var nameFile = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_environment.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nameFile);
            File.WriteAllBytes(ruta, content);
            return ruta;
        }

        public void DeleteFile(string ruta, string container)
        {
            if (ruta != null)
            {
                var nameFile = Path.GetFileName(ruta);
                string directory = Path.Combine(_environment.WebRootPath, container, nameFile);

                if (File.Exists(directory))
                {
                    File.Delete(directory);
                }
            }
        }
    }
}