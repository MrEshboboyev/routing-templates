using Microsoft.AspNetCore.Mvc;
using RoutingExperience.API.Models;

namespace RoutingExperience.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly string ResourcesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
    private readonly string UploadedFilesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "UploadedFiles");
    private readonly string _pathToSave;
    private readonly string _folderName;

    public FileController()
    {
        _folderName = Path.Combine(ResourcesFolder, UploadedFilesFolder);
        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), _folderName);

        if (!System.IO.File.Exists(pathToSave))
        {
            Directory.CreateDirectory(pathToSave);
        }

        _pathToSave = pathToSave;
    }

    [HttpPost("upload"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
    {
        await Task.Delay(1); // simulate async work

        // checking file is null and length size > 0
        if (model.File == null && model.File!.Length == 0)
            return BadRequest("Invalid file! Try again...");

        // create fileName and full path of uploaded file
        var fileName = model.File.FileName;

        var fullPath = Path.Combine(_pathToSave, fileName);

        var dbPath = Path.Combine(_folderName, fileName);

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

        // each files handle
        foreach (var file in model.Files)
        {
            // fileName variable for create file with name
            var fileName = file.FileName;

            // fullPath : for uploaded file exist in Physical
            var fullPath = Path.Combine(_pathToSave, fileName);

            // database path for this file location in application database 
            var dbPath = Path.Combine(_folderName, fileName);

            // checking exists file with this name
            if (!System.IO.File.Exists(fullPath))
            {
                // if this file with fileName is not exist

                // file upload to db using MemoryStream
                using var memoryStream = new MemoryStream();
                // copied to target stream this file information
                await file.CopyToAsync(memoryStream);

                // write all data this memoryStream in fullPath 
                await System.IO.File.WriteAllBytesAsync(fullPath, memoryStream.ToArray());

                // adding dbPath to this file name in response for display 
                response.Add(fileName, dbPath);
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
        // fileName : searching file from database
        var fileName = downloadFileName;

        // fullPath : checking this path in physical database
        var fullPath = Path.Combine(_pathToSave, fileName);

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
