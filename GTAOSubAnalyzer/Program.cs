using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GTAOSubAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Program program = new Program();
            await program.FetchPosts();
        }

        async Task FetchPosts()
        {
            var afterTime = 1372880565;   // Subreddit creation timestamp
            var beforeTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;    // Current timestamp

            // Expected approx. 296,713 posts so far
            // EDIT: 137048 posts fetched, setting initial capacity to 150000
            List<Submission> submissions = new List<Submission>(150000);
            HttpClient client = new HttpClient();
            while (beforeTime.CompareTo(afterTime) > 0)
            {
                // Pushshift API
                Console.WriteLine($"Quering 1000 posts before {beforeTime}...");
                var responseMsg = await client.GetAsync("https://api.pushshift.io/reddit/search/submission/" +
                    $"?subreddit=gtaonline&sort=desc&sort_type=created_utc&size=1000&after=1372880565&before={beforeTime}");
                string responseStr = await responseMsg.Content.ReadAsStringAsync();
                var jo = JObject.Parse(responseStr);
                var jt = jo.SelectToken("data");
                var responseList = jt.ToObject<List<Submission>>();
                if (responseList.Count == 0) break; // Current batch returns none, iteration complete
                submissions.AddRange(responseList);
                beforeTime = submissions[submissions.Count - 1].TimeCreated;

                Console.WriteLine($"Writing raw response to {beforeTime}.json...");
                await File.WriteAllTextAsync($"./{beforeTime}.json", responseStr);
                Console.WriteLine("Waiting for 1 second before firing next query...");
                await Task.Delay(1000);
            }
            Console.WriteLine($"Fetched a total of {submissions.Count} posts.");
            Console.WriteLine("Writing serialized JSON to disk...");

            await File.WriteAllTextAsync("./output.json", JsonConvert.SerializeObject(submissions));
            Console.WriteLine("Exporting complete.");
            Console.Read();
        }
    }
}
