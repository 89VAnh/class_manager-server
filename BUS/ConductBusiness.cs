using BUS.Classes;
using BUS.Interface;
using DAL.Interface;
using Models;

namespace BUS
{
    public class ConductBusiness : IConductBusiness
    {
        private IConductRepository _res;
        private IMonitorRepository _monitorRes;
        private IClassBusiness _classBusiness;

        public ConductBusiness(IConductRepository res, IMonitorRepository monitorRes, IClassBusiness classBusiness)
        {
            _res = res;
            _monitorRes = monitorRes;
            _classBusiness = classBusiness;
        }

        public Task<List<Conduct>> GetConductsOfClass(string classId, int semester, string schoolYear)
        {
            return _res.GetConductsOfClass(classId, semester, schoolYear);
        }

        public async Task<List<FullConduct>> GetFullConductsOfClass(string classId, int semester, string schoolYear)
        {
            List<Conduct> conducts = await GetConductsOfClass(classId, semester, schoolYear);
            List<FullConduct> fullConducts = new List<FullConduct>();

            foreach (Conduct conduct in conducts)
            {
                fullConducts.Add(await ConductToFullConduct(conduct));
            }

            return fullConducts;
        }

        public async Task<FullConduct> ConductToFullConduct(Conduct conduct)
        {
            FullConduct fullConduct = new FullConduct()
            {
                OrdinalNumber = conduct.OrdinalNumber,
                StudentId = conduct.StudentId,
                StudentName = conduct.StudentName,
                StudentBirthday = conduct.StudentBirthday,
                PointAverage = conduct.PointAverage,
                TotalNumOfCredits = conduct.TotalNumOfCredits,
                TotalNumOfFailCredits = conduct.TotalNumOfFailCredits,
                Id = conduct.Id,
                I_1 = conduct.I_1,
                I_2 = conduct.I_2,
                I_3 = conduct.I_3,
                II_1 = conduct.II_1,
                II_2 = conduct.II_2,
                II_3 = conduct.II_3,
                II_4 = conduct.II_4,
                II_5 = conduct.II_5,
                II_6 = conduct.II_6,
                III_1 = conduct.III_1,
                III_2 = conduct.III_2,
                III_3 = conduct.III_3,
                IV_1 = conduct.IV_1,
                IV_2 = conduct.IV_2,
                IV_3 = conduct.IV_3,
                V_1 = conduct.V_1,
                V_2 = conduct.V_2
            };

            // persent of fail credits
            int persentOfFailCredits = (int)Math.Round((double)((double)conduct.TotalNumOfFailCredits / conduct.TotalNumOfCredits) * 100);

            fullConduct.PersentOfFailCredits = persentOfFailCredits;

            // I.4
            if (persentOfFailCredits > 30)
                fullConduct.I_4 = 0;
            else if (persentOfFailCredits > 20)
                fullConduct.I_4 = 1;
            else if (persentOfFailCredits >= 10)
                fullConduct.I_4 = 2;
            else if (persentOfFailCredits > 0)
                fullConduct.I_4 = 3;
            else if (persentOfFailCredits == 0)
                fullConduct.I_4 = 4;
            else
                fullConduct.I_4 = 0;

            // I.5
            float? pointAverage = fullConduct.PointAverage;

            if (pointAverage < 5)
                fullConduct.I_5 = 0;
            else if (pointAverage < 6)
                fullConduct.I_5 = 2;
            else if (pointAverage < 7)
                fullConduct.I_5 = 3;
            else if (pointAverage < 8)
                fullConduct.I_5 = 4;
            else if (pointAverage < 9)
                fullConduct.I_5 = 5;
            else if (pointAverage <= 10)
                fullConduct.I_5 = 6;
            else fullConduct.I_5 = 0;

            // I
            fullConduct.I = fullConduct.I_1 + fullConduct.I_2 + fullConduct.I_3 + fullConduct.I_4 + fullConduct.I_5;

            // II
            fullConduct.II = fullConduct.II_1 + fullConduct.II_2 + fullConduct.II_3 + fullConduct.II_4 + fullConduct.II_5 + fullConduct.II_6;

            // III
            fullConduct.III = fullConduct.III_1 + fullConduct.III_2 + fullConduct.III_3;

            // IV
            fullConduct.IV = fullConduct.IV_1 + fullConduct.IV_2 + fullConduct.IV_3;

            if (fullConduct.II_3 == 0)
                fullConduct.IV -= 10;

            if (fullConduct.PointAverage >= 7 || fullConduct.V_1 == 10)
                fullConduct.IV = 20;

            //V_2
            if (fullConduct.PointAverage >= 9)
                fullConduct.V_2 = 10;

            // V
            if (fullConduct.V_1 == 10 || fullConduct.V_2 == 10)
                fullConduct.V = 10;
            else fullConduct.V = 0;

            // Total
            int? total = fullConduct.I + fullConduct.II + fullConduct.III + fullConduct.IV + fullConduct.V;
            fullConduct.Total = total;

            if (total <= 35)
                fullConduct.Classification = "Kém";
            else if (total <= 50)
                fullConduct.Classification = "Yếu";
            else if (total <= 65)
                fullConduct.Classification = "TB";
            else if (total <= 80)
                fullConduct.Classification = "Khá";
            else if (total <= 90)
                fullConduct.Classification = "Tốt";
            else
                fullConduct.Classification = "Xuất sắc";

            return fullConduct;
        }

        public async Task<bool> Update(MinConduct conduct)
        {
            return await _res.Update(conduct);
        }

        public async Task<byte[]> ExportToExcel(string classId, int semester, string schoolYear, string monitorId)
        {
            Excel excel = new Excel();
            List<FullConduct> conducts = await GetFullConductsOfClass(classId, semester, schoolYear);

            MonitorModel monitor = new MonitorModel() { ClassId = classId, Semester = semester, SchoolYear = schoolYear, MonitorId = monitorId };

            if (!await _monitorRes.Create(monitor))
            {
                throw new Exception($"Monitor is valid : {monitor}");
            };

            return await excel.ExportFullConductsToExcel(_classBusiness, conducts, classId, semester, schoolYear);
        }
    }
}