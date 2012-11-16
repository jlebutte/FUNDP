using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace WebCampus
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)] 
    public class Service1 : IService1
    {
        private CookieContainer _cookies = new CookieContainer();

        public string Login()
        {
            string strLogin = "jlebutte";
            string strPassword = "wyWemuxe";


            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = "login=" + strLogin;
            postData += "&password=" + strPassword;
            postData += "&submitAuth=" + "Entrer";
            byte[] data = encoding.GetBytes(postData);


            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("http://webcampus.fundp.ac.be/claroline/auth/login.php");
            Request.AllowAutoRedirect = true;
            Request.CookieContainer = this._cookies;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            Stream newStream = Request.GetRequestStream();
            // Send the data.
            newStream.Write(data, 0, data.Length);
            newStream.Close();


            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            StreamReader ResponseDataStream = new StreamReader(Response.GetResponseStream());
            string str = ResponseDataStream.ReadToEnd();

            MySessionState state = (MySessionState)HttpContext.Current.Session["MySessionState"];

            if (state == null)
            {

                state = new MySessionState();

                HttpContext.Current.Session["MySessionState"] = state;

            }

            return str;
        }

        public string TestSession(string username, string password)
        {
            MySessionState state = (MySessionState)HttpContext.Current.Session["MySessionState"];
            if (state == null)
            {
                state = new MySessionState();
                state.Username = username;
                state.Password = password;
                HttpContext.Current.Session["MySessionState"] = state;
            }

            return "success";
        }

        public string getSession()
        {
            MySessionState state = (MySessionState)HttpContext.Current.Session["MySessionState"];
            return state.Username;
        }
    }
}
