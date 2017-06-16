using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Domain.Entities;
using RateShopper.Services.Data;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public interface ICarClassService : IBaseService<CarClass>
    {
        //Define structure of method if not present in the main class
        long GetCarClassIDByName(string carClassName);
        List<CarClassDTO> GetAllCarClasses();
        CarClassDTO GetCarClassDetails(long carClassID);
        long SaveCarClass(CarClassDTO objCarClassDTO);
        void SaveCarClassLogo(long carClassID, string logoPath);
        bool DeleteCarClass(long carClassID, long userID);
        string CarClassOrderCount(int displayOrder, string carClassCode);
        Dictionary<string, long> GetCarClassDictionary();
    }
}
