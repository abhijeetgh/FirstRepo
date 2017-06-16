using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts.Dtos;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.Risk.Services.Test.Helpers
{
     public static class DtoBuilder
    {
         public static LocationDto GetLocationForInsert()
         {
             return Builder<LocationDto>.CreateNew().Do(x=>x.Id=0).Build();
         }

         public static ClaimDto GetCliamForInsert()
         {
             return Builder<ClaimDto>.CreateNew()
                                     .Do(x => x.OpenLocaton = GetList<LocationDto>())
                                     .Do(x => x.CloseLocation = GetList<LocationDto>())
                                     .Do(x => x.LossTypes = GetList<LossTypesDto>())
                                     .Do(x => x.Status = GetList<ClaimStatusDto>())
                                     .Do(x => x.Users = GetList<UserDto>())
                                     .Build();
         }

         public static DriverInfoDto GetDriverForInsert()
         {
             return Builder<DriverInfoDto>.CreateNew().Do(x => x.Id = 0).Build();
         }


        public static List<TEntity> GetList<TEntity>() where TEntity : class
        {
            return Builder<TEntity>.CreateListOfSize(5).All().Build().ToList();
        }

        public static TEntity Get<TEntity>() where TEntity : class
        {
            return Builder<TEntity>.CreateNew().Build();
        }


        internal static NotesSearchCriteria GetNotesSearchCriteria()
        {
            return new NotesSearchCriteria {
            
                PageCount=1,
                PageSize=5,
                SortOrder=true,
                SortType = "Date"
            
            };
        }
    }
}
