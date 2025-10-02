using System.Reflection.Metadata.Ecma335;
using ItirafEt.Api.BackgorunServices.RabbitMQ;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HubServices;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class UserRoleService
    {
        private readonly dbContext _context;

        public UserRoleService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<ChangeUserRoleViewModel>> SelecetUserAsync(ChangeUserRoleViewModel model)
        {
            model.UserNameOrUserId = model.UserNameOrUserId.Trim();


            Guid userId;
            if (Guid.TryParse(model.UserNameOrUserId, out userId))
            {
                var isUserExist = await _context.Users.AnyAsync(u => u.Id == userId);

                if (!isUserExist)
                    return ApiResponses<ChangeUserRoleViewModel>.Fail("Kullanıcı bulunamadı.");
            }
            else
            {
                Guid? _userId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.UserName == model.UserNameOrUserId)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                if (_userId == null)
                    return ApiResponses<ChangeUserRoleViewModel>.Fail("Kullanıcı bulunamadı.");
                userId = _userId.Value;

            }

            var result = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new ChangeUserRoleViewModel
                {
                    UserNameOrUserId = model.UserNameOrUserId,
                    SelectedRoleName = u.Roles
                        .Where(ur => ur.RevokedDate == null)
                        .OrderByDescending(ur => ur.AssignedDate)
                        .Select(ur => ur.Role.Name)
                        .FirstOrDefault(),
                    RolesNames = _context.Roles
                        .Select(rt => rt.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return ApiResponses<ChangeUserRoleViewModel>.Fail("Kullanıcı bulunamadı.");

            return ApiResponses<ChangeUserRoleViewModel>.Success(result);


        }
        public async Task<ApiResponses> ChangeUserRoleAsync(ChangeUserRoleViewModel model)
        {

            model.UserNameOrUserId = model.UserNameOrUserId.Trim();


            Guid userId;
            if (Guid.TryParse(model.UserNameOrUserId, out userId))
            {
                var isUserExist = await _context.Users.AnyAsync(u => u.Id == userId);

                if (!isUserExist)
                    return ApiResponses.Fail("Kullanıcı bulunamadı.");
            }
            else
            {
                Guid? _userId = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.UserName == model.UserNameOrUserId)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                if (_userId == null)
                    return ApiResponses.Fail("Kullanıcı bulunamadı.");
                userId = _userId.Value;

            }

            var isRoleExist = await _context.Roles.AnyAsync(r => r.Name == model.SelectedRoleName);
            if (!isRoleExist)
                return ApiResponses.Fail("Rol bulunamadı.");

            var isAdminValid = await _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.Id == model.AssignedByUserId)
                .AnyAsync(u => u.Roles.Any(r => r.Role.Name == RoleType.SuperAdmin.Name && r.RevokedDate == null));

            if (!isAdminValid)
                return ApiResponses.Fail("Yönetici yetkiniz yok.");


            var userRole = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.RevokedDate == null)
                .FirstOrDefaultAsync();

            userRole.RevokedDate = DateTime.UtcNow;
            _context.Update(userRole);
            await _context.SaveChangesAsync();



            var newRole = new UserRoles
            {
                UserId = userId,
                RoleName = model.SelectedRoleName,
                AssignedByUserId = model.AssignedByUserId,
                AssignedDate = DateTime.UtcNow,
                ExpireDate = model.ExpireDate
            };

            await _context.UserRoles.AddAsync(newRole);
            await _context.SaveChangesAsync();

            return ApiResponses.Success();




        }

        public async Task<ApiResponses<SelectOptionForUserWithRoleViewModel>> GetAllUsersWithRolesAsync(SelectOptionForUserWithRoleViewModel model)
        {
            var roles = await GetAllRolesAsync();
            var selectedRoleName = model.SelectedRoleName;

            bool isRoleValid = selectedRoleName == "Hepsi" || roles.Data.Contains(selectedRoleName);
            if (!isRoleValid)
                return ApiResponses<SelectOptionForUserWithRoleViewModel>.Fail("Seçilen rol yanlış");

            Guid? userId = null;
            if (!string.IsNullOrEmpty(model.SearchedUserName))
            {
                userId = await GetUserIdAsync(model.SearchedUserName);
                if (userId == null)
                    return ApiResponses<SelectOptionForUserWithRoleViewModel>.Fail("Kullanıcı bilgisi yanlış girildi");
            }

            var query = _context.UserRoles
                .Include(r => r.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (selectedRoleName != "Hepsi")
                query = query.Where(x => x.RoleName == selectedRoleName);
            try
            {
                if (!model.ShowAll)
                    query = query.Where(x => x.RevokedDate == null);



                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(x => x.AssignedDate)
                    .Skip((model.CurrentPage - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .Select(x => new UsersWithRolesViewModel
                    {
                        UserName = x.User.UserName,
                        RoleName = x.RoleName,
                        AssignedDate = x.AssignedDate,
                        ExpireDate = x.ExpireDate,
                        RevokedDate = x.RevokedDate,
                        AssignedByUserName = x.AssignedByUser.UserName
                    })
                    .ToListAsync();

                model.Items = items;
                model.TotalCount = totalCount;

                return ApiResponses<SelectOptionForUserWithRoleViewModel>.Success(model);
            }
            catch (Exception ex)
            {
                return ApiResponses<SelectOptionForUserWithRoleViewModel>.Fail(ex.Message);
            }

        }


        public async Task<ApiResponses<List<string>>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .Select(r => r.Name)
                .ToListAsync();
            return ApiResponses<List<string>>.Success(roles);
        }

        private async Task<Guid?> GetUserIdAsync(string userName)
        {

            return await _context.Users
                .Where(u => u.UserName.ToLower() == userName.ToLower().Trim())
                .Select(u => (Guid?)u.Id)
                .FirstOrDefaultAsync();
        }
    }
}
