#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class KatilimcilarTableDto
{
    public int KatilimcilarID { get; set; }

    public string Sirket { get; set; }

    public string Katilimcilar { get; set; }

    public string Bolumu { get; set; }

    public string Email { get; set; }

    public bool status { get; set; }

}
