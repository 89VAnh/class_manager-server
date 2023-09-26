using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ConductController : ControllerBase
    {
        private IConductBusiness _conductBusiness;
        private IMonitorBusiness _monitorBusiness;
        private IClassBusiness _classBusiness;

        public ConductController(IConductBusiness conductBusiness, IMonitorBusiness monitorBusiness, IClassBusiness classBusiness)
        {
            _conductBusiness = conductBusiness;
            _monitorBusiness = monitorBusiness;
            _classBusiness = classBusiness;
        }

        [HttpGet]
        [Route("get-conduct-of-class")]
        public async Task<List<Conduct>> GetClassConducts(string classId, int semester, string schoolYear)
        {
            return await _conductBusiness.GetConductsOfClass(classId, semester, schoolYear);
        }

        [HttpPut]
        [Route("update-multiple-conduct")]
        public async Task<IActionResult> UpdateConducts(List<MinConduct> conducts)
        {
            try
            {
                foreach (var conduct in conducts)
                {
                    if (!await _conductBusiness.Update(conduct))
                    {
                        throw new Exception();
                    };
                }
                return Ok("Update success!!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-excel-file-of-class")]
        public async Task<IActionResult> GetExcelFileOfClass(string classId, int semester, string schoolYear, string monitorId)
        {
            _monitorBusiness.Create(new MonitorModel() { ClassId = classId, Semester = semester, SchoolYear = schoolYear, MonitorId = monitorId });

            var stream = new MemoryStream(await _conductBusiness.ExportToExcel(classId, semester, schoolYear, monitorId));

            string filename = $"Tong hop KQRL_HK{semester}_{schoolYear}_{classId}";

            Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}.xlsx");
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}