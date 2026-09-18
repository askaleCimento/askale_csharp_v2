using SapNwRfc;

namespace AskalePortal.Data.SAP.OutputParams
{
    public class AvgVadeDaysSapOutput
    {
        [SapName("E_AVG_DAYS")]
        public double avgDays { get; set; }

        [SapName("E_AVG_VADE")]
        public double avgVade { get; set; }
    }
}
