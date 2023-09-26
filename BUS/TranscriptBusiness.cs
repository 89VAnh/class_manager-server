using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public class TranscriptBusiness : ITranscriptBusiness
    {
        private ITranscriptRepository _res;

        public TranscriptBusiness(ITranscriptRepository res)
        {
            _res = res;
        }

        public async Task<bool> Create(Transcript transcript)
        {
            return await _res.Create(transcript);
        }
    }
}