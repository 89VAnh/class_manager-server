using Models;

namespace DAL.Interface
{
    public interface IClassRepository
    {
        Task<ClassInfo> GetClassInfo(string id);

        Task<bool> CreateClassStudents(ClassStudent clt);
    }
}