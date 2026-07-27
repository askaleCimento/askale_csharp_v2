using SapNwRfc;


namespace AskalePortal.Data.SAP.OutputParams
{
    public class ActiveProcessChecksOutput
    {
        [SapName("ET_LIST")]
        public ActiveProcessCheckList[]? activeProcessCheckList { get; set; }

    }

    public class ActiveProcessCheckList
    {
        [SapName("BELNR")]
        public string? belnr { get; set; }

        [SapName("KUNNR")]
        public string? kunnr { get; set; }

        [SapName("NAME1")]
        public string? name1 { get; set; }

        [SapName("NETDT")]
        public string? netdt { get; set; }

        [SapName("WRBTR")]
        public string? wrbtr { get; set; }
    }
}
