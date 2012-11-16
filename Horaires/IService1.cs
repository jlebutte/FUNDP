using Horaires.Models;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Horaires
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetImage?section={section}&date={date}&width={width}&height={height}")]
        Stream GetImage(int section, string date, int width, int height);

        [OperationContract]
        [WebGet(UriTemplate = "GetCours?section={section}&date={date}", ResponseFormat = WebMessageFormat.Json)]
        List<Cours> GetCours(int section, string date);

        [OperationContract]
        [WebGet(UriTemplate = "GetSections", ResponseFormat = WebMessageFormat.Json)]
        List<Section> GetSection();
    }
}
