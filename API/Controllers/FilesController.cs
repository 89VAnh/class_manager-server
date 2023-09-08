using BUS.Interface;
using Microsoft.AspNetCore.Mvc;
using server.Classes;
using server.Untility;

namespace server.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private ICourseBusiness _courseBusiness;

        public FilesController(ICourseBusiness courseBusiness)
        {
            _courseBusiness = courseBusiness;
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string fileName = file.FileName;
            if (await Tools.CheckTranscriptFileName(fileName))
            {
                Excel excel = new Excel(file, _courseBusiness);
                if (!await excel.LoadToDB())
                {
                    return BadRequest($"Không đọc được file {fileName}");
                }
            }
            else return BadRequest($"Vui lòng chọn file có tên đúng định dạng <tên lớp> <học kỳ>.Xlsx (VD: 125211 HK2.Xlsx)");

            return Ok($"Tải lên file '{fileName}' thành công!");
        }
    }
}