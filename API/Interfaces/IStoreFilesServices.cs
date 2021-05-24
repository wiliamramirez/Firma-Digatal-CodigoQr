using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IStoreFilesServices
    {
        Task<string> SaveFile(byte[] content, string extension, string container, string contentType = " ");
        Task<string> SaveFileAzure(byte[] content, string extension, string container, string contentType);
        Task DeleteFile(string ruta, string container);
        string GetUrl(string container, string nameFile);
    }
}