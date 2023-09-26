using BUS.Interface;
using BUS.Untility;
using Microsoft.AspNetCore.Http;
using Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;

namespace BUS.Classes
{
    public class Excel
    {
        private IFormFile _file;
        private ICourseBusiness _courseBusiness;
        private IClassBusiness _classBusiness;
        private ITranscriptBusiness _transcriptBusiness;

        private Dictionary<int, string> col_courseId = new Dictionary<int, string>();

        private string _classId;
        private int _semester;
        private string _schoolYear;
        private List<FullConduct> _conducts;

        public Excel()
        { }

        public Excel(IFormFile file)
        {
            _file = file;
        }

        public async Task<bool> ReadSheet(ExcelWorksheet sheet)
        {
            if (!await AddCourses(sheet)) return false;
            if (!await AddStudents(sheet)) return false;
            if (!await AddTranscript(sheet)) return false;

            return true;
        }

        private async Task<bool> AddCourses(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int rowCount = sheet.Dimension.Rows;
            int columnCount = sheet.Dimension.Columns;

            bool flag = false;

            for (int row = 2; row <= rowCount; row++)
            {
                if (cells[row, 1].Value.ToString() == "MÔN HỌC")
                {
                    for (int col = 2; col <= columnCount; col++)
                    {
                        object currValue = cells[row, col].Value;
                        if (currValue != null)
                        {
                            Course course = await Tools.GetCourseInfo(currValue.ToString());
                            course.NumOfCredits = Convert.ToInt16(cells[row + 1, col].Value);

                            for (int c = col; c <= columnCount; c++)
                            {
                                if (cells[row + 2, c].Value.ToString() == "H10")
                                {
                                    col_courseId.Add(c, course.Id);
                                    break;
                                }
                            }

                            if (await _courseBusiness.Create(course))
                            {
                                flag = true;
                            }
                        }
                    }
                    break;
                }
            }

            return flag;
        }

        private async Task<bool> AddStudents(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int rowCount = sheet.Dimension.Rows;
            int r = 2;
            bool flag = false;

            for (int row = 2; row <= rowCount; row++)
            {
                if (cells[row, 1].Value.ToString() == "SỐ TÍN CHỈ")
                {
                    r = row + 2;
                    break;
                }
            }

            List<Student> students = new List<Student>();

            for (int row = r; row <= rowCount; row++)
            {
                students.Add(new Student()
                {
                    Id = cells[row, 1].Value.ToString(),
                    Name = await Tools.ToTitleCase(cells[row, 2].Value.ToString()),
                    Birthday = DateTime.Parse(cells[row, 4].Value.ToString())
                });

                flag = true;
            }

            if (flag)
            {
                await _classBusiness.CreateClassStudents(new ClassStudent() { ClassId = _classId, Semester = _semester, SchoolYear = _schoolYear, list_json_student = students });
            }

            return flag;
        }

        private async Task<bool> AddTranscript(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int rowCount = sheet.Dimension.Rows;
            int r = 2;
            bool flag = false;

            for (int row = 2; row <= rowCount; row++)
            {
                if (cells[row, 1].Value.ToString() == "SỐ TÍN CHỈ")
                {
                    r = row + 2;
                    break;
                }
            }

            for (int row = r; row <= rowCount; row++)
            {
                foreach (KeyValuePair<int, string> entry in col_courseId)
                {
                    var point = cells[row, entry.Key].Value;
                    var grade = cells[row, entry.Key + 1].Value;
                    float result;

                    if (point != null && grade != null)
                    {
                        try
                        {
                            if (float.TryParse(point.ToString().Replace(",", "."), out result))
                            {
                                _transcriptBusiness.Create(new Transcript()
                                {
                                    StudentId = cells[row, 1].Value.ToString(),
                                    CourseId = entry.Value,
                                    Point = (float)Math.Round(result, 2),
                                    Grade = grade.ToString(),
                                    Semester = _semester,
                                    SchoolYear = _schoolYear
                                });
                            }
                            else
                            {
                                Debug.WriteLine($"Invalid input {point.ToString().Replace(",", ".")}");
                            }
                        }
                        catch { }
                    }
                    flag = true;
                }
            }

            return flag;
        }

