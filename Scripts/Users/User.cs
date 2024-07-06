namespace TwitchBot.Scripts.Users
{
    /// <summary>
    /// User class, handles its own points and data
    /// </summary>
    public class User
    {
        /// <summary> Last known username  </summary>
        public string name { get; set; }

        /// <summary> Returns userId </summary>
        public string id { get; set; }

        /// <summary> Returns current point count </summary>
        public int points { get; set; } = 10;

        /// <summary> Returns current amount of points lost by gambling </summary>
        public int pointLoss { get; set; } = 0;

        /// <summary> list of chores of the user </summary>
        public List<Chore> chores { get; set; } = new();

        /// <summary> Amount of times a user has poofed </summary>
        public int poofCount { get; set; } = 0;

        /// <summary> List of channels this user has been seen in (relevant to keep tops relatively isolated by channel) </summary>
        public List<string> channelIds { get; set; } = new();

        /// <summary> Last time user called a command, used to reduce spam </summary>
        public DateTime lastCommand = DateTime.Now;


        /// <summary>
        /// Constructor
        /// </summary>
        public User() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public User(string id) : base()
        {
            this.id = id;
            this.points = 10;
        }

        /// <summary>
        /// Adds points to the user
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(int addedPoints)
        {
            if (addedPoints < 0)
                return;

            points += addedPoints;
        }

        /// <summary>
        /// Removes points from the user
        /// </summary>
        /// <param name="removedPoints"></param>
        public void RemovePoints(int removedPoints, bool gamblingLoss = false)
        {
            if (removedPoints < 0)
                return;
            pointLoss += gamblingLoss ? removedPoints : 0;
            points -= removedPoints;
        }

        /// <summary>
        /// Gives own points to another user
        /// </summary>
        /// <param name="givenPoints"></param>
        /// <param name="targetUser"></param>
        public void GivePoints(int givenPoints, User targetUser)
        {
            RemovePoints(givenPoints);
            targetUser.AddPoints(givenPoints);
        }
    }
}