using System;
using System.IO;
using API.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class StoreFilesServices : IStoreFilesServices
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _configuration;

        public StoreFilesServices(IWebHostEnvironment environment, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration.GetConnectionString("AzureStorage");
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

        public string SaveFileAzure(byte[] content, string extension, string container, string contentType)
        {
            throw new NotImplementedException();
        }


        public void DeleteFile(string ruta, string container)
        {
            if (ruta != null)
            {
                var nameFile = Path.GetFileName(ruta);
                var directory = Path.Combine(_environment.WebRootPath, container, nameFile);

                if (File.Exists(directory))
                {
                    File.Delete(directory);
                }
            }
        }

        public string GetUrl(string container, string nameFile)
        {
            var urlCurrent =
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var url = Path.Combine(urlCurrent, container, nameFile).Replace("\\", "/");

            return url;
        }
    }
}