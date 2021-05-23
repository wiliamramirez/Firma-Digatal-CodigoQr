using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using API.DTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public ActionResult<Data> Post([FromForm] AddDocumentDto documentDto)
        {
            string container = "Test";
            string filePath = Path.GetFileName("/Test/imd.pdf");
            string sourceFilePath = Path.Combine(_environment.WebRootPath, container, filePath);
            string hashDocument = CalculateMd5(sourceFilePath);

            var data = new Data
            {
                Id = Guid.NewGuid(),
                Affair = documentDto.Affair,
                Hash = hashDocument,
                Url = $"http:enlaceURl/{hashDocument}",
                Title = documentDto.Title,
                User = "Ramirez Gutierrez, Wiliam Eduar"
            };

            var codeQr = GenerateQr(data);
            string imageCodeQrPath = SaveImageCodeQr(codeQr, ".png");

            var result = PdfStampWithNewFile(imageCodeQrPath, sourceFilePath);

            string hashSecret = CalculateMd5(sourceFilePath);
            data.HashSecret = hashSecret;

            if (result)
            {
                return Ok(data);
            }

            return NotFound();
        }

        private string CalculateMd5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private byte[] GenerateQr(Data data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator oQrCodeGenerator = new QRCodeGenerator();
                QRCodeData oQrCodeData =
                    oQrCodeGenerator.CreateQrCode(JsonConvert.SerializeObject(data), QRCodeGenerator.ECCLevel.Q);
                QRCode oQrCode = new QRCode(oQrCodeData);

                using (Bitmap oBitmap = oQrCode.GetGraphic(3))
                {
                    oBitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private string SaveImageCodeQr(byte[] content, string extension)
        {
            string container = "CodeQr";
            var nameFile = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_environment.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nameFile);
            System.IO.File.WriteAllBytes(ruta, content);

            return ruta;
        }


        private bool PdfStampWithNewFile(string watermarkLocation, string fileLocation)
        {
            Document document = new Document();
            PdfReader pdfReader = new PdfReader(fileLocation);
            PdfStamper stamp = new PdfStamper(pdfReader,
                new FileStream(fileLocation.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(watermarkLocation);

            // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)
            img.SetAbsolutePosition(360, 10);


            PdfContentByte waterMark;
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                waterMark = stamp.GetOverContent(page);
                waterMark.AddImage(img);
            }

            stamp.FormFlattening = true;
            stamp.Close();

            // now delete the original file and rename the temp file to the original file
            System.IO.File.Delete(fileLocation);
            System.IO.File.Move(fileLocation.Replace(".pdf", "[temp][file].pdf"), fileLocation);

            return true;
        }
    }
}