using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace GP_RestService
{
    [ServiceContract]
    public interface IGP_service
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/AggData/{username}/{param}/")]
        string GetAggData(string username, string param);
    }
}
