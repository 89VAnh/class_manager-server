using BUS.Classes;
using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private ICourseBusiness _courseBusiness;
        private IClassBusiness _classBusiness;
        private ITranscriptBusiness _transcriptBusiness;

        public FilesController(ICourseBusiness courseBusiness, IClassBusiness classBusiness, ITranscriptBusiness transcriptBusiness)
        {
            _courseBusiness = courseBusiness;
            _classBusiness = classBusiness;
            _transcriptBusiness = transcriptBusiness;
        }

        [HttpPost]
        [Route("Transcript")]
        public async Task<IActionResult> UploadTranscriptFile(IFormFile file, string classId, int Semester, string SchoolYear)
        {
            string fileName = file.FileName;

            if (Path.GetExtension(fileName).ToLower() == ".xlsx")
            {
                Excel excel = new Excel(file);
                if (!await excel.LoadToDB(_courseBusiness, _classBusiness, _transcriptBusiness, classId, Semester, SchoolYear))
                {
                    return BadRequest($"Không đọc được file {fileName}!");
                }
            }
            else return BadRequest($"Vui lòng chọn file excel có định dạng .xlsx");

            return Ok($"Tải lên file '{fileName}' thành công!");
        }
    }
}