using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskalePortal.Constants
{
    public class CommonConstants
    {
        public enum ActionType
        {
            [System.ComponentModel.Description("ADD")]
            Add,

            [System.ComponentModel.Description("DELETE")]
            Delete,

            [System.ComponentModel.Description("UPDATE")]
            Update,

            [System.ComponentModel.Description("APPROVE")]
            Approve,
            [System.ComponentModel.Description("CANSEELOGS")]
            CanSeeLogs
        }
        public enum IZINTURLERI
        {
            YILLIKIZIN = 2,
            MAZARETIZIN = 3,
            UCRETSIZIZIN = 4,
            DIGER = 5,
            EVLENMEIZNI = 6,
            OLUMIZNI = 7,
            DOGUMIZNI = 8
        }

        public static string HTMLDonusum(string input)
        {
            return Regex.Replace(input, "(<([^>]+)>|&nbsp;)", string.Empty);
        }
        public enum MODULES
        {
            ROLLER = 1,
            KULLANICILAR = 2,
            IP_ADRESLERI = 3,
            GIRIS_LOGLARI = 4,
            ISLEM_LOGLARI = 5,
            SATIS_OZET = 6,
            FIRMA_BAZLI_RAPOR = 7,
            URUN_BAZLI_RAPOR = 8,
            CALISAN_SAYISI = 9,
            KANTAR_RAPORU = 10,
            SIRKETLER = 11,
            AYARLAR = 12,
            TOPLANTI = 13,
            TOPLANTI_KATILIMCILARI = 14,
            TOPLANTI_DETAYLARI = 15,
            KULLANICI_GRUPLARI = 16,
            HELPDESK_TYPE = 17,
            HELPDESK_STATUS = 18,
            HELPDESK_CATEGORY = 19,
            HELPDESK_DEMANDS = 20,
            HELPDESK_DEMANDS_RULES = 21,
            ANNOUNCEMENTS = 22,
            HELPDESK_MESSAGES = 23,
            FAQS = 24,
            HELPDESK_ROLE = 25,
            DOCUMENTS = 26,
            HELPDESK_REPORTS = 27,
            DOCUMENT_ARCHIVES = 28,
            INCOMING_DOCUMENTS = 29,
            INCOMING_DEPARTMENTS = 30,
            INCOMING_DOCUMENT_TYPES = 31,
            INCOMING_DOCUMENT_SOURCES = 32,
            HR_DOCUMENTS = 33,
            HR_ANNOUNCEMENTS = 34,
            PRESS_ANNOUNCEMENTS = 35,
            RATINGS = 36,
            TORBA_DOKME_RAPORU = 37,
            CUSTOMERS = 38,
            APPROVAL_PROCESSES = 39,
            CUSTOMER_CREDITS = 40,
            CUSTOMER_DOCUMENTS = 41,
            PERFORMANCE = 42,
            KVK = 43,
            MUSTERI_SIKAYET_FORM = 44,
            MUSTERI_SIKAYET_EMAIL = 45,
            MUSTERI_SIKAYET_BILDIRIM_TIPI = 46,
            MUSTERI_SIKAYET_TIPI = 47,
            MUSTERI_SIKAYET_CATEGORY = 48,
            MUSTERI_SIKAYET_AKSIYON = 49,
            MUSTERI_SIKAYET_CLOSE = 50,
            SEVKIYAT_PLAKA_TANIMLAMA = 51,
            AYLIK_ORANLI_FIRMA_SATIS = 52,
            ISTAKIP = 53,
            SOZLESMEGIRIS = 54,
            SOZLESMETEMINAT = 55,
            EGITIM_EKLE = 56,
            TOPLANTI_KARARLARI = 57,
            TOPLANTI_SABAH_GIDEN_MAIL = 58,
            TOPLANTI_ISLETME_GIDEN_MAIL = 59,
            SANAL_LIMIT_OLUSTUR = 60,
            FABRIKAMUDURLERITOPLANTI = 61,
            FABRIKAMUDURLERITOPLANTI_KATILIMCILARI = 62,
            FABRIKAMUDURLERITOPLANTI_DETAYLARI = 63,
            MAVI_YAKA = 64,
            HR_UCRET = 65,
            HR_EXPENSE_CONTROL = 66,
            FAZLA_MESAI = 67,
            SATIS_FIYAT_ONAYI = 68,
            ISGAKSIYON = 69,
            ISGAKSIYONTAKIP = 70,
            DIGITALCORIDOR = 71,
            ENERJI_GOSTER = 72,
            DAHILIYAZISMA = 73,
            ACCOUNTPAYMENT = 74,
            Kurumsal = 75,
            PROFIT = 76,
            ICRA = 77,
            ISGUSER = 78,
            ARACLAR = 79,
            GUNLUKCEKLER = 80,
            ANNUALCALENDAR = 81,
            ISGGUNSAYISI = 82,
            ANNUALLEAVE = 83,
            ISGORUSMESI = 84,
            EARSİV = 85,
            YAKITSOZLESME = 86,
            ARACTALEP = 87,
            ICYAZISMA = 88,
            TELEFONREHBERI = 89,
            DIGERKULLANICIIZIN = 90,
        }

        public enum ADMIN_USER_TYPES
        {
            Admin = 1,
            Company_User = 2
        }

        public enum APPROVAL_PROCESSES
        {
            CREDIT_LIMIT = 1,
            DOCUMENT_EXPIRY_DATE = 2,
            MOTORIN_CREDIT_LIMIT = 3,
            MOTORIN_EXPIRY_DATE = 4,
            YENI_MUSTERI = 5,
            HAFTALIK_MUSTERI = 6,
            ACCOUNT_PAYMENT = 7,
            YAKITSOZLESME=8,
            MOTORINFIYAT=9,
        }

        public enum PROCESS_STATES
        {
            ACTIVE = 1,
            DECLINED = 2,
            COMPLETED = 3,
            SAP_COMPLETED = 4
        }

        public enum USER_FORM_TYPES
        {
            BIREYSEL_PERFORMANS = 1,
            BIREYSEL_PERFORMANS_ARA = 2,
            YETKINLIK_DEGERLENDIRME_LIDER = 3,
            YETKINLIK_DEGERLENDIRME_TEMEL = 4
        }

        public static class OkNoLinks
        {
            public const string OK_LINK = "http://sapweb:8083/";
            public const string NO_LINK = "http://sapweb.askalecimento.com.tr:8380";
        }
      
    }
}
