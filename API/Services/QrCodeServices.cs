using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using API.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using QRCoder;

namespace API.Services
{
    public class QrCodeServices : IQrCodeServices
    {
        private readonly IWebHostEnvironment _environment;

        public QrCodeServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<byte[]> GenerateQrCode(string text)
        {
            await using var ms = new MemoryStream();
            var oQrCodeGenerator = new QRCodeGenerator();
            var oQrCodeData = oQrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var oQrCode = new QRCode(oQrCodeData);

            using var oBitmap = oQrCode.GetGraphic(2);
            oBitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public Task AddQrCodeFile(string qrCodeImagePath, string qrCodeContainer, string filePath, string fileContainer,
            string secretKey)
        {
            var fileDirectory = "";
            var qrCodeImageDirectory = "";

        
            fileDirectory = Path.Combine(_environment.WebRootPath, fileContainer, Path.GetFileName(filePath));
            qrCodeImageDirectory = Path.Combine(_environment.WebRootPath, qrCodeContainer,
                Path.GetFileName(qrCodeImagePath));
    

            var pdfReader = new PdfReader(fileDirectory);
            var fileStream = new FileStream(fileDirectory.Replace(".pdf", $"{secretKey}.pdf"), FileMode.Create);
            var stamp = new PdfStamper(pdfReader, fileStream );

            var img = Image.GetInstance(qrCodeImageDirectory);

            // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)
            img.SetAbsolutePosition(460, 10);


            var waterMark = stamp.GetOverContent(pdfReader.NumberOfPages);
            waterMark.AddImage(img);
            
            stamp.FormFlattening = true;
            stamp.Close();
            fileStream.Close();

            return Task.FromResult(0);
        }
    }
}