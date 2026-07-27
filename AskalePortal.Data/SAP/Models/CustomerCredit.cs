using System.Text.Json.Serialization;
using SapNwRfc;

namespace AskalePortal.Data.SAP.Models
{
    public class CustomerCredit
    {
        [SapName("KUNNR")]
        [JsonPropertyName("kunnr")]
        public string? KUNNR { get; set; }

        [SapName("NAME1")]
        [JsonPropertyName("name1")]
        public string? NAME1 { get; set; }

        [SapName("DMBTR")]
        [JsonPropertyName("dmbtr")]
        public string? DMBTR { get; set; }

        [SapName("DMBTR_120")]
        [JsonPropertyName("dmbtr120")]
        public string? DMBTR_120 { get; set; }

        [SapName("DMBTR_A")]
        [JsonPropertyName("dmbtra")]
        public string? DMBTR_A { get; set; }

        [SapName("DMBTR_G")]
        [JsonPropertyName("dmbtrg")]
        public string? DMBTR_G { get; set; }

        [SapName("RISK_BORC")]
        [JsonPropertyName("riskborc")]
        public string? RISK_BORC { get; set; }

        [SapName("DMBTR_T")]
        [JsonPropertyName("dmbtrt")]
        public string? DMBTR_T { get; set; }

        [SapName("ACIK_CEK_M")]
        [JsonPropertyName("acikcekm")]
        public string? ACIK_CEK_M { get; set; }

        [SapName("ACIK_CEK_K")]
        [JsonPropertyName("acikcekk")]
        public string? ACIK_CEK_K { get; set; }

        [SapName("ACIK_SD")]
        [JsonPropertyName("aciksd")]
        public string? ACIK_SD { get; set; }

        [SapName("TOP_BORC")]
        [JsonPropertyName("topborc")]
        public string? TOP_BORC { get; set; }

        [SapName("ACIK_SENET_M")]
        [JsonPropertyName("aciksenetm")]
        public string? ACIK_SENET_M { get; set; }

        [SapName("ACIK_SENET_K")]
        [JsonPropertyName("aciksenetk")]
        public string? ACIK_SENET_K { get; set; }

        [SapName("KREDI_KUL")]
        [JsonPropertyName("kredikul")]
        public string? KREDI_KUL { get; set; }

        [SapName("KLIMK")]
        [JsonPropertyName("klimk")]
        public string? KLIMK { get; set; }

        [SapName("DMBTR_VADE1")]
        [JsonPropertyName("dmbtrvade1")]
        public string? DMBTR_VADE1 { get; set; }

        [SapName("DMBTR_VADE2")]
        [JsonPropertyName("dmbtrvade2")]
        public string? DMBTR_VADE2 { get; set; }

        [SapName("DMBTR_VADE3")]
        [JsonPropertyName("dmbtrvade3")]
        public string? DMBTR_VADE3 { get; set; }

        [SapName("DMBTR_VADE4")]
        [JsonPropertyName("dmbtrvade4")]
        public string? DMBTR_VADE4 { get; set; }

        [SapName("DMBTR_VADE5")]
        [JsonPropertyName("dmbtrvade5")]
        public string? DMBTR_VADE5 { get; set; }

        [SapName("DMBTR2_VADE1")]
        [JsonPropertyName("dmbtr2vade1")]
        public string? DMBTR2_VADE1 { get; set; }

        [SapName("DMBTR2_VADE2")]
        [JsonPropertyName("dmbtr2vade2")]
        public string? DMBTR2_VADE2 { get; set; }

        [SapName("DMBTR2_VADE3")]
        [JsonPropertyName("dmbtr2vade3")]
        public string? DMBTR2_VADE3 { get; set; }

        [SapName("DMBTR2_VADE4")]
        [JsonPropertyName("dmbtr2vade4")]
        public string? DMBTR2_VADE4 { get; set; }

        [SapName("DMBTR2_VADE5")]
        [JsonPropertyName("dmbtr2vade5")]
        public string? DMBTR2_VADE5 { get; set; }

        [SapName("DMBTR_T_901")]
        [JsonPropertyName("dmbtrt901")]
        public string? DMBTR_T_901 { get; set; }

        [SapName("DMBTR_T_902")]
        [JsonPropertyName("dmbtrt902")]
        public string? DMBTR_T_902 { get; set; }

        [SapName("SNLMT")]
        [JsonPropertyName("snlmt")]
        public string? SNLMT { get; set; }

        [SapName("KLLMT")]
        [JsonPropertyName("kllmt")]
        public string? KLLMT { get; set; }
    }
}