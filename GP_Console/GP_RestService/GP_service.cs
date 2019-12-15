using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP_RestService
{
    public class GP_service : IGP_service
    {
        public string GetAggData(string username, string param)
        {            
            return string.Format("Entered username: {0} ; param: {1}",username,param);
        }
    }
}
