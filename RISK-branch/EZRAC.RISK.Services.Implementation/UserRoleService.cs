using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util;
using EZRAC.RISK.Util.Common;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EZRAC.RISK.Services.Implementation
{
    public class UserRoleService : IUserRoleService
    {
        IGenericRepository<UserRole> _userRoleRepository = null;
        IGenericRepository<Permission> _permissionsRepository = null;
        IGenericRepository<RolePermission> _rolePermissionsRepository = null;
        IGenericRepository<RiskCategory> _riskCategoryRepository = null;

        public UserRoleService(IGenericRepository<UserRole> userRoleRepository, IGenericRepository<Permission> permissionsRepository, IGenericRepository<RolePermission> rolePermissionsRepository, IGenericRepository<RiskCategory> riskCategoryRepository)
        {
            _userRoleRepository = userRoleRepository;
            _permissionsRepository = permissionsRepository;
            _rolePermissionsRepository = rolePermissionsRepository;
            _riskCategoryRepository = riskCategoryRepository;
        }
        public async Task<UserRoleDto> GetRolePermissionByIdAsync(long RoleId)
        {
            UserRoleDto userRoleDto = new UserRoleDto();
            var predicate = PredicateBuilder.True<UserRole>();
            predicate = predicate.And(x => x.IsDeleted == false && x.Id == RoleId);
            userRoleDto = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(predicate).ToList().Select(x => new UserRoleDto
                {
                    Id = RoleId,
                    Name = x.Name,
                    PermissionIds = string.Join(",", (x.Permissions.ToList().Select(y => y.Id).ToArray()))
                }).SingleOrDefault<UserRoleDto>();


            return userRoleDto;
        }
        public async Task<IEnumerable<UserRoleDto>> GetAllUserRoleAsync()
        {
            IEnumerable<UserRoleDto> userRoleList = null;

            userRoleList = _userRoleRepository.AsQueryable.Where(x => x.IsDeleted == false).ToList().Select(x => new UserRoleDto
            {
                Id = x.Id,
                Name = x.Name
            });

            return userRoleList;
        }
        public async Task<List<RiskCategoryDto>> GetAllPermissionAsync()
        {
            //IEnumerable<PermissionDto> permissionDtoList = null;
            //permissionList = _permissionsRepository.AsQueryable.ToList().Select(x => new PermissionDto
            //{
            //    PermissionId = x.Id,
            //    PermissionName = x.Name
            //});

            IEnumerable<Permission> permissionList = null;
            permissionList = _permissionsRepository.AsQueryable.ToList();
            List<RiskCategoryDto> riskCategoryDto = new List<RiskCategoryDto>();
            List<RiskCategory> riskCategoryList = await _riskCategoryRepository.AsQueryable.ToListAsync();
            foreach (var riskCategoryItem in riskCategoryList)
            {
                RiskCategoryDto riskCategory = new RiskCategoryDto();
                riskCategory.CategoryID = riskCategoryItem.Id;
                riskCategory.CategoryName = riskCategoryItem.CategoryName;
                riskCategory.ViewPermission = getPermissionList(riskCategoryItem.Id, Convert.ToInt32(RoleViewTypes.View), permissionList);
                riskCategory.CreatePermission = getPermissionList(riskCategoryItem.Id, Convert.ToInt32(RoleViewTypes.Create), permissionList);
                riskCategory.EditPermission = getPermissionList(riskCategoryItem.Id, Convert.ToInt32(RoleViewTypes.Edit), permissionList);
                riskCategory.DeletePermission = getPermissionList(riskCategoryItem.Id, Convert.ToInt32(RoleViewTypes.Delete), permissionList);
                riskCategory.OtherPermission = getPermissionList(riskCategoryItem.Id, Convert.ToInt32(RoleViewTypes.Other), permissionList);
                riskCategoryDto.Add(riskCategory);
            }


            return riskCategoryDto;
        }
        public async Task<UserRoleDto> UpdateUserRolePermissionAsync(string AddPermission, string DeletePermission, long RoleId, string RoleName)
        {
            //bool flag = false;
            var predicate = PredicateBuilder.True<UserRole>();
            predicate = predicate.And(x => x.IsDeleted == false && x.Id == RoleId);
            var userRole = await _userRoleRepository.AsQueryable.IncludeMultiple(X => X.Permissions).Where(predicate).FirstOrDefaultAsync();
            //userRole.Name = RoleName;
            if (!string.IsNullOrEmpty(AddPermission))
            {
                foreach (var item in AddPermission.Split(','))
                {
                    var permission = await _permissionsRepository.GetByIdAsync(Convert.ToInt64(item));
                    userRole.Permissions.Add(permission);
                }
                await _userRoleRepository.UpdateAsync(userRole);
            }
            if (!string.IsNullOrEmpty(DeletePermission))
            {
                foreach (var item in DeletePermission.Split(','))
                {
                    var permission = await _permissionsRepository.GetByIdAsync(Convert.ToInt64(item));
                    userRole.Permissions.Remove(permission);
                }
                await _userRoleRepository.UpdateAsync(userRole);
            }

            //if (string.IsNullOrEmpty(AddPermission) && string.IsNullOrEmpty(DeletePermission))
            //{
            //    await _userRoleRepository.UpdateAsync(userRole);
            //}
            UserRoleDto userRoleDto = new UserRoleDto();
            userRoleDto = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(predicate).ToList().Select(x => new UserRoleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PermissionIds = string.Join(",", (x.Permissions.ToList().Select(y => y.Id).ToArray()))
                }).SingleOrDefault<UserRoleDto>();

            return userRoleDto;
        }

        public async Task<UserRoleDto> CreateUserRolePermissionAsync(string AddPermission, string RoleName)
        {
            UserRole userRole = new UserRole();
            userRole.Name = RoleName;
            userRole.IsDeleted = false;

            await _userRoleRepository.InsertAsync(userRole);
            long UserRoleID = userRole.Id;

            UserRoleDto useRoleDto = new UserRoleDto();
            var predicate = PredicateBuilder.True<UserRole>();
            predicate = predicate.And(x => x.IsDeleted == false && x.Id == UserRoleID);

            if (!string.IsNullOrEmpty(AddPermission))
            {
                var userRoleUpdate = await _userRoleRepository.AsQueryable.IncludeMultiple(X => X.Permissions).Where(predicate).FirstOrDefaultAsync();

                foreach (var item in AddPermission.Split(','))
                {
                    var permission = await _permissionsRepository.GetByIdAsync(Convert.ToInt64(item));
                    userRoleUpdate.Permissions.Add(permission);
                }
                await _userRoleRepository.UpdateAsync(userRole);
            }

            useRoleDto = _userRoleRepository.AsQueryable.IncludeMultiple(x => x.Permissions).Where(predicate).ToList().Select(x => new UserRoleDto
            {
                Id = x.Id,
                Name = x.Name,
                PermissionIds = string.Join(",", (x.Permissions.ToList().Select(y => y.Id).ToArray()))
            }).SingleOrDefault<UserRoleDto>();
            return useRoleDto;
        }

        #region PrivateMethod
        private IEnumerable<PermissionDto> getPermissionList(long categoryId, long permissionlevelId, IEnumerable<Permission> permissionList)
        {
            IEnumerable<PermissionDto> permissionDto = null;
            permissionDto = permissionList.Where(obj => obj.CategoryId == categoryId && obj.PermissionLevelId == permissionlevelId).ToList().Select(x => new PermissionDto
            {
                PermissionId = x.Id,
                PermissionName = x.Name,
                PermissionLevelId = x.PermissionLevelId,
                CategoryId = x.CategoryId
            });
            return permissionDto;
        }
        #endregion

    }

}
