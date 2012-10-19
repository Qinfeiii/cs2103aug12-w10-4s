using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Diagnostics;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util;
using Newtonsoft.Json;


namespace Calendo.GoogleCalendar
{
    class GoogleCalendar
    {

        public static bool Sync(List<Entry> entries)
        {
            string auth = Authorize();
            //postTasks(tasks, auth);
            return false;
        }
        public static string Import()
        {
            string auth = Authorize();
            if (auth == "")
            {
                return "";
            }
            string sURL;
            sURL = "https://www.googleapis.com/tasks/v1/lists/" + getTaskListId(auth) + "/tasks?access_token=" + auth;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            string tasks = "";
            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    tasks += i + ": " + sLine;
            }
            return tasks;
        }

        private static string getTaskListId(String auth)
        {
            string sURL;
            sURL = " https://www.googleapis.com/tasks/v1/users/@me/lists?access_token=" + auth;
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);
            string sLine = "", taskListDetails = "";
            int i = 0;


            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    taskListDetails += sLine;
            }

            JSON<TI> jtest = new JSON<TI>();
            TI values = jtest.Deserialize(taskListDetails);
            String taskListId = "";
            for (int c = 0; c < values.items.Count; c++)
            {
                taskListId += Console.WriteLine(values.items[c].id);
            }
            return taskListId;
        }
        private static string Authorize()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "770362652845-cb7ki86iesscd3f54vs8nd063epao8v3.apps.googleusercontent.com";
            string auth = GetAuthentication(provider);
            return auth;
        }

        private static string GetAuthentication(NativeApplicationClient provider)
        {
            string url = "https://accounts.google.com/o/oauth2/auth?";
            url += "scope=https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/tasks";
            url += "&redirect_uri=http://rahij.com/calendo.php";
            url += "&response_type=token";
            url += "&client_id=" + provider.ClientIdentifier;
            Uri authUri = new Uri(url);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());

            AskAuth a = new AskAuth();
            a.ShowDialog();
            string authCode = a.authCode;

            // Retrieve the access token by using the authorization code:
            return authCode;
        }

        private static void postTasks(Entry tasks, string auth)
        {
            HttpWebRequest httpWReq =
            (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/MTU4OTEwNzMxMTYxNzgzMjEwNDc6MDow/tasks?access_token=" + auth);

            ASCIIEncoding encoding = new ASCIIEncoding();
            //string postData = "{ kind: tasks#task,";
            string postData = "\"title\": lol_its_task";
            //postData += "status: completed}";

            byte[] data = encoding.GetBytes(postData);

            httpWReq.Method = "POST";
            //httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.ContentLength = data.Length;

            using (Stream newStream = httpWReq.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }
        }

    }
}
