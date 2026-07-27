using SapNwRfc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class CustomerDocument
    {
        [Display(Name = "Şirket Kodu")]
        [SapName("BUKRS")]
        public string? BUKRS { get; set; }

        [Display(Name = "Hesap")]
        [SapName("KUNNR")]
        public string? KUNNR { get; set; }

        [Display(Name = "Müşteri Adı")]
        [SapName("ZZMUS_AD")]
        public string? ZZMUS_AD { get; set; }

        [Display(Name = "Belge tarihi")]
        [SapName("BLDAT")]
        public DateTime? BLDAT { get; set; }

        [Display(Name = "Kayıt tarihi")]
        [SapName("BUDAT")]
        public DateTime? BUDAT { get; set; }

        [Display(Name = "Ödeme dnm.temel trh.")]
        [SapName("ZFBDT")]
        public DateTime? ZFBDT { get; set; }

        [Display(Name = "Dağıtım Kanalı")]
        [SapName("DagitimKanali")]
        public string? DagitimKanali { get; set; }

        [Display(Name = "Net vade tarihi")]
        [SapName("FAEDT")]
        public DateTime? FAEDT { get; set; }

        [Display(Name = "Belge No")]
        [SapName("BELNR")]
        public string? BELNR { get; set; }

        [Display(Name = "Belge türü")]
        [SapName("BLART")]
        public string? BLART { get; set; }

        [Display(Name = "Kayıt anahtarı")]
        [SapName("BSCHL")]
        public string? BSCHL { get; set; }

        [Display(Name = "Net vade tarihi sembolü")]
        [SapName("ICO_DUE")]
        public string? ICO_DUE { get; set; }

        [Display(Name = "Özel DK göstergesi")]
        [SapName("UMSKZ")]
        public string? UMSKZ { get; set; }

        [Display(Name = "UP cinsinden tutar")]
        [SapName("DMSHB")]
        public decimal? DMSHB { get; set; }

        [Display(Name = "Ulusal para birimi")]
        [SapName("HWAER")]
        public string? HWAER { get; set; }

        [Display(Name = "Nkt.ind.taban tutarı")]
        [SapName("SKFBT")]
        public decimal? SKFBT { get; set; }

        [Display(Name = "Metin")]
        [SapName("SGTXT")]
        public string? SGTXT { get; set; }

        [Display(Name = "Ödeme biçimi")]
        [SapName("ZLSCH")]
        public string? ZLSCH { get; set; }

        [Display(Name = "Ödeme koşulu")]
        [SapName("ZTERM")]
        public string? ZTERM { get; set; }

        [Display(Name = "Gün 1")]
        [SapName("ZBD1T")]
        public decimal? ZBD1T { get; set; }

        [Display(Name = "Gün 2")]
        [SapName("ZBD2T")]
        public decimal? ZBD2T { get; set; }

        [Display(Name = "Faiz böleni")]
        [SapName("ZINSZ")]
        public decimal? ZINSZ { get; set; }

        [Display(Name = "Net vade tarihine göre gecikme")]
        [SapName("VERZN")]
        public decimal? VERZN { get; set; }

        [Display(Name = "Vergi göstergesi")]
        [SapName("MWSKZ")]
        public string? MWSKZ { get; set; }

        [Display(Name = "Referans")]
        [SapName("XBLNR")]
        public string? XBLNR { get; set; }

        [Display(Name = "Ana hesap")]
        [SapName("HKONT")]
        public string? HKONT { get; set; }

        [Display(Name = "Belge başlığı metni")]
        [SapName("U_BKTXT")]
        public string? U_BKTXT { get; set; }

        [Display(Name = "Mal ve Hizmet Cinsi")]
        [SapName("ZZMALHIZ")]
        public string? ZZMALHIZ { get; set; }

        [Display(Name = "Özel DK işlem sınıfı")]
        [SapName("UMSKS")]
        public string? UMSKS { get; set; }

        [Display(Name = "Dnklş.işl.geri al")]
        [SapName("XRAGL")]
        public string? XRAGL { get; set; }

        [Display(Name = "Fatura referansı")]
        [SapName("REBZG")]
        public string? REBZG { get; set; }

        [Display(Name = "Faturalama belgesi")]
        [SapName("VBELN")]
        public string? VBELN { get; set; }

        [Display(Name = "Kayıt dönemi")]
        [SapName("MONAT")]
        public int? MONAT { get; set; }

        [Display(Name = "Yıl/ay")]
        [SapName("JAMON")]
        public string? JAMON { get; set; }

        [Display(Name = "Mali yıl")]
        [SapName("GJAHR")]
        public int? GJAHR { get; set; }

        [Display(Name = "Tutar (UPB2)")]
        [SapName("DMBE2")]
        public decimal? DMBE2 { get; set; }

        [Display(Name = "UPB 2")]
        [SapName("HWAE2")]
        public string? HWAE2 { get; set; }

        [Display(Name = "Tutar (UPB3)")]
        [SapName("DMBE3")]
        public decimal? DMBE3 { get; set; }

        [Display(Name = "UPB 3")]
        [SapName("HWAE3")]
        public string? HWAE3 { get; set; }

        [Display(Name = "Telefon 1")]
        [SapName("TELF1")]
        public string? TELF1 { get; set; }

        [Display(Name = "Kent")]
        [SapName("ORT01")]
        public string? ORT01 { get; set; }

        [Display(Name = "Vergi Dairesi")]
        [SapName("STCD1")]
        public string? STCD1 { get; set; }

        [Display(Name = "Vergi Numarası")]
        [SapName("STCD2")]
        public string? STCD2 { get; set; }

        [Display(Name = "Malzeme")]
        [SapName("U_MATNR")]
        public string? U_MATNR { get; set; }

        [Display(Name = "Masraf yeri")]
        [SapName("KOSTL")]
        public string? KOSTL { get; set; }

        [Display(Name = "Giriş tarihi")]
        [SapName("U_CPUDT")]
        public DateTime? U_CPUDT { get; set; }

        [Display(Name = "Giriş saati")]
        [SapName("U_CPUTM")]
        public DateTime? U_CPUTM { get; set; }

        [Display(Name = "Kullanıcının adı")]
        [SapName("U_USNAM")]
        public string? U_USNAM { get; set; }
    }
}
