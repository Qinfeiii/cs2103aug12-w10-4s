using System;
using System.IO;
using System.Net;
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

        public void authorize()
        {
			var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = "770362652845-cb7ki86iesscd3f54vs8nd063epao8v3.apps.googleusercontent.com";
            //provider.ClientSecret = "Uq2FptnfOI2BVXCsvt8Q7D4K";   

            // Create the service and register the previously created OAuth2 Authenticator.
            String auth = GetAuthentication(provider);

            
            string sURL;
            sURL = "https://www.googleapis.com/oauth2/v1/userinfo?access_token="+auth;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;

            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }
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

    }
}
