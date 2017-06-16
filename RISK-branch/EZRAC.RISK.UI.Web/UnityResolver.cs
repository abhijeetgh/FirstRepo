using EZRAC.Risk.UI.Web.App_Start;
using Microsoft.Practices.Unity;

namespace EZRAC.Risk.UI.Web.Helper
{
    /// <summary>
    /// This class is used to resolve dependencies using Unity.
    /// </summary>
    public class UnityResolver
    {
        /// <summary>
        /// Resolves the service.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <returns>
        /// Service as per passed type.
        /// </returns>
        public static T ResolveService<T>()
        {
            var resolveService = UnityConfig.GetConfiguredContainer().Resolve<T>();
            return resolveService;
        }
    }
}
