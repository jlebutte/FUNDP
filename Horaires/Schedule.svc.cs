using System.Globalization;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using Schedule.Models;

namespace Schedule
{
    public class Schedule : ISchedule
    {
        private readonly CookieContainer _cookies = new CookieContainer();

        public Stream GetImage(int section, string date, int width, int height)
        {
            try
            {
                Login(section);

                var doc = new HtmlDocument();
                doc.LoadHtml(GetImagePage(date, width, height));

                string str = string.Empty;
                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img[@src]"))
                {
                    str = img.Attributes["src"].Value;
                }

                var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", Assets.Global.MAIN_HTTPS, str));
                request.AllowAutoRedirect = true;
                request.CookieContainer = _cookies;
                var response = (HttpWebResponse)request.GetResponse();

                Stream fs = response.GetResponseStream();

                if (WebOperationContext.Current != null)
                    WebOperationContext.Current.OutgoingResponse.ContentType = "image/gif";
                return fs;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<Cours> GetClassesWeek(int section, string date)
        {
            try
            {
                var cours = new List<Cours>();

                Login(section);

                var doc = new HtmlDocument();
                doc.LoadHtml(GetImagePage(date));

                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//area[@href]"))
                {
                    int id = Convert.ToInt32(img.Attributes["href"].Value.Split(',')[3]);
                    var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/custom/modules/plannings/eventInfo.jsp?eventId={1}", Assets.Global.ADE_HTTPS, id.ToString(CultureInfo.InvariantCulture)));
                    request.AllowAutoRedirect = true;
                    request.CookieContainer = _cookies;
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseDataStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));

                    var coursHtml = new HtmlDocument();
                    coursHtml.LoadHtml(responseDataStream.ReadToEnd());

                    foreach (HtmlNode table in coursHtml.DocumentNode.SelectNodes("//table"))
                    {
                        foreach (HtmlNode row in table.SelectNodes("tr"))
                        {
                            if (ExploreTable(row, id))
                            {
                                HtmlNode selectedRow = row;
                                cours.Add(new Cours
                                              {
                                    Name = selectedRow.SelectNodes("td").ElementAt(0).InnerText,
                                    Day = selectedRow.SelectNodes("td").ElementAt(2).InnerText,
                                    Local = selectedRow.SelectNodes("td").ElementAt(8).InnerText,
                                    Start = selectedRow.SelectNodes("td").ElementAt(1).InnerText.Split('-').First() + selectedRow.SelectNodes("td").ElementAt(3).InnerText.Replace('h', ':'),
                                    Prof = selectedRow.SelectNodes("td").ElementAt(7).InnerText
                                });
                                break;
                            }
                        }
                    }
                }

                return cours;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<Cours> GetClassesDay(int section, string date)
        {
            try
            {
                DateTime dateRequest = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    dateRequest = DateTime.Parse(date);
                }
                return GetClassesWeek(section, date).Where(c => c.DayDT.ToShortDateString() == dateRequest.ToShortDateString()).ToList();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        public List<Section> GetSections()
        {
            List<Section> list = new List<Section>();
            
            Login(0);

            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/standard/gui/tree.jsp?forceLoad=false&amp;isDirect=true", Assets.Global.ADE_HTTP));
            request.AllowAutoRedirect = true;
            request.CookieContainer = _cookies;
            var response = (HttpWebResponse)request.GetResponse();

            var doc = new HtmlDocument();
            doc.LoadHtml(new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1")).ReadToEnd());

                foreach (HtmlNode linkCategory in doc.DocumentNode.SelectNodes("//span[@class='treecategory']").First().SelectNodes("a"))
                {
                    string category = linkCategory.Attributes["href"].Value.Split('\'').ElementAt(1);
                    request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/standard/gui/tree.jsp?category={1}&expand=false&forceLoad=false&reload=false&scroll=0", Assets.Global.ADE_HTTP, category));
                    request.AllowAutoRedirect = true;
                    request.CookieContainer = _cookies;
                    response = (HttpWebResponse)request.GetResponse();

                    doc.LoadHtml(new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1")).ReadToEnd());
                    
                    List<string> treeBranchParents = new List<string>();

                    foreach (HtmlNode treebranch in doc.DocumentNode.SelectNodes("//span[@class='treebranch']"))
                    {
                        foreach (HtmlNode linkBranch in treebranch.SelectNodes("a"))
                        {
                            string branchId = Regex.Replace(linkBranch.Attributes["href"].Value, "[^0-9]", string.Empty);
                            treeBranchParents.Add(branchId);
                            list.Add(new Section()
                            {
                                Title = linkBranch.InnerText,
                                Id = Convert.ToInt32(branchId),
                                ParentId = 0
                            });
                        }
                    }

                    foreach (HtmlNode treebranch in doc.DocumentNode.SelectNodes("//span[@class='treebranch']"))
                    {
                        foreach (HtmlNode linkBranch in treebranch.SelectNodes("a"))
                        {
                            string branchId = Regex.Replace(linkBranch.Attributes["href"].Value, "[^0-9]", string.Empty);   
                            request =
                                (HttpWebRequest)
                                WebRequest.Create(
                                    string.Format(
                                        "{0}/standard/gui/tree.jsp?branchId={1}&expand=true&forceLoad=true&reload=true&scroll=0",
                                        Assets.Global.ADE_HTTP,
                                        branchId));
                            request.AllowAutoRedirect = true;
                            request.CookieContainer = _cookies;
                            response = (HttpWebResponse) request.GetResponse();

                            doc.LoadHtml(new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1")).ReadToEnd());

                            foreach (HtmlNode treebranch2 in doc.DocumentNode.SelectNodes("//span[@class='treebranch']"))
                            {
                                foreach (HtmlNode linkBranch2 in treebranch2.SelectNodes("a"))
                                {
                                    string branchId2 = Regex.Replace(linkBranch2.Attributes["href"].Value, "[^0-9]", string.Empty);
                                  
                                    if(treeBranchParents.All(c => c != branchId2)&&list.All(c=>c.Id.ToString()!=branchId2))
                                    {
                                        list.Add(new Section()
                                                     {
                                                         Title = linkBranch2.InnerText,
                                                         Id = Convert.ToInt32(branchId2),
                                                         ParentId = Convert.ToInt32(branchId)
                                                     });
                                   }
                                }
                            }
                        }
                    }
                }
            return list;
        }

        private string Login(int section)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/standard/direct_planning.jsp?login=web&password=web&projectId=7&resources={1}", Assets.Global.ADE_HTTP, section.ToString()));
            request.AllowAutoRedirect = true;
            request.CookieContainer = _cookies;
            var response = (HttpWebResponse)request.GetResponse();
            var responseDataStream = new StreamReader(response.GetResponseStream());
            return responseDataStream.ReadToEnd();
        }

        private string GetImagePage(string date, int width = 0, int height = 0)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            StreamReader responseDataStream;

            if (width == 0)
                width = 1360;
            if (height == 0)
                height = 659;

            if (!string.IsNullOrWhiteSpace(date))
            {
                string[] dateArray = date.Split('-');
                double week = (new DateTime(Convert.ToInt32(dateArray[0]), Convert.ToInt32(dateArray[1]), Convert.ToInt32(dateArray[2])) - new DateTime(2012, 08, 12)).TotalDays / 7;

                request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/custom/modules/plannings/bounds.jsp?week={1}&reset=true", Assets.Global.ADE_HTTPS, Math.Ceiling(week)));
                request.AllowAutoRedirect = true;
                request.CookieContainer = this._cookies;
                response = (HttpWebResponse)request.GetResponse();
                responseDataStream = new StreamReader(stream: response.GetResponseStream());
                responseDataStream.ReadToEnd();

                request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/custom/modules/plannings/imagemap.jsp?week={1}&width={2}&height={3}", Assets.Global.ADE_HTTPS, Math.Ceiling(week), width, height));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/custom/modules/plannings/imagemap.jsp?width=1360&height=659", Assets.Global.ADE_HTTPS));
            }

            request.AllowAutoRedirect = true;
            request.CookieContainer = _cookies;
            response = (HttpWebResponse)request.GetResponse();
            responseDataStream = new StreamReader(response.GetResponseStream());
            return responseDataStream.ReadToEnd();
        }

        private bool ExploreTable(HtmlNode row, int id)
        {
            if (row.GetAttributeValue("class", string.Empty) == string.Empty)
            {
                foreach (HtmlNode cell in row.SelectNodes("th|td"))
                {
                    foreach (HtmlNode link in cell.SelectNodes("a"))
                    {
                        string idLink = Regex.Replace(link.Attributes["href"].Value, "[^0-9]", string.Empty);
                        if (idLink == id.ToString(CultureInfo.InvariantCulture))
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }
}
