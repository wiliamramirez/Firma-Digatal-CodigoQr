using System.Threading.Tasks;

namespace API.Services
{
    public interface IStoreFiles
    {
        string SaveFile(byte[] content, string extension, string container, string contentType = " ");
        void DeleteFile(string ruta, string container);
    }
}