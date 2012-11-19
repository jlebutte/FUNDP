using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Schedule.Models;

namespace Schedule
{
    [ServiceContract]
    public interface ISchedule
    {
        [OperationContract]
        [WebGet(UriTemplate = "GetImage?section={section}&date={date}&width={width}&height={height}")]
        Stream GetImage(int section, string date, int width, int height);

        [OperationContract]
        [WebGet(UriTemplate = "GetClassesWeek?section={section}&date={date}", ResponseFormat = WebMessageFormat.Json)]
        List<Cours> GetClassesWeek(int section, string date);

        [OperationContract]
        [WebGet(UriTemplate = "GetClassesDay?section={section}&date={date}", ResponseFormat = WebMessageFormat.Json)]
        List<Cours> GetClassesDay(int section, string date);

        [OperationContract]
        [WebGet(UriTemplate = "GetSections", ResponseFormat = WebMessageFormat.Json)]
        List<Section> GetSections();
    }
}
