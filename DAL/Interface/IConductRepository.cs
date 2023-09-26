using Models;

namespace DAL.Interface
{
    public interface IConductRepository
    {
        public Task<List<Conduct>> GetConductsOfClass(string classId, int semester, string schoolYear);

        public Task<bool> Update(MinConduct conduct);
    }
}