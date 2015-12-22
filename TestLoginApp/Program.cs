using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestLoginApp
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("Enter SA Username");
            var username = Console.ReadLine();
            Console.WriteLine("Enter SA Password");
            var password = Console.ReadLine();
            var result1 = await Test(username, password);
            var cookies = result1.ToList();
            Console.WriteLine("First Cookie Results: ");
            foreach (var cookie in cookies)
            {
                Console.WriteLine("Cookie Name: " + cookie.Name);
                Console.WriteLine("Cookie Domain: " + cookie.Domain);
                Console.WriteLine("Cookie HttpOnly: " + cookie.HttpOnly);
            }
            var result2 = await Test(username, password);
            cookies = result2.ToList();
            Console.WriteLine("Second Cookie Results: ");
            foreach (var cookie in cookies)
            {
                Console.WriteLine("Cookie Name: " + cookie.Name);
                Console.WriteLine("Cookie Domain: " + cookie.Domain);
                Console.WriteLine("Cookie HttpOnly: " + cookie.HttpOnly);
            }
            Console.WriteLine();
        }

        static async Task<CookieContainer> Test(string username, string password)
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
