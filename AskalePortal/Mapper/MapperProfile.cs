using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.ResponseModels;
using AutoMapper;

namespace AskalePortal.API.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AdminUser, AdminUserSaveDto>();
            CreateMap<AdminUserSaveDto, AdminUser>();
            CreateMap<CompanyDto, Company>();
            CreateMap<Company, CompanyDto>();
            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleDto>();
            CreateMap<HelpDeskDemand, HelpDeskDemandSaveDto>();
            CreateMap<HelpDeskDemandSaveDto, HelpDeskDemand>();
            CreateMap<AracTalepTableSaveDto, AracTalepTable>();
            CreateMap<AracTalepTable, AracTalepTableSaveDto>();
            CreateMap<KurumsalDocumentsDto, KurumsalDocument>();
            CreateMap<KurumsalDocument, KurumsalDocumentsDto>();
            CreateMap<AnnualLeaveTableDto, AnnualLeaveTable>();
            CreateMap<AnnualLeaveTable, AnnualLeaveTableDto>();

            CreateMap<AnnualLeaveTableSaveDto, AnnualLeaveTable>()
                .ForMember(destination => destination.Id, option => option.MapFrom(source => source.id ?? 0))
                .ForMember(destination => destination.enteredDate, option => option.MapFrom(source => ParseRequiredDate(source.enteredDate, nameof(source.enteredDate))))
                .ForMember(destination => destination.dayleft, option => option.MapFrom(source => Convert.ToDecimal(source.dayleft ?? 0d)))
                .ForMember(destination => destination.dayRequested, option => option.MapFrom(source => Convert.ToDecimal(source.dayRequested ?? 0d)))
                .ForMember(destination => destination.startDate, option => option.MapFrom(source => ParseRequiredDate(source.startDate, nameof(source.startDate))))
                .ForMember(destination => destination.endDate, option => option.MapFrom(source => ParseRequiredDate(source.endDate, nameof(source.endDate))))
                .ForMember(destination => destination.createdDate, option => option.MapFrom(source => ParseNullableDate(source.createdDate)))
                .ForMember(destination => destination.updatedDate, option => option.MapFrom(source => ParseNullableDate(source.updateDate)))
                .ForMember(destination => destination.AnnualLeaveDetail, option => option.Ignore())
                .ForMember(destination => destination.HRAnnualSapIntegration, option => option.Ignore())
                .ForMember(destination => destination.currentUser, option => option.Ignore())
                .ForMember(destination => destination.typeNavigation, option => option.Ignore())
                .ForMember(destination => destination.user, option => option.Ignore())
                .ForMember(destination => destination.vekalet, option => option.Ignore());

            CreateMap<AnnualLeaveTable, AnnualLeaveTableSaveDto>()
                .ForMember(destination => destination.id, option => option.MapFrom(source => source.Id))
                .ForMember(destination => destination.enteredDate, option => option.MapFrom(source => FormatDate(source.enteredDate)))
                .ForMember(destination => destination.dayleft, option => option.MapFrom(source => Convert.ToDouble(source.dayleft)))
                .ForMember(destination => destination.dayRequested, option => option.MapFrom(source => Convert.ToDouble(source.dayRequested)))
                .ForMember(destination => destination.startDate, option => option.MapFrom(source => FormatDate(source.startDate)))
                .ForMember(destination => destination.endDate, option => option.MapFrom(source => FormatDate(source.endDate)))
                .ForMember(destination => destination.createdDate, option => option.MapFrom(source => FormatNullableDate(source.createdDate)))
                .ForMember(destination => destination.updateDate, option => option.MapFrom(source => FormatNullableDate(source.updatedDate)));
            CreateMap<Rating, RatingDto>();
            CreateMap<RatingDto, Rating>();
            CreateMap<RatingQuestionDto, RatingQuestion>();
            CreateMap<RatingQuestion, RatingQuestionDto>();
            CreateMap<DieselPriceDto, DieselPrice>();
            CreateMap<DieselPrice, DieselPriceDto>();
            CreateMap<FuelPriceDifferenceDto, FuelPriceDifference>();
            CreateMap<FuelPriceDifference, FuelPriceDifferenceDto>();
            CreateMap<UserByNameEMailDto, AdminUser>();
            CreateMap<AdminUser, UserByNameEMailDto>();
            CreateMap<SozlesmeTableSaveDto, SozlesmeTable>();
            CreateMap<SozlesmeTable, SozlesmeTableSaveDto>();
            CreateMap<SureliIsTakipSaveDto, SureliIsTakipTable>();
            CreateMap<SureliIsTakipTable, SureliIsTakipSaveDto>();
            CreateMap<CorporateDocumentsDto, KVKDocument>();
            CreateMap<KVKDocument, CorporateDocumentsDto>();
            CreateMap<KvkDocumentDto, KVKDocument>();
            CreateMap<KVKDocument, KvkDocumentDto>();
            CreateMap<IncomingDocument, IncomingDocumentSaveDto>();
            CreateMap<IncomingDocumentSaveDto, IncomingDocument>();
            CreateMap<IncomingDocumentSource, IncomingDocumentSourceSaveDto>();
            CreateMap<IncomingDocumentSourceSaveDto, IncomingDocumentSource>();
            CreateMap<IncomingDocumentType, IncomingDocumentTypeSaveDto>();
            CreateMap<IncomingDocumentTypeSaveDto, IncomingDocumentType>();
            CreateMap<IncomingDocumentType, ComingDocumentTypeDto>();
            CreateMap<ComingDocumentTypeDto, IncomingDocumentType>();
            CreateMap<Announcement, AnnouncementSaveDto>();
            CreateMap<AnnouncementSaveDto, Announcement>();
            CreateMap<Meeting, MeetingSaveDto>();
            CreateMap<MeetingSaveDto, Meeting>();
            CreateMap<FactoryManagerMeeting, FactoryManagerMeetingSaveDto>();
            CreateMap<FactoryManagerMeetingSaveDto, FactoryManagerMeeting>();
            CreateMap<FactoryManagerMeetingDetail, FactoryManagerMeetingDetailSaveDto>();
            CreateMap<FactoryManagerMeetingDetailSaveDto, FactoryManagerMeetingDetail>();
            CreateMap<HRExpenseTypeTable, HRExpenseTypeTableSaveDto>();
            CreateMap<HRExpenseTypeTableSaveDto, HRExpenseTypeTable>();
            CreateMap<HRTripDescription, HRTripDescriptionSaveDto>();
            CreateMap<HRTripDescriptionSaveDto, HRTripDescription>();
            CreateMap<HREmployeeType, HREmployeeTypeSaveDto>();
            CreateMap<HREmployeeTypeSaveDto, HREmployeeType>();
            CreateMap<FinansUserTable, FinansUserTableSaveDto>();
            CreateMap<FinansUserTableSaveDto, FinansUserTable>();
            CreateMap<HRGidisYeri, HRGidisYeriSaveDto>();
            CreateMap<HRGidisYeriSaveDto, HRGidisYeri>();
            CreateMap<HRDestinationLocationTable, HRDestinationLocationTableSaveDto>();
            CreateMap<HRDestinationLocationTableSaveDto, HRDestinationLocationTable>();
            CreateMap<HRExpenseAmount, HRExpenseAmountSaveDto>();
            CreateMap<HRExpenseAmountSaveDto, HRExpenseAmount>();
            CreateMap<RepresentativeExpenseTable, RepresentativeExpenseTableSaveDto>();
            CreateMap<RepresentativeExpenseTableSaveDto, RepresentativeExpenseTable>();
            CreateMap<HRExpenseWithOutTable, HRExpenseWithOutTableSaveDto>();
            CreateMap<HRExpenseWithOutTableSaveDto, HRExpenseWithOutTable>();
            CreateMap<HRExpenseWithOutTripTable, HRExpenseWithOutTripTableSaveDto>();
            CreateMap<HRExpenseWithOutTripTableSaveDto, HRExpenseWithOutTripTable>();
            CreateMap<HRExpenseTripTable, HRExpenseTripTableSaveDto>();
            CreateMap<HRExpenseTripTableSaveDto, HRExpenseTripTable>();
            CreateMap<HRExpenseTable, HRExpenseTableSaveDto>();
            CreateMap<HRExpenseTableSaveDto, HRExpenseTable>();
            CreateMap<HelpDeskStatus, HelpDeskStatusSaveDto>();
            CreateMap<HelpDeskStatusSaveDto, HelpDeskStatus>();
            CreateMap<CompanySection, CompanySectionSaveDto>();
            CreateMap<CompanySectionSaveDto, CompanySection>();
            CreateMap<CompanySaveDto, Company>();
            CreateMap<Company, CompanySaveDto>();
            CreateMap<HelpDeskCategorySaveDto, HelpDeskCategory>();
            CreateMap<HelpDeskCategory, HelpDeskCategorySaveDto>();
            CreateMap<HelpDeskDemandRuleSaveDto, HelpDeskDemandRule>();
            CreateMap<HelpDeskDemandRule, HelpDeskDemandRuleSaveDto>();
            CreateMap<HelpDeskMessageSaveDto, HelpDeskMessage>();
            CreateMap<HelpDeskMessage, HelpDeskMessageSaveDto>();
            CreateMap<ISGGunTableSaveDto, ISGGunTable>();
            CreateMap<ISGGunTable, ISGGunTableSaveDto>();
            CreateMap<UserTelephoneTableSaveDto, UserTelephoneTable>();
            CreateMap<UserTelephoneTable, UserTelephoneTableSaveDto>();
            CreateMap<InternalCorrespondenceSaveDto, DahiliYazismaTable>();
            CreateMap<DahiliYazismaTable, InternalCorrespondenceSaveDto>();
            CreateMap<IcYazismalarTableSaveDto, IcYazismalarTable>();
            CreateMap<IcYazismalarTable, IcYazismalarTableSaveDto>();
            CreateMap<HRDepartmanTableSaveDto, HRDepartmanTable>();
            CreateMap<HRDepartmanTable, HRDepartmanTableSaveDto>();
            CreateMap<RoleDetailSaveDto, RoleDetail>();
            CreateMap<RoleDetail, RoleDetailSaveDto>();
            CreateMap<HelpDeskTypeSaveDto, HelpDeskType>();
            CreateMap<HelpDeskType, HelpDeskTypeSaveDto>();
            CreateMap<HelpDeskRoleSaveDto, HelpDeskRole>();
            CreateMap<HelpDeskRole, HelpDeskRoleSaveDto>();
            CreateMap<FaqSaveDto, Faq>();
            CreateMap<Faq, FaqSaveDto>();
            CreateMap<DocumentArchiveSaveDto, DocumentArchive>();
            CreateMap<DocumentArchive, DocumentArchiveSaveDto>(); 
            CreateMap<AnnualCalenderTableSaveDto, AnnualCalenderTable>();
            CreateMap<AnnualCalenderTable, AnnualCalenderTableSaveDto>();
            CreateMap<KurumsalDocumentSaveDto, KurumsalDocument>();
            CreateMap<KurumsalDocument, KurumsalDocumentSaveDto>();
            CreateMap<EArsivFaturaSaveDto, EArsivFatura>();
            CreateMap<EArsivFatura, EArsivFaturaSaveDto>();
            CreateMap<ApprovalProcessSaveDto, ApprovalProcess>();
            CreateMap<ApprovalProcess, ApprovalProcessSaveDto>();
            CreateMap<ApprovalProcessDetailSaveDto, ApprovalProcessDetail>();
            CreateMap<ApprovalProcessDetail, ApprovalProcessDetailSaveDto>();
            CreateMap<MusteriSikayetTipiSaveDto, MusteriSikayetTipi>();
            CreateMap<MusteriSikayetTipi, MusteriSikayetTipiSaveDto>();
            CreateMap<CustomerComplaintSaveDto, MusteriSikayetForm>();
            CreateMap<MusteriSikayetForm, CustomerComplaintSaveDto>();
            CreateMap<CustomerComplaintActionSaveDto, MusteriSikayetAction>();
            CreateMap<MusteriSikayetAction, CustomerComplaintActionSaveDto>();
        }

        private static DateTime ParseRequiredDate(string? value, string fieldName)
        {
            if (TryParseDate(value, out var parsed))
            {
                return parsed;
            }

            throw new AutoMapperMappingException($"{fieldName} alanı geçerli bir tarih değil: '{value}'.");
        }

        private static DateTime? ParseNullableDate(string? value)
        {
            return TryParseDate(value, out var parsed) ? parsed : null;
        }

        private static bool TryParseDate(string? value, out DateTime parsed)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                parsed = default;
                return false;
            }

            var formats = new[]
            {
                "yyyyMMddHHmmss",
                "yyyyMMdd",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd",
                "dd.MM.yyyy HH:mm:ss",
                "dd.MM.yyyy HH:mm",
                "dd.MM.yyyy"
            };

            return DateTime.TryParseExact(
                       value.Trim(),
                       formats,
                       System.Globalization.CultureInfo.InvariantCulture,
                       System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                       out parsed)
                   || DateTime.TryParse(
                       value.Trim(),
                       System.Globalization.CultureInfo.GetCultureInfo("tr-TR"),
                       System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                       out parsed)
                   || DateTime.TryParse(
                       value.Trim(),
                       System.Globalization.CultureInfo.InvariantCulture,
                       System.Globalization.DateTimeStyles.AllowWhiteSpaces,
                       out parsed);
        }

        private static string FormatDate(DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string? FormatNullableDate(DateTime? value)
        {
            return value.HasValue ? FormatDate(value.Value) : null;
        }

    }
    
}
