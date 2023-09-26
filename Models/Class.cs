namespace Models
{
    public class ClassInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FormTeacher { get; set; }
        public string Department { get; set; }
        public string Monitor { get; set; }
        public string AssistantDean { get; set; }
    }

    public class ClassStudent
    {
        public string ClassId { get; set; }
        public int Semester { get; set; }
        public string SchoolYear { get; set; }
        public List<Student> list_json_student { get; set; }
    }
}