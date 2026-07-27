using AskalePortal.Data.Models;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.InputParams
{
    public class AccontPaymentApi
    {
        [SapName("apiKey")]
        public string? apiKey { get; set; }

        [SapName("odemeEmri")]
        public AccountPaymentSAPTable? odemeEmri { get; set; }
    }

    //public class AccountPaymentSapDto
    //{

    //    [SapName("OENUM")]
    //    public string? oenum { get; set; }

    //    [SapName("CPUDT")]
    //    public string? cpudt { get; set; }

    //    [SapName("CPUTM")]
    //    public string? cputm { get; set; }

    //    [SapName("USNAM")]
    //    public string? usnam { get; set; }

    //    [SapName("AEDAT")]
    //    public string? aedat { get; set; }

    //    [SapName("AEUHR")]
    //    public string? aeuhr { get; set; }

    //    [SapName("AENAM")]
    //    public string? aenam { get; set; }

    //    [SapName("BUKRS")]
    //    public string? bukrs { get; set; }

    //    [SapName("HKONT")]
    //    public string? hkont { get; set; }

    //    [SapName("KURUMKODU")]
    //    public string? korumkodu { get; set; }

    //    [SapName("SUBEKODU")]
    //    public string? subekodu { get; set; }

    //    [SapName("NAME1")]
    //    public string? name1 { get; set; }

    //    [SapName("UNVA1")]
    //    public string? unva1 { get; set; }

    //    [SapName("NAME2")]
    //    public string? name2 { get; set; }

    //    [SapName("UNVA2")]
    //    public string? unva2 { get; set; }

    //    [SapName("BSTAT")]
    //    public string? bstat { get; set; }

    //    [SapName("BELNR")]
    //    public string? belnr { get; set; }

    //    [SapName("GJAHR")]
    //    public string? gjahr { get; set; }

    //    [SapName("ZSAYINO")]
    //    public string? zsayino { get; set; }

    //    [SapName("ZNOT")]
    //    public string? znot { get; set; }

    //    [SapName("BANKL")]
    //    public string? bankl { get; set; }

    //    [SapName("BANKN")]
    //    public string? bankn { get; set; }

    //    [SapName("IBAN")]
    //    public string? iban { get; set; }

    //    [SapName("AccountPaymentKalemSAPTables")]
    //    public List<AccountPaymentKalemSAPTableDto>? listAccountPaymentKalemSAPTableDto { get; set; }

    //}

    public class AccountPaymentKalemSAPTableDto
    {
        [SapName("OENUM")]
        public string? oenum { get; set; }

        [SapName("POSNR")]
        public string? posnr { get; set; }

        [SapName("LIFNR")]
        public string? lifnr { get; set; }

        [SapName("NAME1")]
        public string? name1 { get; set; }

        [SapName("WRBTR")]
        public string? wrbtr { get; set; }

        [SapName("BANKS")]
        public string? banks { get; set; }

        [SapName("BANKL")]
        public string? bankl { get; set; }

        [SapName("BANKN")]
        public string? bankn { get; set; }

        [SapName("BANKA")]
        public string? banka { get; set; }

        [SapName("BRNCH")]
        public string? brnch { get; set; }

        [SapName("IBAN")]
        public string? iban { get; set; }


        [SapName("ODEME_NO")]
        public string? odemeNo { get; set; }

    }


}
