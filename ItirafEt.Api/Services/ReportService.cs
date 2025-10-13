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
            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                    return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı boş olamaz.");

                if (model.Name.Trim().Length > 64)
                    return ApiResponses<CreateReportViewModel>.Fail("Şikayet adı maksimum 64 karakter uzunluğunda olabilir.");

                if (model.IconUrl?.Trim().Length > 128)
                    return ApiResponses<CreateReportViewModel>.Fail("Şikayet icon URLi maksimum 128 karakter uzunluğunda olabilir.");

                if (await _context.ReportTypes.AnyAsync(rt => rt.Name.ToLower() == model.Name.Trim().ToLower()))
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

                return ApiResponses<CreateReportViewModel>.Success(model);
            }
            catch (Exception ex)
            {
                return ApiResponses<CreateReportViewModel>.Fail(ex.Message);
            }

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

            var nameExists = await _context.ReportTypes
                .AsNoTracking()
                .AnyAsync(rt => rt.Id != model.Id && rt.Name.ToLower() == model.Name.Trim().ToLower());

            if (nameExists)
                return ApiResponses<CreateReportViewModel>.Fail("Bu şikayet adı zaten mevcut.");

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

        //public async Task<ReportedItemViewModel> GetAllReportedPostsAsync(ReportStatusEnum status)
        //{
        //    var reportedPosts = await _context.Reports
        //        .AsNoTracking()
        //        .Where(pr => pr.Status == status)
        //        .GroupBy(pr => pr.PostId)
        //        .Select(g => new ReportedItemViewModel
        //        {
        //            TotalCount = g.Count(),
        //            StatusName = EnumExtensions.GetEnumDisplayName(status),
        //            ReportedItemContent = g.First().Post.Content.Length > 100 ? g.First().Post.Content.Substring(0, 100) + "..." : g.First().Post.Content
        //        })
        //        .ToListAsync();


        //}

        //public async Task<ApiResponses> ReportPostAsync(ReportPostViewModel model)
        //{
        //    var postId = await _context.Posts
        //        .AsNoTracking()
        //        .Where(p => p.Id == model.PostId)
        //        .Select(p => p.Id)
        //        .FirstOrDefaultAsync();

        //    if (postId == 0)
        //        return ApiResponses.Fail("Gönderi bulunamadı.");

        //    var userId = await _context.Users
        //        .AsNoTracking()
        //        .Where(u => u.Id == model.UserId)
        //        .Select(u => u.Id)
        //        .FirstOrDefaultAsync();

        //    if (userId == Guid.Empty)
        //        return ApiResponses.Fail("Kullanıcı bulunamadı.");

        //    var reportType = await _context.ReportTypes
        //        .AsNoTracking()
        //        .Where(rt => rt.Id == model.ReportTypeId && rt.IsActive)
        //        .Select(rt => rt.Id)
        //        .FirstOrDefaultAsync();

        //    if (reportType == 0)
        //        return ApiResponses.Fail("Geçersiz şikayet türü.");

        //    var alreadyReported = await _context.PostReports
        //        .AsNoTracking()
        //        .AnyAsync(pr => pr.PostId == model.PostId && pr.ReportingUserId == model.UserId);

        //    if (alreadyReported)
        //        return ApiResponses.Fail("Bu gönderiyi zaten şikayet ettiniz.");

        //    var postReport = new Report
        //    {
        //        PostId = postId,
        //        ReportingUserId = userId,
        //        ReportTypeId = reportType,
        //        ReportExplanation = string.IsNullOrEmpty(model.ReportExplanation?.Trim()) ? null : model.ReportExplanation?.Trim(),
        //        CreatedDate = DateTime.UtcNow,
        //        Status = ReportStatusEnum.Pending
        //    };

        //    await _context.PostReports.AddAsync(postReport);
        //    await _context.SaveChangesAsync();
        //    return ApiResponses.Success();
        //}
    }

    public class ReportPostViewModel
    {
        public Guid UserId { get; set; }

        public int PostId { get; set; }

        [Required(ErrorMessage = "Lütfen Şikayet Türü Seçiniz.")]
        public int ReportTypeId { get; set; }

        [MaxLength(1024, ErrorMessage = "Şikayet Açıklaması Maksimum 1024 karakter uzunluğunda olabilir.")]
        public string? ReportExplanation { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    public class ReportedItemViewModel
    {
        public int TotalCount { get; set; }
        public string StatusName { get; set; }
        public string ReportedItemContent { get; set; }

    }
}