        public async Task<bool> LoadToDB(ICourseBusiness courseBusiness, IClassBusiness classBusiness, ITranscriptBusiness transcriptBusiness, string classId, int Semester, string SchoolYear)
        {
            _courseBusiness = courseBusiness;
            _classBusiness = classBusiness;
            _transcriptBusiness = transcriptBusiness;

            _classId = classId;
            _semester = Semester;
            _schoolYear = SchoolYear;

            if (_file != null)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await _file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            ExcelWorkbook workbook = package.Workbook;
                            ExcelWorksheet sheet = workbook.Worksheets[0];

                            string? firstCellValue = sheet.Cells[1, 1].Value.ToString();

                            if (firstCellValue != null && firstCellValue.StartsWith("KẾT QUẢ HỌC TẬP KỲ"))
                            {
                                if (workbook.Worksheets.Count == 1)
                                {
                                    return await ReadSheet(sheet);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("File excel did not exitist");
            }

            return false;
        }

        private async void WriteSynthesiseSheet(ExcelWorksheet sheet, string classId, int semester, string schoolYear)
        {
            ExcelRange cells = sheet.Cells;

            cells[5, 2].Value = classId;
            cells[5, 7].Value = semester;
            cells[5, 10].Value = schoolYear;

            int row = 7;
            int conductsCount = _conducts.Count;
            sheet.InsertRow(row, conductsCount);

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.StudentBirthday?.ToString("dd/MM/yyyy");
                cells[row, 5].Value = conduct.I;
                cells[row, 6].Value = conduct.II;
                cells[row, 7].Value = conduct.III;
                cells[row, 8].Value = conduct.IV;
                cells[row, 9].Value = conduct.V;
                cells[row, 10].Value = conduct.Total;
                cells[row, 11].Value = conduct.Classification;

                var rangeStyle = cells[row, 1, row, 11].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 11; c++)
                {
                    var cellStyle = cells[row, c].Style;
                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);

                    if (c >= 5 && c <= 10)
                    {
                        cellStyle.Numberformat.Format = "0";
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                row++;
            }

            row = row + 1;

            int excellent = await Tools.ClassificationCount(_conducts, "Xuất sắc");
            int good = await Tools.ClassificationCount(_conducts, "Tốt");
            int decent = await Tools.ClassificationCount(_conducts, "Khá");
            int average = await Tools.ClassificationCount(_conducts, "TB");
            int belowAverage = await Tools.ClassificationCount(_conducts, "Yếu");
            int poor = await Tools.ClassificationCount(_conducts, "Kém");

            cells[row, 3].Value = $"= {excellent} / {conductsCount}";
            cells[row, 4].Value = Math.Round(((double)excellent / conductsCount) * 100, 2) + "%";

            cells[row + 1, 3].Value = $"= {good} / {conductsCount}";
            cells[row + 1, 4].Value = Math.Round(((double)good / conductsCount) * 100, 2) + "%";

            cells[row + 2, 3].Value = $"= {decent} / {conductsCount}";
            cells[row + 2, 4].Value = Math.Round(((double)decent / conductsCount) * 100, 2) + "%";

            cells[row, 9].Value = $"= {average} / {conductsCount}";
            cells[row, 10].Value = Math.Round(((double)average / conductsCount) * 100, 2) + "%";

            cells[row + 1, 9].Value = $"= {belowAverage} / {conductsCount}";
            cells[row + 1, 10].Value = Math.Round(((double)belowAverage / conductsCount) * 100, 2) + "%";

            cells[row + 2, 9].Value = $"= {poor} / {conductsCount}";
            cells[row + 2, 10].Value = Math.Round(((double)poor / conductsCount) * 100, 2) + "%";

            for (int r = 0; r <= 2; r++)
            {
                cells[row + r, 4].Style.Numberformat.Format = "0.00%";
                cells[row + r, 10].Style.Numberformat.Format = "0.00%";
            }

            row = row + 3 + 6;

            ClassInfo ci = await _classBusiness.GetClassInfo(classId);
            if (ci != null)
            {
                cells[row, 2].Value = await Tools.ToTitleCase(ci.AssistantDean);
                cells[row, 4].Value = await Tools.ToTitleCase(ci.FormTeacher);
                cells[row, 6].Value = await Tools.ToTitleCase(ci.Monitor);
            }
        }

        private async void WriteSheet1(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            cells[2, 6].Value = _conducts[0].TotalNumOfCredits;

            int row = 3;

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.PointAverage;
                cells[row, 5].Value = conduct.TotalNumOfFailCredits;
                cells[row, 6].Value = conduct.PersentOfFailCredits + "%";
                cells[row, 7].Value = conduct.I_1;
                cells[row, 8].Value = conduct.I_2;
                cells[row, 9].Value = conduct.I_3;
                cells[row, 10].Value = conduct.I_4;
                cells[row, 11].Value = conduct.I_5;
                cells[row, 12].Value = conduct.I;

                var rangeStyle = cells[row, 1, row, 12].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 12; c++)
                {
                    var cellStyle = cells[row, c].Style;

                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);

