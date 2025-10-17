using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.EndPoints
{
    public static class ReportEndPoints
    {
        public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/createReportType", async (CreateReportTypeViewModel model, ReportService reportService) =>
            {
                try
                {
                    return Results.Ok(await reportService.CreateReportTypeAsync(model));

                }
                catch (Exception ex)
                {
                    return Results.Ok(ApiResponses<CreateReportTypeViewModel>.Fail(ex.Message));
                }

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));


            app.MapPost("/api/editReportType", async (CreateReportTypeViewModel model, ReportService reportService) =>
            {

                return Results.Ok(await reportService.EditReportTypeAsync(model));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapGet("/api/getAllReportTypes", async (ReportService reportService) =>
            {
                return Results.Ok(await reportService.GetAllReportTypesAsync());

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin)));

            app.MapGet("/api/getAllActiveReportTypes/{reportClass}", async (ReportService reportService, ReportClassEnum reportClass) =>
            {
                return Results.Ok(await reportService.GetAllActiveReportTypesAsync(reportClass));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.User), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.Moderator)));

            app.MapGet("/api/getReports", async (ReportService reportService, ReportStatusEnum status, ReportClassEnum? reportClass) =>
            {
                return Results.Ok(await reportService.GetReportedItemsAsync(status, reportClass));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.SuperAdmin), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.Moderator)));

            app.MapPost("/api/createReport", async (SendReportViewModel model, ReportService reportService) =>
            {
                return Results.Ok(await reportService.SendReportAsync(model));

            }).RequireAuthorization(p => p.RequireRole(nameof(UserRoleEnum.User), nameof(UserRoleEnum.SuperUser), nameof(UserRoleEnum.Moderator), nameof(UserRoleEnum.Admin), nameof(UserRoleEnum.SuperAdmin)));


            return app;
        }
    }
}
