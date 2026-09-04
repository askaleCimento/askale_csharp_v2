#nullable enable

namespace AskalePortal.Data.Contracts.Detached;

public sealed class ISGAksiyonTableDto
{
   public int id{get;set;}
   public string? fileOnceki{get;set;}
   public string? fileSonraki{get;set;}
   public string? vtext{get;set;}
   public string? uygunsuzlukKaynagi{get;set;}
   public DateTime? uygunsuzlukTarihi{get;set;}
   public string? section{get;set;}
   public string? name{get;set;}
   public string? uygunsuzlukAciklama{get;set;}
   public string? uygunsuzlukOneri{get;set;}
   public bool? bittiMi{get;set;}

}
