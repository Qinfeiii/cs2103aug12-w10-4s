﻿//@author A0091539X
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
using System.Threading;

namespace Calendo.GoogleCalendar
{
    public class GoogleCalendar
    {
        private const string API_KEY = "AIzaSyDQPMYzYwXWh4JUZX16RnV2DNJddg_5INo";
        private const string ERROR_MESSAGE = "Oops. An error occured. Please try again later.";
        private static string AuthorizationCode = "";
        private TaskManager storage = TaskManager.Instance;

        public virtual bool Export()
        {
            if (AuthorizationCode == "")
                return false;
            storage.Load();
            DeleteGcalTasks(GetTasksIds(GetTaskResponse(AuthorizationCode)), AuthorizationCode);
            if (PostTasks(storage.Entries, AuthorizationCode) == false)
            {
                MessageBox.Show(ERROR_MESSAGE);
                return false;
            }
            return true;
        }

        public virtual bool Import()
        {
            if (AuthorizationCode == "")
                return false;
            List<Entry> taskList = GetTaskDetails(GetTaskResponse(AuthorizationCode));
            storage.Load();
            storage.Entries.Clear();

            foreach (Entry task in taskList)
            {
                storage.Entries.Add(task);
            }
            storage.Save();
            return true;
        }

        // Used by Task Manager to call authorization (must be on same thread as UI)
        public static string Authorize()
        {
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "770362652845-cb7ki86iesscd3f54vs8nd063epao8v3.apps.googleusercontent.com";
            AuthorizationCode = GetAuthentication(provider);
            return AuthorizationCode;
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

        private Boolean DeleteGcalTasks(List<string> taskIds, string auth)
        {
            string taskListId = GetTaskListId(auth);
            foreach (string taskId in taskIds)
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks/" + taskId + "?key=" + API_KEY + "&access_token=" + auth);
                httpWReq.Method = "DELETE";
                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.Timeout = 10000;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private Boolean PostTasks(List<Entry> tasks, string auth)
        {
            Thread.Sleep(5000);
            foreach (Entry task in tasks)
            {
                string taskListId = GetTaskListId(auth);
                HttpWebRequest httpWReq =
                    (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/" + taskListId + "/tasks?key=" + API_KEY + "&access_token=" + auth);
                httpWReq.ContentType = "application/json";
                httpWReq.Method = "POST";
                httpWReq.Timeout = 10000;
                JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
                string postData = "{\"title\": \"" + task.Description + "\"";
                if (task.Type != EntryType.Floating)
                    postData += ",\"due\": \"" + jsonObject.DateToJSON(task.StartTime) + "\"";
                postData += "}";

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(postData);

                httpWReq.ContentLength = data.Length;
                try
                {
                    using (Stream newStream = httpWReq.GetRequestStream())
                    {
                        newStream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)httpWReq.GetResponse();
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        private string GetTaskResponse(string auth)
        {
            string sURL;
            string taskListId = GetTaskListId(auth);
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

        private string GetTaskListId(string auth)
        {
            string sURL;
            sURL = "https://www.googleapis.com/tasks/v1/users/@me/lists?access_token=" + auth;
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

            JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
            TaskResponse values = jsonObject.Deserialize(taskListDetails);
            String taskListId = "";
            for (int c = 0; c < values.Items.Count; c++)
            {
                taskListId += values.Items[c].Id;
            }
            return taskListId;
        }

        private List<string> GetTasksIds(string tasks)
        {
            JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
            TaskResponse values = jsonObject.Deserialize(tasks);
            List<string> taskList = new List<string>();
            for (int c = 0; c < values.Items.Count; c++)
            {
                if (values.Items[c].Title != "")
                    taskList.Add(values.Items[c].Id);
            }
            return taskList;
        }

        private List<Entry> GetTaskDetails(string tasks)
        {
            JSON<TaskResponse> jsonObject = new JSON<TaskResponse>();
            TaskResponse values = jsonObject.Deserialize(tasks);
            List<Entry> taskList = new List<Entry>();
            for (int c = 0; c < values.Items.Count; c++)
            {
                if (values.Items[c].Title == "")
                    continue;
                Entry entry = new Entry();
                entry.Description = values.Items[c].Title;
                if (values.Items[c].due != null)
                {
                    entry.StartTime = jsonObject.JSONToDate(values.Items[c].due);
                    entry.StartTimeFormat = TimeFormat.Date;
                    entry.Type = EntryType.Deadline;
                }
                else
                {
                    entry.Type = EntryType.Floating;
                }
                taskList.Add(entry);
            }
            return taskList;
        }

    }
}
