namespace AskalePortal.Data.RequestModel
{
    public class InternalCorrespondenceListParameterDto
    {
        public int? id { get; set; }
        public int? companyId { get; set; }
        public string? servisi { get; set; }
        public string? konu { get; set; }
        public string? aciklama { get; set; }
        public bool? bittimi { get; set; }
        public bool? redEttiMi { get; set; }
        public int userId { get; set; }
    }
}
