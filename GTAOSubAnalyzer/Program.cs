using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // await program.FetchPosts();  // Used for downloading/trimming raw post data
            await program.ReadPosts();  // Need output.json from FetchPosts()
        }

        async Task FetchPosts()
        {
            var subredditName = "gtaonline";    // Subreddit name
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
                    $"?subreddit={subredditName}&sort=desc&sort_type=created_utc&size=1000&after=1372880565&before={beforeTime}");
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

        async Task ReadPosts()
        {
            Console.WriteLine("Trying to read JSON from output.json...");
            string subStr = await File.ReadAllTextAsync("./output.json");
            var subList = JsonConvert.DeserializeObject<List<Submission>>(subStr);
            int subCount = subList.Count;

            CheckMostOccurence(subList);
            // var returnedDict = CheckOccurence(subList);
            // foreach (var entry in returnedDict) Console.WriteLine($"{entry.Key}: {entry.Value};");
            Console.Read();
        }

        /// <summary>
        /// Checks post title and text against a list of keywords.
        /// </summary>
        /// <param name="subs"></param>
        /// <returns></returns>
        Dictionary<string, int> CheckOccurence(List<Submission> subs)
        {
            Console.WriteLine("Start checking occurences...");
            Console.WriteLine("Time now: " + DateTime.Now);
            var dynamicDict = new Dictionary<string, int>()
            {
                { "mission", 0 },
                { "heist", 0 },
                { "bunker", 0 },
                { "coke", 0 },

                { "kuruma", 0 },
                { "karuma", 0 },
                { "kuroma", 0 },
                { "karoma", 0 },
                { "karomu", 0 },


                { "buzzard", 0 },
                { "avenger", 0 },
                { "hunter", 0 },
                { "akula", 0 },
                { "akuma", 0 },

                { "mrbossftw", 0 },
                { "mrcuntftw", 0 },

                { "griefer", 0 },
                { "greifer", 0 },
                { "modder", 0 },
                { "noob", 0 },
                
                { "r*", 0 },
                { "rockstar", 0 },
                { "cockstar", 0 },

                { "server", 0 },
                { "lag", 0 },
                { "loading", 0 },
            };
            var templateDict = new Dictionary<string, int>();
            foreach (var entry in dynamicDict) templateDict.Add(entry.Key, entry.Value);
            
            foreach (var sub in subs)
            {
                string text = sub.Title + sub.Selftext;
                foreach (var entry in templateDict)
                    if (Contains(entry.Key, text.ToLower())) dynamicDict[entry.Key] ++;
            }
            Console.WriteLine("Finished. Time now: " + DateTime.Now);
            return dynamicDict;
        }

        /// <summary>
        /// Counts users having most post submissions.
        /// </summary>
        /// <param name="subs"></param>
        void CheckMostOccurence(List<Submission> subs)
        {
            List<string> users = new List<string>();
            List<int> posts = new List<int>();

            foreach (var sub in subs)
            {
                if (!users.Contains(sub.Author))
                {
                    users.Add(sub.Author);
                    posts.Add(1);
                }
                else posts[users.IndexOf(sub.Author)]++;
            }
            // Print out top 100 posters
            for (int i = 1; i <= 100; i ++)
            {
                int top = posts.IndexOf(posts.Max());
                Console.WriteLine($"Top {i}: {users[top]}, {posts.Max()}");
                users.RemoveAt(top);
                posts.RemoveAt(top);
            }
        }

        /// <summary>
        /// Quicker method to check whether a string contains another one.
        /// </summary>
        /// <param name="find"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        static bool Contains(string find, string from)
        {
            return (from.Length - from.Replace(find, "").Length) / find.Length > 0;
        }
    }
}
