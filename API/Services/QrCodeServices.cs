using System.Drawing.Imaging;
using System.IO;
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

        public byte[] GenerateQrCode(string text)
        {
            using var ms = new MemoryStream();
            var oQrCodeGenerator = new QRCodeGenerator();
            var oQrCodeData = oQrCodeGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var oQrCode = new QRCode(oQrCodeData);

            using var oBitmap = oQrCode.GetGraphic(2);
            oBitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public void AddQrCodeFile(string qrCodeImagePath, string qrCodeContainer, string filePath, string fileContainer)
        {
            var fileDirectory = "";
            var qrCodeImageDirectory = "";

            if (qrCodeImagePath != null && filePath != null)
            {
                fileDirectory = Path.Combine(_environment.WebRootPath, fileContainer, Path.GetFileName(filePath));
                qrCodeImageDirectory = Path.Combine(_environment.WebRootPath, qrCodeContainer,
                    Path.GetFileName(qrCodeImagePath));
            }

            if (File.Exists(fileDirectory) && File.Exists(qrCodeImageDirectory))
            {
                var pdfReader = new PdfReader(fileDirectory);
                var stamp = new PdfStamper(pdfReader,
                    new FileStream(fileDirectory.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

                var img = Image.GetInstance(qrCodeImageDirectory);

                // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)
                img.SetAbsolutePosition(360, 10);


                var waterMark = stamp.GetOverContent(pdfReader.NumberOfPages);
                waterMark.AddImage(img);

                stamp.FormFlattening = true;
                stamp.Close();

                // now delete the original file and rename the temp file to the original file
                File.Delete(fileDirectory);
                File.Move(fileDirectory.Replace(".pdf", "[temp][file].pdf"), fileDirectory);
            }
        }
    }
}