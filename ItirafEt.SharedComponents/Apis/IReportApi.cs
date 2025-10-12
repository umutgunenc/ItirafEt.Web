using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IReportApi
    {
        [Headers("Authorization: Bearer")]
        [Post("/api/createReportType")]
        Task<ApiResponses<CreateReportViewModel>> CreateReportTypeAsync(CreateReportViewModel model);

        [Headers("Authorization: Bearer")]
        [Post("/api/editReportType")]
        Task<ApiResponses<CreateReportViewModel>> EditReportTypeAsync(CreateReportViewModel model);

        [Headers("Authorization: Bearer")]
        [Get("/api/getAllReportTypes")]
        Task<ApiResponses<List<CreateReportViewModel>>> GetAllReportTypeAsync();

    }
}
