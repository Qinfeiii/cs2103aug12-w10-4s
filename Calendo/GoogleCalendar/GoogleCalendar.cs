//@author A0091539X
using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Diagnostics;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util;
using Calendo.Logic;
using System.Windows.Forms;

namespace Calendo.GoogleCalendar
{
    class GoogleCalendar
    {
        private TaskManager storage = TaskManager.Instance;
        private static string auth = "";
   
        public Boolean Export()
        {
            if (auth == "")
                return false;
            storage.Load();
            deleteGcalTasks( getTasksIds(getTaskResponse(auth)),auth);
            postTasks(storage.Entries, auth);
            return true;
        }

        public Boolean Import()
        {
            if (auth == "")
                return false;
            List<Entry> taskList = getTaskDetails(getTaskResponse(auth));
            storage.Load();
            storage.Entries.Clear();

            foreach (Entry task in taskList)
            {
                storage.Entries.Add(task);
            }
            storage.Save();
            return true;
        }

        private string getTaskResponse(string auth)
        {
            string sURL;
            string taskListId = getTaskListId(auth);
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
            return tasks;
        }

        private string getTaskListId(String auth)
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

            JSON<TaskResponse> jtest = new JSON<TaskResponse>();
            TaskResponse values = jtest.Deserialize(taskListDetails);
            String taskListId = "";
            for (int c = 0; c < values.Items.Count; c++)
            {
                taskListId += values.Items[c].Id;
            }
            return taskListId;
        }

        // Used by Task Manager to call authorization (because must be on same thread)
        public static string Authorize()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "770362652845-cb7ki86iesscd3f54vs8nd063epao8v3.apps.googleusercontent.com";
            auth = GetAuthentication(provider);
            return auth;
        }

        private static string GetAuthentication(NativeApplicationClient provider)
        {
            string url = "https://accounts.google.com/o/oauth2/auth?";
            url += "scope=https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/tasks+https://www.googleapis.com/auth/calendar";
            url += "&redirect_uri=http://rahij.com/calendo.php";
            url += "&response_type=token";
            url += "&client_id=" + provider.ClientIdentifier;
            Uri authUri = new Uri(url);

            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());

            AskAuth a = new AskAuth();
            a.ShowDialog();
            string authCode = a.AuthorizationCode;

            if (authCode == "")
            {
                return "";
            }
            // Retrieve the access token by using the authorization code:
            return authCode;
        }

        private string postTasks(List<Entry> tasks, string auth)
        {
            var responseText = "";

            foreach (Entry task in tasks)
            {
                String taskListId = getTaskListId(auth);
                /*
                 * Creating a http request
                 */

                HttpWebRequest httpWReq =
                    (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks?key=AIzaSyDQPMYzYwXWh4JUZX16RnV2DNJddg_5INo&access_token=" + auth);
                httpWReq.ContentType = "application/json";
                httpWReq.Method = "POST";
                httpWReq.Timeout = 10000;
                JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
                string postData = "{\"title\": \"" + task.Description + "\"";
                if (task.Type != EntryType.FLOATING)
                    postData += ",\"due\": \"" + jsonObject.DateToJSON(task.StartTime) + "\"";
                postData += "}";
                responseText = "";

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(postData);

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
		
		private void deleteGcalTasks(List<String> taskIds, string auth)
        {
            string taskListId = getTaskListId(auth);
            foreach (string taskId in taskIds)
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks/" + taskId + "?key=AIzaSyDQPMYzYwXWh4JUZX16RnV2DNJddg_5INo&access_token=" + auth);
                httpWReq.Method = "DELETE";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                MessageBox.Show(response.StatusDescription);
            }
        }
		
		private List<String> getTasksIds(string tasks)
		{
            JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
            TaskResponse values = jsonObject.Deserialize(tasks);
            List<String> taskList = new List<string>();
            for (int c = 0; c < values.Items.Count; c++)
            {
                if (values.Items[c].Title!="")
                    taskList.Add(values.Items[c].Id);
            }
            return taskList;
        }

        private List<Entry> getTaskDetails(string tasks)
        {
            JSON<TaskResponse> jtest = new JSON<TaskResponse>();
            TaskResponse values = jtest.Deserialize(tasks);
            List<Entry> taskList = new List<Entry>();
            for (int c = 0; c < values.Items.Count; c++)
            {
                if (values.Items[c].Title == "")
                    continue;
                Entry entry = new Entry();
                entry.Description = values.Items[c].Title;
                if (values.Items[c].due != null)
                {
                    entry.StartTime = jtest.JSONToDate(values.Items[c].due);
                    entry.StartTimeFormat = TimeFormat.DATE;
                    entry.Type = EntryType.DEADLINE;
                }
                else
                    entry.Type = EntryType.FLOATING;
                taskList.Add(entry);
            }
            return taskList;
        }

    }
}
