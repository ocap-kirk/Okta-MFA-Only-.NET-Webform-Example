using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OktaMfaOnly
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public string stateToken;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Code that runs on application startup
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
            new ScriptResourceDefinition
            {
                Path = "~/scripts/jquery-3.3.1.min.js",
                DebugPath = "~/scripts/jquery-3.3.1.js",
                CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.min.js",
                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.3.1.js"
            });

        }
        protected void Button1_Click1(object sender, EventArgs e)
        {
            
            string toke = mfaWithOkta(username.Text);
            if (toke != null)
                stateToken = toke;
            username.Visible = false;
            Button1.Visible = false;
        }
        protected string mfaWithOkta(string username)
        {
            OktaSettings oktaSettings = new Okta.Core.OktaSettings();
            oktaSettings.ApiToken = "00jSoRPyhdLF9MBypNmvkdm0LEXKZAc4tam7lw1Dqq";
            oktaSettings.BaseUri = new Uri("https://org.oktapreview.com");

            UsersClient usersClient = new UsersClient(oktaSettings);

            //create with fakedomain, fake email, fake first&last name
            User tempUser = new User(username+"@oktalife.info", "fake@fake.fake.fake.com", "Fake", "Fake");


            tempUser.Credentials = new LoginCredentials();

            String password = System.Convert.ToBase64String(HMACSHA256PasswordGenerator.GenerateHash(username));

            tempUser.Credentials.Password.Value = password;

            string[] groups = new string[1];

            //add the user to the Okta group that prompts for MFA
            groups[0] = "00gbdoy4imvfNeV3Z0h7";
            tempUser.SetProperty("groupIds", groups);
            Console.WriteLine(tempUser.ToJson());

            try
            {
                usersClient.Add(tempUser, true);
            }
            catch(OktaException ex)
            {
                //user already created, move on
            }

            AuthClient authN = new Okta.Core.Clients.AuthClient(oktaSettings);
            AuthResponse authResp = authN.Authenticate(username, password);
            return authResp.StateToken;
            
        }
        protected void Button2_Click1(object sender, EventArgs e)
        {
            OktaSettings oktaSettings = new Okta.Core.OktaSettings();
            oktaSettings.ApiToken = "00jSoRPyhdLF9MBypNmvkdm0LEXKZAc4tam7lw1Dqq";
            oktaSettings.BaseUri = new Uri("https://org.oktapreview.com");

            SessionsClient sessionsClient = new SessionsClient(oktaSettings);

            try
            {
                sessionsClient.CreateSession(sessionToken.Text);
                ResultOfMFA.Text = "Successful MFA with Okta";
            }
            catch (OktaException)
            {
                //Invalid session validation, don't let them through
                ResultOfMFA.Text = "Invalid MFA with Okta";
            
            }
            ResultOfMFA.Style.Add("display","block");
        }

    }
    public static class HMACSHA256PasswordGenerator
    {

        public static byte[] GenerateHash(String stringToHash)
        {
            //RNGCryptoServiceProvider is an implementation of a random number generator.
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                //provide a 64 byte[] string to base the hash on
                byte[] secretkey = new Byte[64];

                // Typically the array is filled with cryptographically strong random bytes. But for the sake of this project,
                //   it is based on a provided key.
                //      typically: rng.GetBytes(secretkey);
                //      our use: secretkey = Encoding.ASCII.GetBytes("yhQDsAxWCvNbnwx8tKp5Mz5ifYLQUytAidd7zZCmMJQXcto9bOAdQACNtwuBvV57")
                secretkey = Encoding.ASCII.GetBytes("yhQDsAxWCvNbnwx8tKp5Mz5ifYLQUytAidd7zZCmMJQXcto9bOAdQACNtwuBvV57");

                using (HMACSHA256 hmac = new HMACSHA256(secretkey))
                {

                    byte[] hashValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(stringToHash));
                    return hashValue;
                }


            }

        }

    }
}