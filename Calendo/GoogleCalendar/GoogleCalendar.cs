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
using Calendo.Data;
using System.Windows.Forms;

namespace Calendo.GoogleCalendar
{
    class GoogleCalendar
    {
        private static string STORAGE_PATH = "archive.txt";
        private static StateStorage<List<Entry>> storage = new StateStorage<List<Entry>>(STORAGE_PATH);
       // private static String auth = Authorize();
       // private static String taskListId = getTaskListId(auth);
    
        public static String Sync()
        {
            storage.Load();
            string auth = Authorize();
            List<String> tasks = getTasksDetails(getTaskResponse(auth));
            postTasks(storage.Entries, auth);
            deleteGcalTasks(tasks, auth);
            return "";
        }

        private static string getTaskResponse(String auth)
        {
            string sURL;
            String taskListId = getTaskListId(auth);
            sURL = "https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks?access_token=" + auth;

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
                    tasks += sLine;
            }
            deleteGcalTasks(getTasksDetails(tasks), auth);
            return tasks;
        }
        public static String Import()
        {
            return "";     
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
                //taskListId += Console.WriteLine(values.items[c].id);
                taskListId += values.items[c].id;
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

        private static String postTasks(List<Entry> tasks, string auth)
        {
            var responseText = "";

            foreach (Entry task in tasks)
            {
                //MessageBox.Show(task.Description);
                String taskListId = getTaskListId(auth);
                HttpWebRequest httpWReq =
                    (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks?key=AIzaSyDQPMYzYwXWh4JUZX16RnV2DNJddg_5INo&access_token=" + auth);

                httpWReq.ContentType = "application/json";
                ASCIIEncoding encoding = new ASCIIEncoding();

                responseText = "";
                JSON<TI> jtest = new JSON<TI>();
                string postData = "{\"title\": \"" + task.Description + "\",\"due\": \"" + jtest.DateToJSON(task.StartTime) + "\"}";

                byte[] data = encoding.GetBytes(postData);

                httpWReq.Method = "POST";
                httpWReq.ContentLength = data.Length;

                using (Stream newStream = httpWReq.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                }

                var httpResponse = (HttpWebResponse)httpWReq.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText += streamReader.ReadToEnd();
                }
            }

        return responseText;
		}
		
		private static void deleteGcalTasks(List<String> taskIds, String auth)
        {
            String taskListId = getTaskListId(auth);
            storage.Load();
            foreach (String taskId in taskIds)
            {
                //MessageBox.Show(taskId);
                HttpWebRequest httpWReq =
                        (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks/"+taskId+"?key=AIzaSyDQPMYzYwXWh4JUZX16RnV2DNJddg_5INo&access_token=" + auth);

                ASCIIEncoding encoding = new ASCIIEncoding();
                httpWReq.Method = "DELETE";
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
            }
            //MessageBox.Show("delete loop over");
        }
		
		private static List<String> getTasksDetails(string tasks)
		{
            JSON<TaskResponse> jtest = new JSON<TaskResponse>();
            TaskResponse values = jtest.Deserialize(tasks);
            List<String> taskList = new List<string>();
            for (int c = 0; c < values.items.Count; c++)
            {
                //taskListId += Console.WriteLine(values.items[c].id);
                if (values.items[c].title!="")
                    taskList.Add(values.items[c].id);
            }
            return taskList;
        }

    }
}
