using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public class ClassBusiness : IClassBusiness
    {
        private IClassRepository _res;

        public ClassBusiness(IClassRepository res)
        {
            _res = res;
        }

        public Task<ClassInfo> GetClassInfo(string id)
        {
            return _res.GetClassInfo(id);
        }

        public Task<bool> CreateClassStudents(ClassStudent clt)
        {
            return _res.CreateClassStudents(clt);
        }
    }
}