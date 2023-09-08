using BUS.Interface;
using OfficeOpenXml;
using server.Models;
using server.Untility;

namespace server.Classes
{
    public class Excel
    {
        private IFormFile _file;
        private ICourseBusiness _courseBusiness;

        public Excel(IFormFile file, ICourseBusiness courseBusiness)
        {
            _file = file;
            _courseBusiness = courseBusiness;
        }

        public async Task<bool> LoadToDB()
        {
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

        public async Task<bool> ReadSheet(ExcelWorksheet sheet)
        {
            try
            {
                if (!await AddCourses(sheet)) return false;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> AddCourses(ExcelWorksheet sheet)
        {
            ExcelRange cells = sheet.Cells;

            int rowCount = sheet.Dimension.Rows;
            int columnCount = sheet.Dimension.Columns;

            for (int row = 2; row <= rowCount; row++)
            {
                if (cells[row, 1].Value.ToString() == "MÔN HỌC")
                {
                    for (int column = 2; column <= columnCount; column++)
                    {
                        object currValue = cells[row, column].Value;
                        if (currValue != null)
                        {
                            Course course = await Tools.GetCourseInfo(currValue.ToString());
                            course.NumOfCredits = Convert.ToInt16(cells[row + 1, column].Value);
                            return _courseBusiness.Create(course);
                        }
                    }
                    break;
                }
            }

            return false;
        }
    }
}