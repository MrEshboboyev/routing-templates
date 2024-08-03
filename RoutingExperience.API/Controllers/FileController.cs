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
            // checking file is null and length size > 0
            if (model.File == null && model.File.Length == 0)
            {
                return BadRequest("Invalid file! Try again...");
            }

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

        [HttpPost("multipleUpload"), DisableRequestSizeLimit]
        public async Task<IActionResult> MultipleUploadFiles([FromForm] MultipleFilesUploadModel model)
        {
            // response variable for display information uploaded files status
            var response = new Dictionary<string, string>();

            // checking Files in null or Files.Count is equal 0, returning BadRequest()
            if (model.Files == null || model.Files.Count == 0)
            {
                return BadRequest("Invalid files! Try again...");
            }

            // folder for multiple uploaded files
            var folderName = Path.Combine("Resources", "UploadedFiles");

            // saved path in physical disk drive
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // checking this path exists
            if (!Directory.Exists(pathToSave))
            {
                // is not exist, create this folder
                Directory.CreateDirectory(pathToSave);
            }

            // each files handle
            foreach (var file in model.Files)
            {
                // fileName variable for create file with name
                var fileName = file.FileName;

                // fullPath : for uploaded file exist in Physical
                var fullPath = Path.Combine(pathToSave, fileName);

                // database path for this file location in application database 
                var dbPath = Path.Combine(folderName, fileName);

                // checking exists file with this name
                if (!System.IO.File.Exists(fullPath))
                {
                    // if this file with fileName is not exist

                    // file upload to db using MemoryStream
                    using (var memoryStream = new MemoryStream())
                    {
                        // copied to target stream this file information
                        await file.CopyToAsync(memoryStream);

                        // write all data this memoryStream in fullPath 
                        await System.IO.File.WriteAllBytesAsync(fullPath, memoryStream.ToArray());

                        // adding dbPath to this file name in response for display 
                        response.Add(fileName, dbPath);
                    }
                }
                else
                {
                    // if this file with fileName is exist

                    // adding exist file status 'already exist' to response
                    response.Add(fileName, " : Already exist in Database");
                }
            }

            // returning ready response
            return Ok(new { response });
        }

        // download exist file
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string downloadFileName)
        {
            // folderName : database folder in application
            var folderName = Path.Combine("Resources", "UploadedFiles");

            // pathToSave : physical path in server
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // fileName : searching file from database
            var fileName = downloadFileName;

            // fullPath : checking this path in physical database
            var fullPath = Path.Combine(pathToSave, fileName);

            // checking this file in database : it is not exist, return BadRequest
            if (!System.IO.File.Exists(fullPath))
            {
                return BadRequest("File is not found in Database!");
            }

            // if exist it 

            // ready download file 

            // fileBytes : all bytes of download file 
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            // fileContentResult : returning result for user
            var fileContentResult = new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = fileName,
            };

            // return this file content result
            return fileContentResult;
        }
    }
}
