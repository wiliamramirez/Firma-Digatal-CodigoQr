using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


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

        [HttpPost()]
        public async Task<ActionResult<DocumentDto>> Post([FromForm] AddDocumentDto addDocumentDto)
        {
            
            var user = await _context.Users.FindAsync(User.GetId());

            if (addDocumentDto.File == null)
            {
                return BadRequest();
            }

            /* Leer documento y alamcenar*/
            await using var memoryStream = new MemoryStream();
            addDocumentDto.File.CopyTo(memoryStream);
            var content = memoryStream.ToArray();
            var extension = Path.GetExtension(addDocumentDto.File.FileName);
            var documentPath = await
                _storeFiles.SaveFile(content, extension, _documentContainer, addDocumentDto.File.ContentType);

            /*  */
            var urlFile = _storeFiles.GetUrl(_documentContainer, Path.GetFileName(documentPath));

            /*url de la ruta del archivo*/
            var finalDocumentPath = urlFile.Replace(".pdf", $"{_secretKey}.pdf");

            /* Hash del documento sin codigo qr */
            var hashDocument = CalculateMd5(documentPath);


            /*Valores que se imprimiran el codigo qr*/
            var printQrCode = new
            {
                Url = finalDocumentPath,
                User = User.GetSurname()
            };

            var contentQrCode = await _qrCode.GenerateQrCode(ConvertString(printQrCode));

            /*Ruta del codigo qr*/
            var qrCodeImagePath = await _storeFiles.SaveFile(contentQrCode, ".png", _codeQrContainer);

            /* agregar el codigo qr al documento */
            await _qrCode.AddQrCodeFile(
                qrCodeImagePath,
                _codeQrContainer,
                documentPath,
                _documentContainer,
                _secretKey);

            /*Generando hash del documento mas el codigo qr*/
            var hashSecret = CalculateMd5(documentPath);

            var document = new Document
            {
                /*Id = hashDocument,*/
                Id = Guid.NewGuid().ToString(),
                Affair = addDocumentDto.Affair,
                Url = finalDocumentPath,
                Title = addDocumentDto.Title,
                User = User.GetSurname(),
                HashSecret = hashSecret,
                AppUser = user
            };

            _context.Documents.Add(document);
            var resultContext = await _context.SaveChangesAsync();

            /*Completando el mapeo*/
            var userDto = _mapper.Map<UserDto>(user);
            var documentDto = new DocumentDto
            {
                Id = document.Id,
                Affair = addDocumentDto.Affair,
                Title = addDocumentDto.Title,
                Url = finalDocumentPath,
                User = User.GetSurname(),
                Hash = hashSecret,
                UserDto = userDto,
            };


            if (resultContext > 0)
            {
                await _storeFiles.DeleteFile(urlFile, _documentContainer);
                return Ok(documentDto);
            }

            return BadRequest();
        }

        [HttpGet()]
        public async Task<ActionResult<List<DocumentDto>>> ListDocuments()
        {
            return Ok();
        }

        private string CalculateMd5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = System.IO.File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private string ConvertString(Object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}