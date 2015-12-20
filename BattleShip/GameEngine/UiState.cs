using System.ComponentModel;

namespace GameEngine
{
    public enum UiState
    {
        [Description("Waiting to Place")]
        WaitingToPlace = 0,

        Placing,

        [Description("Waiting to Attack")]
        WaitingToAttack,

        Attacking,

        [Description("Game Finished")]
        GameFinished
    }
}