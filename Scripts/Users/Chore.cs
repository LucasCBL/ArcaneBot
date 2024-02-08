namespace TwitchBot.Scripts.Users
{
    /// <summary>
    /// Chore / task set by the user, each user has a task list
    /// </summary>
    public class Chore
    {
        /// <summary> Task name / description </summary>
        public string Description { get; set; }

        /// <summary> Created at date </summary>
        public DateTime Created { get; set; } = DateTime.Now;

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="description"></param>
        /// <param name="created"></param>
        public Chore(string description, DateTime created)
        {
            Description = description;
            Created = created;
        }

        /// <summary>
        /// Task constructor
        /// </summary>
        /// <param name="description"></param>
        public Chore(string description) : this(description, DateTime.Now)
        { }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Chore() { }

    }
}
