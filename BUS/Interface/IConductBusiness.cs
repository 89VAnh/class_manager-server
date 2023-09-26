using Models;

namespace BUS.Interface
{
    public interface IConductBusiness
    {
        Task<List<Conduct>> GetConductsOfClass(string classId, int semester, string schoolYear);

        Task<List<FullConduct>> GetFullConductsOfClass(string classId, int semester, string schoolYear);

        Task<FullConduct> ConductToFullConduct(Conduct conduct);

        Task<bool> Update(MinConduct conduct);

        Task<byte[]> ExportToExcel(string classId, int semester, string schoolYear, string monitorId);
    }
}