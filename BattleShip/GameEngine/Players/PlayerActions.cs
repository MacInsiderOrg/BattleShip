namespace GameEngine.Players
{
    public class PlayerActions
    {
        public int Turns { get; set; }
        public int Misses { get; set; }
        public int Hits { get; set; }

        public PlayerActions()
        {
            Turns = 0;
            Misses = 0;
            Hits = 0;
        }
    }
}