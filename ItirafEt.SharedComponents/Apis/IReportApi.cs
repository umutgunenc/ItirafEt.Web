using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IReportApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createReportType")]
        Task<ApiResponses<CreateReportTypeViewModel>> CreateReportTypeAsync(CreateReportTypeViewModel model);

        [Headers("Authorization: Bearer")]
        [Post("/api/editReportType")]
        Task<ApiResponses<CreateReportTypeViewModel>> EditReportTypeAsync(CreateReportTypeViewModel model);

        [Headers("Authorization: Bearer")]
        [Get("/api/getAllReportTypes")]
        Task<ApiResponses<List<CreateReportTypeViewModel>>> GetAllReportTypeAsync();


        [Headers("Authorization: Bearer")]
        [Get("/api/getAllActiveReportTypes/{reportClass}")]
        Task<ApiResponses<List<CreateReportTypeViewModel>>> GetAllActiveReportTypeAsync(ReportClassEnum reportClass);


        [Headers("Authorization: Bearer")]
        [Get("/api/getReports")]
        Task<ApiResponses<List<ReportListViewModel>>> GetReportsAsync( [AliasAs("status")] ReportStatusEnum status , [AliasAs("reportClass")] ReportClassEnum? reportClass = null );

        [Headers("Authorization: Bearer")]
        [Post("/api/createReport")]
        Task<ApiResponses> CreateReportAsync(SendReportViewModel model);
    }
}


