using DAL.Helper;
using DAL.Interface;
using Models;

namespace DAL
{
    public class TranscriptRepository : ITranscriptRepository
    {
        private IDatabaseHelper _db;

        public TranscriptRepository(IDatabaseHelper db)
        {
            _db = db;
        }

        public async Task<bool> Create(Transcript transcript)
        {
            string msgError = "";

            try
            {
                var result = _db.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_transcript_create",
                "@StudentId", transcript.StudentId, "@CourseId", transcript.CourseId, "@Point", transcript.Point, "@Grade", transcript.Grade, "@Semester", transcript.Semester, "@SchoolYear", transcript.SchoolYear
                );
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}