using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace API.Controllers
{
    public class DocumentsController : BaseApiController
    {
        private readonly IStoreFilesServices _storeFiles;
        private readonly DataContext _context;
        private readonly IQrCodeServices _qrCode;
        private readonly string _documentContainer = "Documents";
        private readonly string _codeQrContainer = "QrCodeImages";

        public DocumentsController(IStoreFilesServices storeFiles, DataContext context, IQrCodeServices qrCode)
        {
            _storeFiles = storeFiles;
            _context = context;
            _qrCode = qrCode;
        }

        [HttpPost]
        public async Task<ActionResult<Document>> Post([FromForm] AddDocumentDto addDocumentDto)
        {
            if (addDocumentDto.File == null)
            {
                return BadRequest();
            }

            await using var memoryStream = new MemoryStream();
            addDocumentDto.File.CopyTo(memoryStream);
            var content = memoryStream.ToArray();
            var extension = Path.GetExtension(addDocumentDto.File.FileName);
            var documentPath = await
                _storeFiles.SaveFile(content, extension, _documentContainer, addDocumentDto.File.ContentType);

            var hashDocument = CalculateMd5(documentPath);
            var urlFile = _storeFiles.GetUrl(_documentContainer, Path.GetFileName(documentPath));

            var documentDto = new DocumentDto
            {
                Affair = addDocumentDto.Affair,
                Title = addDocumentDto.Title,
                Url = urlFile,
                User = "Pepito",
                Hash = hashDocument
            };

            var contentQrCode = await _qrCode.GenerateQrCode(ConvertString(documentDto));

            var qrCodeImagePath = await _storeFiles.SaveFile(contentQrCode, ".png", _codeQrContainer);

            await _qrCode.AddQrCodeFile(
                qrCodeImagePath,
                _codeQrContainer,
                documentPath,
                _documentContainer);

            var hashSecret = CalculateMd5(documentPath);

            var document = new Document
            {
                /*Id = hashDocument,*/
                Id = Guid.NewGuid().ToString(),
                Affair = addDocumentDto.Affair,
                Url = urlFile,
                Title = addDocumentDto.Title,
                User = "Ramirez Gutierrez, Wiliam Eduar",
                HashSecret = hashSecret
            };

            _context.Documents.Add(document);
            var resultContext = await _context.SaveChangesAsync();


            if (resultContext > 0)
            {
                /*_storeFiles.DeleteFile(documentPath, containerFile);
                _storeFiles.DeleteFile(qrCodeImagePath, containerCodeQr);*/
                return Ok(document);
            }

            return BadRequest();
        }

        private string CalculateMd5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = System.IO.File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private string ConvertString(DocumentDto obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return json;
        }
    }
}