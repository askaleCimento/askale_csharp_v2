using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class FirmaBazliGunlukRapor
    {
        [SapName("MANDT")]
        public string? MANDT { get; set; }
        [SapName("TARIH")]
        public DateTime? TARIH { get; set; }
        [SapName("SATORG")]
        public string? SATORG { get; set; }
        [SapName("MUSTERI")]
        public string? MUSTERI { get; set; }
        [SapName("MUSTERITNM")]
        public string? MUSTERITNM { get; set; }
        [SapName("MALZEME")]
        public string? MALZEME { get; set; }
        [SapName("MALZEMETNM")]
        public string? MALZEMETNM { get; set; }
        [SapName("DAGITIMKANALI")]
        public string? DAGITIMKANALI { get; set; }
        [SapName("DAGITIMKANALITNM")]
        public string? DAGITIMKANALITNM { get; set; }
        [SapName("BOLGE")]
        public string? BOLGE { get; set; }
        [SapName("BOLGETNM")]
        public string? BOLGETNM { get; set; }
        [SapName("TESLIMAT")]
        public string? TESLIMAT { get; set; }
        [SapName("KALEM")]
        public string? KALEM { get; set; }
        [SapName("ODEMEKOSULU")]
        public string? ODEMEKOSULU { get; set; }
        [SapName("TESLIMATMIKTARI")]
        public decimal? TESLIMATMIKTARI { get; set; }
        [SapName("MIKTARBIRIM")]
        public string? MIKTARBIRIM { get; set; }
        [SapName("KOSULTUTARI")]
        public decimal? KOSULTUTARI { get; set; }
        [SapName("NAKLIYELIFIYAT")]
        public decimal? NAKLIYELIFIYAT { get; set; }
        [SapName("PARABIRIM")]
        public string? PARABIRIM { get; set; }
        [SapName("BRUTFIYAT")]
        public decimal? BRUTFIYAT { get; set; }
        [SapName("INDIRIMTOPLAMI")]
        public decimal? INDIRIMTOPLAMI { get; set; }
        [SapName("INDIRIMSONRASI")]
        public decimal? INDIRIMSONRASI { get; set; }
        [SapName("NAKLIYESIGORTA")]
        public decimal? NAKLIYESIGORTA { get; set; }
        [SapName("NETDEGER")]
        public decimal? NETDEGER { get; set; }
        [SapName("KDV")]
        public decimal? KDV { get; set; }
        [SapName("NIHAITUTAR")]
        public decimal? NIHAITUTAR { get; set; }

    }
}
