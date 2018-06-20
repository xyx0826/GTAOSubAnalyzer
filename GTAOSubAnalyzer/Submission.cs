using Newtonsoft.Json;

namespace GTAOSubAnalyzer
{
    /// <summary>
    /// Object that holds the information of a reddit post.
    /// </summary>
    public class Submission
    {
        /// <summary>
        /// The author of the submission.
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        /// The title of the submission.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The text of the submission (in text mode).
        /// </summary>
        [JsonProperty("selftext")]
        public string Selftext { get; set; }

        /// <summary>
        /// The creation time, in seconds Unix Timestamp, of the submission.
        /// </summary>
        [JsonProperty("created_utc")]
        public int TimeCreated { get; set; }

        /// <summary>
        /// The full link to the submission.
        /// </summary>
        [JsonProperty("full_link")]
        public string FullLink { get; set; }

        /// <summary>
        /// The score of the submission.
        /// </summary>
        [JsonProperty("score")]
        public int Score { get; set; }

        /// <summary>
        /// The count of comments under the submission.
        /// </summary>
        [JsonProperty("num_comments")]
        public int CommentCount { get; set; }

        /// <summary>
        /// The count of times the submission being x-posted.
        /// </summary>
        [JsonProperty("num_crossposts")]
        public int CrosspostCount { get; set; }

        /// <summary>
        /// Whether the submission is NSFW.
        /// </summary>
        [JsonProperty("over_18")]
        public bool IsNSFW { get; set; }

        /// <summary>
        /// Whether the submission is a spoiler.
        /// </summary>
        [JsonProperty("spoiler")]
        public bool IsSpoiler { get; set; }

        /// <summary>
        /// Whether the submission is locked by a moderator.
        /// </summary>
        [JsonProperty("locked")]
        public bool IsLocked { get; set; }

        /// <summary>
        /// The flair of the submission.
        /// </summary>
        [JsonProperty("link_flair_text")]
        public string LinkFlairText { get; set; }

        /// <summary>
        /// The count of subreddit subscribers by the time the submission is created.
        /// </summary>
        [JsonProperty("subreddit_subscribers")]
        public int SubCount { get; set; }
    }

}
