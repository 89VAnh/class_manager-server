using Models;

namespace BUS.Untility
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

        public static async Task<int> ClassificationCount(List<FullConduct> conducts, string str)
        {
            return conducts.Where(x => x.Classification == str).Count();
        }

        public static async Task<string> ToTitleCase(string str)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}