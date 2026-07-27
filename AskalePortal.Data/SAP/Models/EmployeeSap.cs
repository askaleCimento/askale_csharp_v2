using SapNwRfc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.SAP.Models
{
    public class EmployeeSap
    {
        [SapName("MANDT")]
        public string? MANDT { get; set; }

        [SapName("PERNR")]
        public string? PERNR { get; set; }

        [SapName("ENAME")]
        public string? ENAME { get; set; }

        [SapName("WERKS")]
        public string? WERKS { get; set; }

        [SapName("NAME1")]
        public string? NAME1 { get; set; }

        [SapName("BTRTL")]
        public string? BTRTL { get; set; }

        [SapName("BTEXT")]
        public string? BTEXT { get; set; }

        [SapName("PERSG")]
        public string? PERSG { get; set; }

        [SapName("PGTXT")]
        public string? PGTXT { get; set; }

        [SapName("PERSK")]
        public string? PERSK { get; set; }

        [SapName("PKTXT")]
        public string? PKTXT { get; set; }

        [SapName("ORGEH")]
        public string? ORGEH { get; set; }

        [SapName("ORGTX")]
        public string? ORGTX { get; set; }

        [SapName("PLANS")]
        public string? PLANS { get; set; }

        [SapName("PLSTX")]
        public string? PLSTX { get; set; }

        [SapName("STELL")]
        public string? STELL { get; set; }

        [SapName("STLTX")]
        public string? STLTX { get; set; }

        [SapName("KOSTL")]
        public string? KOSTL { get; set; }

        [SapName("CINSY")]
        public string? CINSY { get; set; }

        [SapName("SSTXT")]
        public string? SSTXT { get; set; }

        [SapName("WAERS")]
        public string? WAERS { get; set; }

        [SapName("SCHEM")]
        public string? SCHEM { get; set; }

        [SapName("BANKL")]
        public string? BANKL { get; set; }

        [SapName("BANKN")]
        public string? BANKN { get; set; }

        [SapName("IBAN")]
        public string? IBAN { get; set; }

        [SapName("MERNI")]
        public string? MERNI { get; set; }

        [SapName("SL_STEXT")]
        public string? SL_STEXT { get; set; }

        [SapName("KIDEM")]
        public string? KIDEM { get; set; }

        [SapName("EINDT")]
        public DateTime? EINDT { get; set; }

        [SapName("BDATE")]
        public string? BDATE { get; set; }

        [SapName("FREDT")]

        public string? FREDT { get; set; }

        [SapName("BLDGR")]
        public string? BLDGR { get; set; }

        [SapName("MRSTA")]
        public string? MRSTA { get; set; }

        [SapName("NUMCH")]
        public string? NUMCH { get; set; }

        [SapName("BRPLC")]
        public string? BRPLC { get; set; }

        [SapName("ADRFR")]
        public string? ADRFR { get; set; }

        [SapName("STAT2")]
        public string? STAT2 { get; set; }

        [SapName("STATX")]
        public string? STATX { get; set; }

        [SapName("SYSUNAME")]
        public string? SYSUNAME { get; set; }
    }
}
