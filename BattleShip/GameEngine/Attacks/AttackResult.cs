namespace GameEngine.Attacks
{
    public enum AttackResult
    {
        Unknown = 0,    // This is default value
        Miss = -1,      // If attacked is fails
        Hit = 1         // If user hit one of the ship part
    }
}