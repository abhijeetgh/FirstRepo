using EZRAC.Core.Data.EntityFramework;
using EZRAC.RISK.Domain;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZRAC.RISK.Services.Implementation
{
    public class PoliceAgencyService : IPoliceAgencyService
    {
        IGenericRepository<RiskPoliceAgency> _riskPoliceAgencyRepository = null;
        IGenericRepository<RiskIncident> _incidentRepository = null;

        public PoliceAgencyService(IGenericRepository<RiskPoliceAgency> riskPoliceAgencyRepository, IGenericRepository<RiskIncident> incidentRepository)
        {
            _riskPoliceAgencyRepository = riskPoliceAgencyRepository;
            _incidentRepository = incidentRepository;
        }


        public async Task<PoliceAgencyDto> GetPoliceAgencyDetailById(long id)
        {
            var riskPoliceAgency = await _riskPoliceAgencyRepository.GetByIdAsync(id);
            var agencyDto = MapPoliceAgencyDto(riskPoliceAgency);
            return agencyDto;
        }

        public async Task<bool> AddOrUpdatePoliceAgency(PoliceAgencyDto agencyDto, long userId)
        {
            var success = false;

            var isDuplicate = IsPoliceAgencyDuplicate(agencyDto.Id, agencyDto.AgencyName);
            if (isDuplicate)
            {
                return false;
            }

            if (agencyDto.Id != 0)
            {
                var policeAgency = await _riskPoliceAgencyRepository.GetByIdAsync(agencyDto.Id);
                if (policeAgency != null)
                {
                    policeAgency.Id = agencyDto.Id;
                    policeAgency.AgencyName = agencyDto.AgencyName;
                    policeAgency.Address = agencyDto.Address;
                    policeAgency.City = agencyDto.City;
                    policeAgency.Contact = agencyDto.Contact;
                    policeAgency.Email = agencyDto.Email;
                    policeAgency.Fax = agencyDto.Fax;
                    policeAgency.Phone = agencyDto.Phone;
                    policeAgency.State = agencyDto.State;
                    policeAgency.ZIP = agencyDto.ZIP;
                    policeAgency.UpdatedBy = userId;
                    policeAgency.UpdatedDateTime = DateTime.Now;
                    await _riskPoliceAgencyRepository.UpdateAsync(policeAgency);
                    success = true;
                }
            }
            else
            {
                 await _riskPoliceAgencyRepository.InsertAsync(new RiskPoliceAgency
                    {
                        Address = agencyDto.Address,
                        City = agencyDto.City,
                        AgencyName = agencyDto.AgencyName,
                        Contact = agencyDto.Contact,
                        Email = agencyDto.Email,
                        Fax = agencyDto.Fax,
                        Phone = agencyDto.Phone,
                        State = agencyDto.State,
                        ZIP = agencyDto.ZIP,
                        UpdatedBy = userId,
                        UpdatedDateTime = DateTime.Now,
                        CreatedDateTime = DateTime.Now,
                    });
                    success = true;
            }
            return success;
        }

        public async Task<bool> DeletePoliceAgency(PoliceAgencyDto agencyDto, long userId)
        {
            var success = false;
            var isDuplicate = IsPoliceAgencyAlreadyUsed(agencyDto.Id);
            if (isDuplicate)
            {
                return false;
            }
            var agencyToDelete = await _riskPoliceAgencyRepository.GetByIdAsync(agencyDto.Id);
            if (agencyToDelete != null)
            {
                agencyToDelete.IsDeleted = true;
                agencyToDelete.UpdatedBy = userId;
                agencyToDelete.UpdatedDateTime = DateTime.Now;
                await _riskPoliceAgencyRepository.UpdateAsync(agencyToDelete);
                success = true;
            }
            return success;
        }

        public bool IsPoliceAgencyAlreadyUsed(long id)
        {
            var isDuplicate = _incidentRepository.AsQueryable.Where(x => x.PoliceAgencyId == id).Any();
            return isDuplicate;
        }

        #region Private Methods
        private bool IsPoliceAgencyDuplicate(long id, string name)
        {
            var exist = _riskPoliceAgencyRepository.AsQueryable.Where(x => !x.IsDeleted && x.Id != id && x.AgencyName == name).Any();
            return exist;
        }

        private PoliceAgencyDto MapPoliceAgencyDto(RiskPoliceAgency riskPoliceAgency)
        {
            var agencyDto = new PoliceAgencyDto();
            agencyDto.Id = riskPoliceAgency.Id;
            agencyDto.AgencyName = riskPoliceAgency.AgencyName;
            agencyDto.Address = riskPoliceAgency.Address;
            agencyDto.City = riskPoliceAgency.City;
            agencyDto.Contact = riskPoliceAgency.Contact;
            agencyDto.Email = riskPoliceAgency.Email;
            agencyDto.Fax = riskPoliceAgency.Fax;
            agencyDto.Phone = riskPoliceAgency.Phone;
            agencyDto.State = riskPoliceAgency.State;
            agencyDto.ZIP = riskPoliceAgency.ZIP;
            return agencyDto;
        } 
        #endregion

    }
}
