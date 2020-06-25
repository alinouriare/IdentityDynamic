using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIdentity.ViewModels.Role;

namespace WebIdentity.Repositories
{
   public interface IUtilities
    {

         IList<ActionAndControllerName> AreaAndActionAndControllerNamesList();
         IList<string> GetAllAreasNames();
         string DataBaseRoleValidationGuid();
    }
}
