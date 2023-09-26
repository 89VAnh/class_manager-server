using Models;

namespace BUS.Interface
{
    public interface IClassBusiness
    {
        Task<ClassInfo> GetClassInfo(string id);

        Task<bool> CreateClassStudents(ClassStudent clt);
    }
}