using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
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

            if (await _context.ReportTypes.AnyAsync(rt => rt.Name.ToLower() == model.Name.Trim().ToLower()))
                return ApiResponses<CreateReportViewModel>.Fail("Bu isimde zaten bir şikayet türü mevcut.");

            var reportType = new ReportType
            {
                Name = model.Name.Trim(),
                IsActive = true,
                IconUrl = model.IconUrl?.Trim()
            };

            await _context.ReportTypes.AddAsync(reportType);
            await _context.SaveChangesAsync();

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

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(rt => rt.Id == model.Id);

            if (reportType == null)
                return ApiResponses<CreateReportViewModel>.Fail("Böyle bir şikayet türü bulunamadı.");

            var nameExists = await _context.ReportTypes
                .AsNoTracking()
                .AnyAsync(rt => rt.Id != model.Id && rt.Name.ToLower() == model.Name.Trim().ToLower());

            if (nameExists)
                return ApiResponses<CreateReportViewModel>.Fail("Bu şikayet adı zaten mevcut.");

            reportType.IsActive = model.IsActive;
            reportType.IconUrl = model.IconUrl?.Trim();
            reportType.Name = model.Name.Trim();

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
                    IconUrl = rt.IconUrl
                })
                .ToListAsync();
            return ApiResponses<List<CreateReportViewModel>>.Success(reportTypes);
        }

        public async Task<ApiResponses<List<CreateReportViewModel>>> GetAllActiveReportTypesAsync()
        {
            var reportTypes = await _context.ReportTypes
                .AsNoTracking()
                .Where(rt => rt.IsActive)
                .Select(rt => new CreateReportViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    IsActive = rt.IsActive,
                    IconUrl = rt.IconUrl
                })
                .ToListAsync();

            return ApiResponses<List<CreateReportViewModel>>.Success(reportTypes);
        }
    }
}
