using System.ComponentModel.DataAnnotations;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.Services.ClientServices;
using ItirafEt.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class ReportService
    {
        private readonly dbContext _context;

        public ReportService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<CreateReportViewModel>> CreateReportTypeAsync(CreateReportViewModel model)
        {

            if (string.IsNullOrEmpty(model.Name.Trim()))
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı boş olamaz.");

            if (model.Name.Trim().Length > 64)
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı maksimum 64 karakter uzunluğunda olabilir.");

            if (model.IconUrl?.Trim().Length > 128)
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet icon URLi maksimum 128 karakter uzunluğunda olabilir.");

            if (await _context.ReportTypes.AsNoTracking().AnyAsync(rt => rt.Name.ToLower() == model.Name.Trim().ToLower() && rt.ReportClass == model.ReportClass))
                return ApiResponses<CreateReportViewModel>.Fail("Bu isimde zaten bir şikayet türü mevcut.");

            if (!Enum.IsDefined(typeof(ReportClassEnum), model.ReportClass))
                return ApiResponses<CreateReportViewModel>.Fail("Geçersiz şikayet türü.");

            var reportType = new ReportType
            {
                Name = model.Name.Trim(),
                IsActive = true,
                IconUrl = model.IconUrl?.Trim(),
                ReportClass = (ReportClassEnum)model.ReportClass
            };

            await _context.ReportTypes.AddAsync(reportType);
            await _context.SaveChangesAsync();
            model.Id = reportType.Id;

            return ApiResponses<CreateReportViewModel>.Success(model);


        }

        public async Task<ApiResponses<CreateReportViewModel>> EditReportTypeAsync(CreateReportViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name.Trim()))
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı boş olamaz.");
            if (model.Name.Trim().Length > 64)
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı maksimum 64 karakter uzunluğunda olabilir.");
            if (model.IconUrl?.Trim().Length > 128)
                return ApiResponses<CreateReportViewModel>.Fail("Şikayet icon URLi maksimum 128 karakter uzunluğunda olabilir.");

            if (!Enum.IsDefined(typeof(ReportClassEnum), model.ReportClass))
                return ApiResponses<CreateReportViewModel>.Fail("Geçersiz şikayet türü.");

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(rt => rt.Id == model.Id);

            if (reportType == null)
                return ApiResponses<CreateReportViewModel>.Fail("Böyle bir şikayet türü bulunamadı.");

            if (await _context.ReportTypes.AsNoTracking().AnyAsync(rt => rt.Id != model.Id && rt.Name.ToLower() == model.Name.Trim().ToLower() && rt.ReportClass == model.ReportClass))
                return ApiResponses<CreateReportViewModel>.Fail("Bu isimde zaten bir şikayet türü mevcut.");

            reportType.IsActive = model.IsActive;
            reportType.IconUrl = model.IconUrl?.Trim();
            reportType.Name = model.Name.Trim();
            reportType.ReportClass = (ReportClassEnum)model.ReportClass;

            _context.ReportTypes.Update(reportType);
            await _context.SaveChangesAsync();

            return ApiResponses<CreateReportViewModel>.Success(model);
        }

        public async Task<ApiResponses<List<CreateReportViewModel>>> GetAllReportTypesAsync()
        {
            var reportTypes = await _context.ReportTypes
                .AsNoTracking()
                .Select(rt => new CreateReportViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    IsActive = rt.IsActive,
                    IconUrl = rt.IconUrl,
                    ReportClass = rt.ReportClass,
                })
                .ToListAsync();
            return ApiResponses<List<CreateReportViewModel>>.Success(reportTypes);
        }

        public async Task<ApiResponses<List<CreateReportViewModel>>> GetAllActiveReportTypesAsync(ReportClassEnum reportClassEnum)
        {
            var reportTypes = await _context.ReportTypes
                .AsNoTracking()
                .Where(rt => rt.IsActive && rt.ReportClass == reportClassEnum)
                .Select(rt => new CreateReportViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    IsActive = rt.IsActive,
                    IconUrl = rt.IconUrl,
                    ReportClass = rt.ReportClass,
                })
                .ToListAsync();

            return ApiResponses<List<CreateReportViewModel>>.Success(reportTypes);
        }

    }


}

