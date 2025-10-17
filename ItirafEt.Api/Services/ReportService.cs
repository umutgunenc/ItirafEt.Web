using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.Services.ClientServices;
using ItirafEt.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Exceptions;

namespace ItirafEt.Api.Services
{
    public class ReportService
    {
        private readonly dbContext _context;

        public ReportService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<CreateReportTypeViewModel>> CreateReportTypeAsync(CreateReportTypeViewModel model)
        {

            if (string.IsNullOrEmpty(model.Name.Trim()))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet adı boş olamaz.");

            if (model.Name.Trim().Length > 64)
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet adı maksimum 64 karakter uzunluğunda olabilir.");

            if (model.IconUrl?.Trim().Length > 128)
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet icon URLi maksimum 128 karakter uzunluğunda olabilir.");

            if (await _context.ReportTypes.AsNoTracking().AnyAsync(rt => rt.Name.ToLower() == model.Name.Trim().ToLower() && rt.ReportClass == model.ReportClass))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Bu isimde zaten bir şikayet türü mevcut.");

            if (!Enum.IsDefined(typeof(ReportClassEnum), model.ReportClass))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Geçersiz şikayet türü.");

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

            return ApiResponses<CreateReportTypeViewModel>.Success(model);


        }

        public async Task<ApiResponses<CreateReportTypeViewModel>> EditReportTypeAsync(CreateReportTypeViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name.Trim()))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet adı boş olamaz.");
            if (model.Name.Trim().Length > 64)
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet adı maksimum 64 karakter uzunluğunda olabilir.");
            if (model.IconUrl?.Trim().Length > 128)
                return ApiResponses<CreateReportTypeViewModel>.Fail("Şikayet icon URLi maksimum 128 karakter uzunluğunda olabilir.");

            if (!Enum.IsDefined(typeof(ReportClassEnum), model.ReportClass))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Geçersiz şikayet türü.");

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(rt => rt.Id == model.Id);

            if (reportType == null)
                return ApiResponses<CreateReportTypeViewModel>.Fail("Böyle bir şikayet türü bulunamadı.");

            if (await _context.ReportTypes.AsNoTracking().AnyAsync(rt => rt.Id != model.Id && rt.Name.ToLower() == model.Name.Trim().ToLower() && rt.ReportClass == model.ReportClass))
                return ApiResponses<CreateReportTypeViewModel>.Fail("Bu isimde zaten bir şikayet türü mevcut.");

            reportType.IsActive = model.IsActive;
            reportType.IconUrl = model.IconUrl?.Trim();
            reportType.Name = model.Name.Trim();
            reportType.ReportClass = (ReportClassEnum)model.ReportClass;

            _context.ReportTypes.Update(reportType);
            await _context.SaveChangesAsync();

