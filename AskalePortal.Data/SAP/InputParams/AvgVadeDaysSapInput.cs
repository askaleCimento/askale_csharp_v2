using SapNwRfc;

namespace AskalePortal.Data.SAP.InputParams
{
    public class AvgVadeDaysSapInput
    {
        [SapName("I_KUNNR")]
        public string? kunnr { get; set; }
    }
}
