using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateShopper.Services.Data;
using RateShopper.Domain.Entities;
using RateShopper.Data;
using System.Data.Entity;
using RateShopper.Core.Cache;
using RateShopper.Domain.DTOs;

namespace RateShopper.Services.Data
{
    public class CarClassService : BaseService<CarClass>, ICarClassService
    {
        public CarClassService(IEZRACRateShopperContext context, ICacheManager cacheManager)
            : base(context, cacheManager)
        {
            _context = context;
            _dbset = _context.Set<CarClass>();
            _cacheManager = cacheManager;
        }


        public long GetCarClassIDByName(string carClassName)
        {
            CarClass carClass = base.GetAll(true).Where(obj1 => !obj1.IsDeleted).FirstOrDefault(obj => obj.Code.Equals(carClassName, StringComparison.InvariantCultureIgnoreCase));
            if (carClass == null || carClass.ID <= 0)
            {
                carClass = base.GetAll(true).Where(obj1 => !obj1.IsDeleted).FirstOrDefault();
            }

            return carClass.ID;
        }

        public List<CarClassDTO> GetAllCarClasses()
        {
            return GetAll(false).Where(d => !d.IsDeleted).Select(d => new CarClassDTO { ID = d.ID, Code = d.Code, Description = d.Description, CarClassOrder = d.DisplayOrder }).OrderBy(d => d.CarClassOrder).ToList();
        }

        public CarClassDTO GetCarClassDetails(long carClassID)
        {
            CarClass carClassEntity = GetById(carClassID, false);
            if (carClassEntity != null)
            {
                return new CarClassDTO()
                {
                    ID = carClassEntity.ID,
                    Code = carClassEntity.Code,
                    Description = carClassEntity.Description,
                    Logo = carClassEntity.Logo,
                    CarClassOrder = carClassEntity.DisplayOrder
                };
            }
            return new CarClassDTO();
        }

