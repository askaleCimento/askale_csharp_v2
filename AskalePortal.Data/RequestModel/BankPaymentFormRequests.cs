namespace AskalePortal.Data.RequestModel
{
    public sealed class AccountPaymentFilterFormRequest
    {
        public string? name1 { get; set; }
        public int? userId { get; set; }
    }

    public sealed class TransferPaymentFilterFormRequest
    {
        public string? firma { get; set; }
        public int? userId { get; set; }
    }

    public abstract class BankPaymentPageFormRequest
    {
        public int? page { get; set; }
        public int? size { get; set; }
        public string? sort { get; set; }
        public string? sortingKey { get; set; }
        public string? sortingValue { get; set; }
        public string? sortingDirection { get; set; }
        public int? userId { get; set; }
        public bool? refresh { get; set; }
    }

    public sealed class AccountPaymentPageFormRequest : BankPaymentPageFormRequest
    {
        public string? name1 { get; set; }
    }

    public sealed class TransferPaymentPageFormRequest : BankPaymentPageFormRequest
    {
        public string? firma { get; set; }
    }
}
