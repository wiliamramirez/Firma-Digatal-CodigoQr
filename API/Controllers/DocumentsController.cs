using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using API.DTOs;
using API.Services;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IStoreFiles _storeFiles;
        private readonly string containerFile = "File";
        private readonly string containerCodeQr = "CodeQr";

        public DocumentsController(IStoreFiles storeFiles)
        {
            _storeFiles = storeFiles;
        }

        [HttpPost]
        public ActionResult<Data> Post([FromForm] AddDocumentDto documentDto)
        {
            var sourceDocumentPath = "";

            if (documentDto.File != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    documentDto.File.CopyTo(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(documentDto.File.FileName);
                    sourceDocumentPath =
                        _storeFiles.SaveFile(content, extension, containerFile, documentDto.File.ContentType);
                }
            }

            string hashDocument = CalculateMd5(sourceDocumentPath);

            var data = new Data
            {
                Id = Guid.NewGuid(),
                Affair = documentDto.Affair,
                Hash = hashDocument,
                Url = $"http:enlaceURl/{hashDocument}",
                Title = documentDto.Title,
                User = "Ramirez Gutierrez, Wiliam Eduar"
            };

            var contentCodeQr = GenerateQr(data);
            string sourceImageCodeQrPath = _storeFiles.SaveFile(contentCodeQr, ".png", containerCodeQr);

            var result = PdfStampWithNewFile(sourceImageCodeQrPath, sourceDocumentPath);
            string hashSecret = CalculateMd5(sourceDocumentPath);

            data.HashSecret = hashSecret;

            if (result)
            {
                _storeFiles.DeleteFile(sourceDocumentPath, containerFile);
                _storeFiles.DeleteFile(sourceImageCodeQrPath, containerCodeQr);
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

                using (Bitmap oBitmap = oQrCode.GetGraphic(2))
                {
                    oBitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }


        private bool PdfStampWithNewFile(string watermarkLocation, string fileLocation)
        {
            PdfReader pdfReader = new PdfReader(fileLocation);
            PdfStamper stamp = new PdfStamper(pdfReader,
                new FileStream(fileLocation.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(watermarkLocation);

            // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)
            img.SetAbsolutePosition(360, 10);


            PdfContentByte waterMark;

            waterMark = stamp.GetOverContent(pdfReader.NumberOfPages);
            waterMark.AddImage(img);

            stamp.FormFlattening = true;
            stamp.Close();

            // now delete the original file and rename the temp file to the original file
            System.IO.File.Delete(fileLocation);
            System.IO.File.Move(fileLocation.Replace(".pdf", "[temp][file].pdf"), fileLocation);

            return true;
        }
    }
}