using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IQrCodeServices
    {
        Task<byte[]> GenerateQrCode(string text);
        Task AddQrCodeFile(string qrCodeImagePath, string qrCodeContainer, string filePath, string fileContainer);
    }
}