namespace API.Interfaces
{
    public interface IQrCodeServices
    {
        byte[] GenerateQrCode(string text);
        void AddQrCodeFile(string rutaQrCodeImage, string containerQrCode, string rutaFile, string containerFile);
    }
}