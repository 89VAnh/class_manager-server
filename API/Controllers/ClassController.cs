using BUS.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private IClassBusiness _classBusiness;

        public ClassController(IClassBusiness classBusiness)
        {
            _classBusiness = classBusiness;
        }

        [Route("get-class-info")]
        [HttpGet]
        public async Task<ClassInfo> GetClassInfo(string classId)
        {
            return await _classBusiness.GetClassInfo(classId);
        }
    }
}