using Models;

namespace DAL.Interface
{
    public interface ITranscriptRepository
    {
        public Task<bool> Create(Transcript transcript);
    }
}