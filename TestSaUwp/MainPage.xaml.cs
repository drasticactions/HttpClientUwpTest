using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HttpClient = System.Net.Http.HttpClient;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestSaUwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameTextbox.Text) || string.IsNullOrEmpty(PasswordTextBox.Password)) return;

            var result = await Test(UsernameTextbox.Text, PasswordTextBox.Password);
            var cookieList = Serialize(result.GetCookies(new Uri("http://fake.forums.somethingawful.com")));
            ConsoleTest.Text = string.Empty;
            ConsoleTest.Text = "First Cookie Result";

            // NOTE: Does not show every cookie. bbuserid and bbpassword are not apart of the cookie container. 
            // But they are in the console project... and they do get sent with every new httpclient... wtf.
            foreach (var cookie in cookieList)
            {
                ConsoleTest.Text += Environment.NewLine + "Cookie Name: " + cookie.Name;
                ConsoleTest.Text += Environment.NewLine + "Cookie Domain: " + cookie.Domain;
                ConsoleTest.Text += Environment.NewLine + "Cookie HttpOnly: " + cookie.HttpOnly;
            }

            // This actually does show bbuserid and bbpassword. While they were not in the cookie container, they do
            // get stored in the "cookie manager". Again, why here?
            var filter = new HttpBaseProtocolFilter();
            var cookieManager = filter.CookieManager;
            foreach (var cookie in cookieManager.GetCookies(new Uri("http://fake.forums.somethingawful.com")))
            {
                cookieManager.DeleteCookie(cookie);
            }

            result = await Test(UsernameTextbox.Text, PasswordTextBox.Password);
            cookieList = Serialize(result.GetCookies(new Uri("http://fake.forums.somethingawful.com")));
            ConsoleTest.Text += Environment.NewLine + "Second Cookie Result";

            foreach (var cookie in cookieList)
            {
                ConsoleTest.Text += Environment.NewLine + "Cookie Name: " + cookie.Name;
                ConsoleTest.Text += Environment.NewLine + "Cookie Domain: " + cookie.Domain;
                ConsoleTest.Text += Environment.NewLine + "Cookie HttpOnly: " + cookie.HttpOnly;
            }
        }

        List<Cookie> Serialize(CookieCollection cookies)
        {
            var cookieList = cookies.OfType<Cookie>();
            return cookieList.ToList();
        }

        async Task<CookieContainer> Test(string username, string password)
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true,
                UseDefaultCredentials = false,
                CookieContainer = cookieContainer
            };
            using (var client = new HttpClient(handler))
            {
                var dic = new Dictionary<string, string>
                {
                    ["action"] = "login",
                    ["username"] = username,
                    ["password"] = password
                };
                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
                {
                    NoCache = true
                };
                var header = new FormUrlEncodedContent(dic);
                var response = await client.PostAsync(new Uri("http://forums.somethingawful.com/account.php?"), header);
                return cookieContainer;
            }
        }
    }
}