            return ApiResponses<CreateReportTypeViewModel>.Success(model);
        }

        public async Task<ApiResponses<List<CreateReportTypeViewModel>>> GetAllReportTypesAsync()
        {
            var reportTypes = await _context.ReportTypes
                .AsNoTracking()
                .Select(rt => new CreateReportTypeViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    IsActive = rt.IsActive,
                    IconUrl = rt.IconUrl,
                    ReportClass = rt.ReportClass,
                })
                .ToListAsync();
            return ApiResponses<List<CreateReportTypeViewModel>>.Success(reportTypes);
        }

        public async Task<ApiResponses<List<CreateReportTypeViewModel>>> GetAllActiveReportTypesAsync(ReportClassEnum reportClassEnum)
        {
            var reportTypes = await _context.ReportTypes
                .AsNoTracking()
                .Where(rt => rt.IsActive && rt.ReportClass == reportClassEnum)
                .Select(rt => new CreateReportTypeViewModel
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    IsActive = rt.IsActive,
                    IconUrl = rt.IconUrl,
                    ReportClass = rt.ReportClass,
                })
                .ToListAsync();

            return ApiResponses<List<CreateReportTypeViewModel>>.Success(reportTypes);
        }

        public async Task<ApiResponses> SendReportAsync(SendReportViewModel model)
        {
            if (!Enum.IsDefined(typeof(ReportClassEnum), model.ReportClass))
                return ApiResponses.Fail("Geçersiz şikayet türü.");

            if (model.ReportClass ==(int)ReportClassEnum.Post && model.PostId == null && model.ReportedUserId == null)
                return ApiResponses.Fail("Şikayet talebiniz geçersiz.");

            if (model.ReportClass == (int)ReportClassEnum.Comment && model.ComemntId == null && model.PostId == null && model.ReportedUserId == null)
                return ApiResponses.Fail("Şikayet talebiniz geçersiz.");

            if (model.ReportClass == (int)ReportClassEnum.User && model.ReportedUserId == null)
                return ApiResponses.Fail("Şikayet talebiniz geçersiz.");

            if (model.ReportExplanation?.Trim().Length > 1024)
                return ApiResponses.Fail("Şikayet açıklaması maksimum 1024 karakter uzunluğunda olabilir.");

            if(model.ReportTypeId is null)
                return ApiResponses.Fail("Lütfen şikayet türü seçiniz.");

            if(!await _context.ReportTypes.AsNoTracking().AnyAsync(rt => rt.Id == model.ReportTypeId))
                return ApiResponses.Fail("Lütfen geçerli bir şikayet türü seçiniz.");


            var isThereReportingUser = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == model.ReportingUserId);
            if (!isThereReportingUser)
                return ApiResponses.Fail("Şikayet talebiniz geçersiz.");




            if (model.ReportClass == (int)ReportClassEnum.Post)
            {
                var isTherePost = await _context.Posts
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == model.PostId && !p.IsDeletedByUser && !p.IsDeletedByAdmin);

                if (!isTherePost)
                    return ApiResponses.Fail("Şikayet edilen gönderi bulunamadı. Gönderi silinmiş olabilir.");

                var allReadyReported = await _context.Reports
                    .AsNoTracking()
                    .AnyAsync(r => r.PostId == model.PostId && r.ReportingUserId == model.ReportingUserId && r.Status == ReportStatusEnum.Pending);
                if (allReadyReported)
                    return ApiResponses.Fail("Bu gönderi için zaten bir şikayetiniz bulunmakta. Şikayetiniz incelenene kadar yeni bir şikayet oluşturamazsınız.");


            }

            if (model.ReportClass == (int)ReportClassEnum.Comment)
            {
                var isThereComment = await _context.Comments
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == model.ComemntId && c.IsActive);
                if (!isThereComment)
                    return ApiResponses.Fail("Şikayet edilen yorum bulunamadı. Sikayet ettiğiniz yorum silinmiş olabilir.");

                var allReadyReported = await _context.Reports
                    .AsNoTracking()
                    .AnyAsync(r => r.ComemntId == model.ComemntId && r.ReportingUserId == model.ReportingUserId && r.Status == ReportStatusEnum.Pending);

                if (allReadyReported)
                    return ApiResponses.Fail("Bu yorum için zaten bir şikayetiniz bulunmakta. Şikayetiniz incelenene kadar yeni bir şikayet oluşturamazsınız.");

            }

            if (model.ReportClass == (int)ReportClassEnum.User)
            {
                var isThereReportedUser = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == model.ReportedUserId && !u.IsBanned && !u.IsDeleted);
                if (!isThereReportedUser)
                    return ApiResponses.Fail("Şikayet edilen kullanıcı bulunamadı. Kullanıcı banlanmış veya silinmiş olabliir.");

                var allReadyReported = await _context.Reports
                    .AsNoTracking()
                    .AnyAsync(r => r.ReportedUserId == model.ReportedUserId && r.ReportingUserId == model.ReportingUserId && r.Status == ReportStatusEnum.Pending);

                if (allReadyReported)
                    return ApiResponses.Fail("Bu kullanıcı için zaten bir şikayetiniz bulunmakta. Şikayetiniz incelenene kadar yeni bir şikayet oluşturamazsınız.");
            }

            var report = CreateReport(model);
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            return ApiResponses.Success();

        }

        public async Task<ApiResponses<List<ReportListViewModel>>> GetReportedItemsAsync( ReportStatusEnum status , ReportClassEnum? reportClass = null)
        {
            var query = _context.Reports
                .AsNoTracking()
                .AsQueryable();

            if(status != ReportStatusEnum.All)
                query = query.Where(r => r.Status == status);

            if (reportClass.HasValue)
                query = query.Where(r => r.ReportType.ReportClass == reportClass);

            var groupedReports = await query
                .GroupBy(r => new
                {
                    r.ReportType.ReportClass,
                    r.PostId,
                    r.ComemntId,
                    r.ReportedUserId,
                    r.Status
                })
                .Select(g => new ReportListViewModel
                {
                    ReportClass = g.Key.ReportClass,
                    PostId = g.Key.PostId,
                    CommentId = g.Key.ComemntId,
                    ReportedUserId = g.Key.ReportedUserId,
                    ReportedUserName = _context.Users.AsNoTracking().Where(u => u.Id == g.Key.ReportedUserId).Select(u => u.UserName).FirstOrDefault(),
                    Status = g.Key.Status,
                    ReportCount = g.Count()
                })
                .OrderByDescending(r => r.ReportCount)
                .ToListAsync();

            return ApiResponses<List<ReportListViewModel>>.Success(groupedReports);
        }
        private Report CreateReport(SendReportViewModel model)
        {
            return new Report
            {
                ReportingUserId = model.ReportingUserId,
                PostId = model.PostId,
                ComemntId = model.ComemntId,
                ReportedUserId = model.ReportedUserId,
                ReportTypeId = (int)model.ReportTypeId,
                ReportExplanation = model.ReportExplanation?.Trim(),
                CreatedDate = DateTime.UtcNow,
                Status = ReportStatusEnum.Pending
            };
        }

    }

}

