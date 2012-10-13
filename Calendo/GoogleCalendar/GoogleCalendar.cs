﻿using System;
using System.Net;
using System.Collections.Generic;
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


namespace Calendo.GoogleCalendar
{
    class GoogleCalendar
    {
	
	public static bool Sync(List<Entry> entries){
	   String auth = Authorize();
        //postTasks(tasks, auth);
       return false;
	}
	public static String Import(){
			String auth = Authorize();
            string sURL;
            sURL = " https://www.googleapis.com/tasks/v1/lists/MTU4OTEwNzMxMTYxNzgzMjEwNDc6MDow/tasks?access_token=" + auth;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            String tasks="";
            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    tasks+=i + ": " + sLine;
            }
        return tasks;
	}
	private static string Authorize()
        { 
            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "770362652845-cb7ki86iesscd3f54vs8nd063epao8v3.apps.googleusercontent.com";
            String auth = GetAuthentication(provider);
            return auth;
        }

        private static String GetAuthentication(NativeApplicationClient provider)
        {
            String url="https://accounts.google.com/o/oauth2/auth?";
            url += "scope=https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/tasks";
            url+="&redirect_uri=http://rahij.com/calendo.php";
            url+="&response_type=token";
            url += "&client_id=" + provider.ClientIdentifier;
            Console.WriteLine(url);
            Uri authUri = new Uri(url);
            
            // Request authorization from the user (by opening a browser window):
            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();
            Console.WriteLine();

            // Retrieve the access token by using the authorization code:
            return authCode;
        }
		
        private static void postTasks(Entry tasks, String auth)
        {
            HttpWebRequest httpWReq =
            (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/tasks/v1/lists/MTU4OTEwNzMxMTYxNzgzMjEwNDc6MDow/tasks?access_token="+auth);

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
                newStream.Write(data,0,data.Length);
            }
        }

    }
}
