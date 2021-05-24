namespace API.Interfaces
{
    public interface IQrCodeServices
    {
        byte[] GenerateQrCode(string text);
        void AddQrCodeFile(string qrCodeImagePath, string qrCodeContainer, string filePath, string fileContainer);
    }
}