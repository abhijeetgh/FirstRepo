using EZRAC.Risk.UI.Web.Helper;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Implementation;
using System;
using System.ComponentModel.DataAnnotations;

namespace EZRAC.Risk.UI.Web
{
    public sealed class UnitNumberValidatorAttribute: ValidationAttribute
    {
         #region  Private Members
        
       
        private ITSDService _tsdService;

        #endregion

        /// <summary>
        /// public Claims controller Constructor
        /// </summary>
        #region Constructors
        public UnitNumberValidatorAttribute() 
        {

            _tsdService = UnityResolver.ResolveService<ITSDService>();
          
        }
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (_tsdService.IsUnitNumberValid(value.ToString()))
                    return ValidationResult.Success;
                else
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                
            }

            return ValidationResult.Success;   
        }
    }
}