                    switch (c)
                    {
                        case 1:
                            cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            break;

                        case 4:
                            cellStyle.Numberformat.Format = "0.00";
                            break;

                        case 6:
                            cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            cellStyle.Numberformat.Format = "0%";
                            break;

                        default:
                            cellStyle.Numberformat.Format = "0";
                            break;
                    }
                }

                row++;
            }
        }

        private async void WriteSheet2(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int row = 3;

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.II_1;
                cells[row, 5].Value = conduct.II_2;
                cells[row, 6].Value = conduct.II_3;
                cells[row, 7].Value = conduct.II_4;
                cells[row, 8].Value = conduct.II_5;
                cells[row, 9].Value = conduct.II_6;
                cells[row, 10].Value = conduct.II;

                var rangeStyle = cells[row, 1, row, 10].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 10; c++)
                {
                    var cellStyle = cells[row, c].Style;

                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);

                    if (c >= 4)
                    {
                        cellStyle.Numberformat.Format = "0";
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    if (c == 1 || c == 2)
                    {
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                row++;
            }
        }

        private async void WriteSheet3(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int row = 3;

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.III_1;
                cells[row, 5].Value = conduct.III_2;
                cells[row, 6].Value = conduct.III_3;
                cells[row, 7].Value = conduct.III;

                var rangeStyle = cells[row, 1, row, 7].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 7; c++)
                {
                    var cellStyle = cells[row, c].Style;

                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);

                    if (c >= 4)
                    {
                        cellStyle.Numberformat.Format = "0";
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    if (c == 1 || c == 2)
                    {
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                row++;
            }
        }

        private async void WriteSheet4(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int row = 3;

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.PointAverage;
                cells[row, 5].Value = conduct.IV_1;
                cells[row, 6].Value = conduct.IV_2;
                cells[row, 7].Value = conduct.IV_3;
                cells[row, 8].Value = conduct.IV;

                var rangeStyle = cells[row, 1, row, 8].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 8; c++)
                {
                    var cellStyle = cells[row, c].Style;

                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (c == 1 || c == 2)
                    {
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    if (c == 4)
                    {
                        cellStyle.Numberformat.Format = "0.00";
                    }

                    if (c > 4)
                    {
                        cellStyle.Numberformat.Format = "0";
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                row++;
            }
        }

        private async void WriteSheet5(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int row = 3;

            foreach (FullConduct conduct in _conducts)
            {
                cells[row, 1].Value = conduct.OrdinalNumber;
                cells[row, 2].Value = conduct.StudentId;
                cells[row, 3].Value = conduct.StudentName;
                cells[row, 4].Value = conduct.PointAverage;
                cells[row, 5].Value = conduct.V_1;
                cells[row, 6].Value = conduct.V_2;
                cells[row, 7].Value = conduct.V;

                var rangeStyle = cells[row, 1, row, 7].Style;
                rangeStyle.Font.Size = 11;

                for (int c = 1; c <= 7; c++)
                {
                    var cellStyle = cells[row, c].Style;

                    cellStyle.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (c == 1 || c == 2)
                    {
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    if (c == 4)
                    {
                        cellStyle.Numberformat.Format = "0.00";
                    }

                    if (c > 4)
                    {
                        cellStyle.Numberformat.Format = "0";
                        cellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                row++;
            }
        }

        public async Task<byte[]> ExportFullConductsToExcel(IClassBusiness classBusiness, List<FullConduct> conducts, string classId, int semester, string schoolYear)
        {
            _classBusiness = classBusiness;

            FileInfo file = new FileInfo("../BUS/Template/ConductTemplate.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorkbook workBook = package.Workbook;
                ExcelWorksheets sheets = workBook.Worksheets;

                _conducts = conducts;

                WriteSynthesiseSheet(sheets[0], classId, semester, schoolYear);
                WriteSheet1(sheets[1]);
                WriteSheet2(sheets[2]);
                WriteSheet3(sheets[3]);
                WriteSheet4(sheets[4]);
                WriteSheet5(sheets[5]);

                return await package.GetAsByteArrayAsync();
            }
        }
    }
}