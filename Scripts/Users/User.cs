namespace TwitchBot.Scripts.User
{
    /// <summary>
    /// User class, handles its own points and data
    /// </summary>
    public class User
    {
        /// <summary>
        /// Returns userId
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Returns current point count
        /// </summary>
        public int points { get; private set; } = 10;

        /// <summary>
        /// Constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public User(string id) : base()
        {
            this.id = id;
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
        public void RemovePoints(int removedPoints)
        {
            if (removedPoints < 0)
                return;

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