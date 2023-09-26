using Models;

namespace BUS.Interface
{
    public interface ITranscriptBusiness
    {
        public Task<bool> Create(Transcript transcript);
    }
}