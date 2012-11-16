using System.ServiceModel;
using System.ServiceModel.Web;

namespace WebCampus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet()]
        string Login();

        [OperationContract]
        [WebGet()]
        string TestSession(string username, string password);

        [OperationContract]
        [WebGet()]
        string getSession();
    }
}
