
namespace API.Interfaces
{
    public interface IStoreFilesServices
    {
        string SaveFile(byte[] content, string extension, string container, string contentType = " ");
        string SaveFileAzure(byte[] content, string extension, string container, string contentType);
        void DeleteFile(string ruta, string container);
        string GetUrl(string container, string nameFile);
    }
}