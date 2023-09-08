using server.Models;
using System.Text.RegularExpressions;

namespace server.Untility
{
    public class Tools
    {
        public static async Task<Course> GetCourseInfo(string? str)
        {
            if (str != null)
            {
                string[] arr = str.Split('-');

                return new Course() { Id = arr[0].Trim(), Name = arr[1].Trim() };
            }
            return new Course();
        }

        public static async Task<bool> CheckTranscriptFileName(string fileName)
        {
            Regex fileNameRegex = new Regex(@"^.+ HK[12].Xlsx$");
            return fileNameRegex.IsMatch(fileName);
        }
    }
}