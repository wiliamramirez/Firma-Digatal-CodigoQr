using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    /*localhost:5000/api/Documents/listar*/
    public class DocumentsController : BaseApiController
    {
        private readonly IStoreFilesServices _storeFiles;
        private readonly DataContext _context;
        private readonly IQrCodeServices _qrCode;
        private readonly IMapper _mapper;
        private readonly string _documentContainer = "Documents";
        private readonly string _documentContainerCheck = "DocumentCheck";
        private readonly string _codeQrContainer = "QrCodeImages";
        private readonly string _secretKey = "-SH";

        public DocumentsController(IStoreFilesServices storeFiles, DataContext context, IQrCodeServices qrCode,
            IMapper mapper)
        {
            _storeFiles = storeFiles;
            _context = context;
            _qrCode = qrCode;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<DocumentDto>> Post([FromForm] IFormFile file)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == User.GetId());

            if (file == null)
            {
                return BadRequest();
            }

            /* Leer documento y alamcenar*/
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();
            var extension = Path.GetExtension(file.FileName);
            var documentPath = await
                _storeFiles.SaveFile(content, extension, _documentContainer, file.ContentType);

            /*  */
            var urlFile = _storeFiles.GetUrl(_documentContainer, Path.GetFileName(documentPath));

            /*nombre-SH.pdf*/

            /*url de la ruta del archivo*/
            var finalUrlDocument = urlFile.Replace(".pdf", $"{_secretKey}.pdf");

            var finalPathDocument = documentPath.Replace(".pdf", $"{_secretKey}.pdf");

            /*START- CREAR CODIGO QR*/
            var contentQrCode = await _qrCode.GenerateQrCode(finalUrlDocument);

            /*Ruta del codigo qr */
            var qrCodeImagePath = await _storeFiles.SaveFile(contentQrCode, ".png", _codeQrContainer);
            /*END- CREAR CODIGO QR*/


            /* START - agregar el codigo qr al documento */
            await _qrCode.AddQrCodeFile(
                qrCodeImagePath,
                _codeQrContainer,
                documentPath,
                _documentContainer,
                _secretKey);
            /* END - agregar el codigo qr al documento */


            /*Generando hash del documento mas el codigo qr*/
            var hashSecret = CalculateMd5(finalPathDocument);

            var document = new Document
            {
                Id = Guid.NewGuid(),
                Url = finalUrlDocument,
                AppUser = user
            };

            _context.Documents.Add(document);


            var documentDetails = new DocumentDetail
            {
                Id = Guid.NewGuid(),
                Affair = "cualquier cosa",
                Title = "otra cosa que no es la anterior",
                User = User.GetSurname(),
                HashSecret = hashSecret,
                Document = document
            };
            _context.DocumentDetails.Add(documentDetails);

            var resultContext = await _context.SaveChangesAsync();


            var documentDto = new DocumentDto
            {
                Id = document.Id,
                Affair = "Asunto",
                Title = "Titulo",
                Url = finalUrlDocument,
                User = User.GetSurname(),
                Hash = hashSecret,
            };


            if (resultContext > 0)
            {
                await _storeFiles.DeleteFile(documentPath, _documentContainer);
                await _storeFiles.DeleteFile(qrCodeImagePath, _codeQrContainer);
                return Ok(documentDto);
            }

            return BadRequest();
        }

        /*/api/documents/check*/
        [HttpPost("check")]
        public async Task<ActionResult<DocumentDto>> CheckDocument([FromForm] IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }

            /* Leer documento y alamacenar*/
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var content = memoryStream.ToArray();
            var extension = Path.GetExtension(file.FileName);
            var documentPath = await
                _storeFiles.SaveFile(content, extension, _documentContainerCheck, file.ContentType);

            /*Retornar hash de documento*/
            var hashDocument = CalculateMd5(documentPath);

            /*  */
            var document = await _context.Documents
                .Include(x => x.DocumentDetail)
                .FirstOrDefaultAsync(x => x.DocumentDetail.HashSecret == hashDocument);

            await _storeFiles.DeleteFile(documentPath, _documentContainerCheck);


            if (document == null)
            {
                return new DocumentDto();
            }

            var documentDto = new DocumentDto
            {
                Id = document.Id,
                Url = document.Url,
                Affair = document.DocumentDetail.Affair,
                Hash = document.DocumentDetail.HashSecret,
                Title = document.DocumentDetail.Title,
                User = document.DocumentDetail.User
            };

            return documentDto;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentDto>>> ListDocuments()
        {
            var documents = await _context.Documents.Where(x => x.AppUserId == User.GetId())
                .Include(x => x.DocumentDetail)
                .ToListAsync();

            var documentsDto = new List<DocumentDto>();

            foreach (var document in documents)
            {
                var documentDto = new DocumentDto
                {
                    Id = document.Id,
                    Affair = document.DocumentDetail.Affair,
                    Title = document.DocumentDetail.Title,
                    User = document.DocumentDetail.User,
                    Hash = document.DocumentDetail.HashSecret,
                    Url = document.Url,
                };

                documentsDto.Add(documentDto);
            }

            return documentsDto;
        }


        private string CalculateMd5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = System.IO.File.OpenRead(filename);
            
            
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /*private string ConvertString(Object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }*/
    }
}