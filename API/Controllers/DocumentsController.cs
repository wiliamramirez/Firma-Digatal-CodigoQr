using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


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
        public ActionResult Post()
        {
            string container = "Test";
            string filePath = Path.GetFileName("/Test/imd.pdf");
            string imagePath = Path.GetFileName("/Test/codigo.png");
            string sourceFilePath = Path.Combine(_environment.WebRootPath, container, filePath);
            string watermarkImagePath = Path.Combine(_environment.WebRootPath, container, imagePath);
            string destinationFilePath = "./wwwroot/Test/Watermarked_Output.pdf";

            var result = PdfStampWithNewFile(watermarkImagePath, sourceFilePath, destinationFilePath);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        private bool PdfStampWithNewFile(string watermarkLocation, string fileLocation, string destinationFilePath)
        {
            Document document = new Document();
            PdfReader pdfReader = new PdfReader(fileLocation);
            PdfStamper stamp = new PdfStamper(pdfReader,
                new FileStream(fileLocation.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(watermarkLocation);
            img.SetAbsolutePosition(125,
                300); // set the position in the document where you want the watermark to appear (0,0 = bottom left corner of the page)


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