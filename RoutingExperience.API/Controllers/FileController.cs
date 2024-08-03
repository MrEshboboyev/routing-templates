using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutingExperience.API.Models;

namespace RoutingExperience.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            // creating Resources/UploadedFiles folder
            var folderName = Path.Combine("Resources", "UploadedFiles");

            // path to save uploaded file
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (!System.IO.File.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            // create fileName and full path of uploaded file
            var fileName = model.File.FileName;

            var fullPath = Path.Combine(pathToSave, fileName);

            var dbPath = Path.Combine(folderName, fileName);

            // already exists full path
            if (System.IO.File.Exists(fullPath))
            {
                return BadRequest("File already exists in database!");
            }

            // copy uploaded file to fullPath
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                model.File.CopyTo(stream);
            }

            return Ok(new { dbPath });
        }
    }
}