        public long SaveCarClass(CarClassDTO objCarClassDTO)
        {
            bool isMoveUp = false;
            if (objCarClassDTO != null)
            {
                if (objCarClassDTO.ID == 0)
                {
                    CarClass objExistingCarClass = GetAll().Where(obj => obj.Code == objCarClassDTO.Code && !obj.IsDeleted).FirstOrDefault();
                    if (objExistingCarClass == null)
                    {
                        CarClass objCarClassEntity = new CarClass()
                        {
                            Code = objCarClassDTO.Code,
                            Description = objCarClassDTO.Description,
                            IsDeleted = false,
                            Logo = objCarClassDTO.Logo,
                            UpdatedBy = objCarClassDTO.CreatedBy,
                            CreatedBy = objCarClassDTO.CreatedBy,
                            UpdatedDateTime = DateTime.Now,
                            CreatedDateTime = DateTime.Now,
                            DisplayOrder = objCarClassDTO.CarClassOrder
                        };
                        CarClass lastCarClass = _context.CarClasses.Where(car => !car.IsDeleted && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderByDescending(car => car.DisplayOrder).FirstOrDefault();
                        //fetch car class 
                        if (lastCarClass.DisplayOrder > objCarClassDTO.CarClassOrder)
                        {
                            List<CarClass> carClassList = new List<CarClass>();
                            carClassList = _context.CarClasses.Where(car => !car.IsDeleted && car.DisplayOrder >= objCarClassDTO.CarClassOrder && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderBy(car => car.DisplayOrder).ToList();
                            if (carClassList != null && carClassList.Count > 0)
                            {

                                foreach (var car in carClassList)
                                {
                                    car.DisplayOrder += 1;
                                    car.UpdatedDateTime = DateTime.Now;
                                    car.UpdatedBy = objCarClassDTO.CreatedBy;
                                    _context.Entry(car).State = EntityState.Modified;
                                }
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            objCarClassEntity.DisplayOrder = lastCarClass.DisplayOrder + 1;
                        }
                        Add(objCarClassEntity);
                        return objCarClassEntity.ID;
                    }
                    return objCarClassDTO.ID;
                }
                else
                {
                    //Check for duplication of code
                    if (GetAll().Where(obj => obj.Code == objCarClassDTO.Code && !obj.IsDeleted && obj.ID != objCarClassDTO.ID).FirstOrDefault() == null)
                    {
                        CarClass objCarClassesEntity = GetById(objCarClassDTO.ID, false);
                        if (objCarClassesEntity != null)
                        {
                            int originalDisplayOrder = objCarClassesEntity.DisplayOrder;
                            objCarClassesEntity.Code = objCarClassDTO.Code;
                            if (!string.IsNullOrEmpty(objCarClassDTO.Logo))
                            {
                                objCarClassesEntity.Logo = objCarClassDTO.Logo;
                            }
                            objCarClassesEntity.Description = objCarClassDTO.Description;
                            objCarClassesEntity.UpdatedBy = objCarClassDTO.CreatedBy;
                            objCarClassesEntity.UpdatedDateTime = DateTime.Now;
                            objCarClassesEntity.DisplayOrder = objCarClassDTO.CarClassOrder;
                            if (originalDisplayOrder < objCarClassDTO.CarClassOrder)
                            {
                                isMoveUp = false;
                            }
                            else
                            { isMoveUp = true; }
                            //fetch car class 
                            CarClass lastCarClass = new CarClass();
                            List<CarClass> carClassList = new List<CarClass>();
                            if (!isMoveUp)
                            {
                                carClassList = _context.CarClasses.Where(car => !car.IsDeleted && car.DisplayOrder > originalDisplayOrder && car.DisplayOrder <= objCarClassDTO.CarClassOrder && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderBy(car => car.DisplayOrder).ToList();
                                int i = originalDisplayOrder;
                                if (carClassList != null && carClassList.Count > 0)
                                {

                                    foreach (var car in carClassList)
                                    {
                                        if (i == objCarClassDTO.CarClassOrder)
                                        {
                                            i = i + 1;
                                            car.DisplayOrder = i;
                                        }
                                        else
                                        {
                                            car.DisplayOrder = i;
                                            i++;
                                        }
                                        car.UpdatedDateTime = DateTime.Now;
                                        car.UpdatedBy = objCarClassDTO.CreatedBy;
                                        _context.Entry(car).State = EntityState.Modified;
                                    }
                                    _context.SaveChanges();
                                    lastCarClass = _context.CarClasses.Where(car => !car.IsDeleted && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderByDescending(car => car.DisplayOrder).FirstOrDefault();
                                }
                                else
                                {
                                    lastCarClass = _context.CarClasses.Where(car => !car.IsDeleted && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderByDescending(car => car.DisplayOrder).FirstOrDefault();
                                    objCarClassesEntity.DisplayOrder = lastCarClass.DisplayOrder + 1;
                                }
                            }
                            else
                            {
                                carClassList = _context.CarClasses.Where(car => !car.IsDeleted && car.DisplayOrder >= objCarClassDTO.CarClassOrder && car.DisplayOrder < originalDisplayOrder && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderBy(car => car.DisplayOrder).ToList();
                                if (carClassList != null && carClassList.Count > 0)
                                {
                                    int i = objCarClassDTO.CarClassOrder;
                                    foreach (var car in carClassList)
                                    {
                                        i++;
                                        car.DisplayOrder = i;
                                        car.UpdatedDateTime = DateTime.Now;
                                        car.UpdatedBy = objCarClassDTO.CreatedBy;
                                        _context.Entry(car).State = EntityState.Modified;
                                    }
                                    _context.SaveChanges();
                                }
                                lastCarClass = _context.CarClasses.Where(car => !car.IsDeleted && !car.Code.Equals(objCarClassDTO.Code, StringComparison.OrdinalIgnoreCase)).OrderByDescending(car => car.DisplayOrder).FirstOrDefault();
                            }
                            if (objCarClassesEntity.DisplayOrder - lastCarClass.DisplayOrder > 1)
                            {
                                objCarClassesEntity.DisplayOrder = lastCarClass.DisplayOrder + 1;
                            }
                            Update(objCarClassesEntity);
                        }
                        return objCarClassesEntity.ID;
                    }
                    return 0;
                }
            }
            return 0;
        }

        public void SaveCarClassLogo(long carClassID, string logoPath)
        {
            if (carClassID > 0 && !string.IsNullOrEmpty(logoPath))
            {
                CarClass objCarClassEntity = GetById(carClassID, false);
                if (objCarClassEntity != null)
                {
                    objCarClassEntity.Logo = logoPath;
                    Update(objCarClassEntity);
                }
            }
        }

        public bool DeleteCarClass(long carClassID, long userID)
        {
            if (carClassID > 0)
            {
                CarClass objCarClassEntity = GetById(carClassID, false);
                if (objCarClassEntity != null)
                {
                    objCarClassEntity.IsDeleted = true;
                    objCarClassEntity.UpdatedBy = userID;
                    objCarClassEntity.UpdatedDateTime = DateTime.Now;
                    //fetch car class 
                    List<CarClass> carClassList = new List<CarClass>();
                    carClassList = _context.CarClasses.Where(car => !car.IsDeleted && car.DisplayOrder >= objCarClassEntity.DisplayOrder && !car.Code.Equals(objCarClassEntity.Code, StringComparison.OrdinalIgnoreCase)).OrderBy(car => car.DisplayOrder).ToList();
                    if (carClassList != null && carClassList.Count > 0)
                    {
                        foreach (var car in carClassList)
                        {
                            car.DisplayOrder -= 1;
                            car.UpdatedDateTime = DateTime.Now;
                            car.UpdatedBy = objCarClassEntity.UpdatedBy;

                            _context.Entry(car).State = EntityState.Modified;
                        }
                        _context.SaveChanges();
                    }
                    Update(objCarClassEntity);
                    return true;
                }
            }
            return false;
        }

        public string CarClassOrderCount(int displayOrder, string carClassCode)
        {
            string carClassName = string.Empty;
            if (base.GetAll().Where(car => !car.IsDeleted && car.DisplayOrder == displayOrder && !car.Code.Equals(carClassCode, StringComparison.OrdinalIgnoreCase)).Count() > 0)
            {
                carClassName = base.GetAll().Where(car => !car.IsDeleted && car.DisplayOrder == displayOrder && !car.Code.Equals(carClassCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Code.ToString();
            }
            return carClassName;
        }

        public Dictionary<string, long> GetCarClassDictionary()
        {
            return GetAll(false).Where(cars => !cars.IsDeleted).Select(obj => new { carClass = obj.Code, ID = obj.ID }).ToDictionary(obj => obj.carClass, obj => obj.ID);
        }
    }
}
