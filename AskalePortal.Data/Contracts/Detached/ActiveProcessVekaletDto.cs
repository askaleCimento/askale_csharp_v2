#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ActiveProcessVekaletDto
{
    public int Id { get; set; }

    public int VekaletVerenId { get; set; }

    public int VekaletAlanId { get; set; }

    public DateTime Tarih { get; set; }

    public bool enabled { get; set; }

}
