using GP_Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GP_Dal.Dal;

namespace GP_RestService
{
    public class GP_service : IGP_service
    {
        public AggData GetAggData(string username, string param)
        {
            AggMode mode = AggMode.all;
            if (param == "min")
                mode = AggMode.min;
            else if (param == "max")
                mode = AggMode.max;
            else if (param != "all")
                throw new ArgumentException("Invalid param value.");

            return new GP_Dal.Dal().GetAggregateData(mode, username);
        }
    }
}
