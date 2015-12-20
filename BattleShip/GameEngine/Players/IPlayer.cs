using GameEngine.Attacks;
using GameEngine.Ships;

namespace GameEngine.Players
{
    public interface IPlayer
    {
        string PlayerName { get; set; }
        string PlayerAvatar { get; }

        // Player ships
        Ship[] Ships { get; }

        // Player attacks
        Attack[,] Attacks { get; set; }

        // Max coordinates
        Coordinate MaxCoordinates { get; set; }

        // Initialize Player
        void Initialize(Coordinate maxCoords, Ship[] startingShips);

        // Check if user is ready to go :)
        bool IsReady();

        // Place all ships
        void PlaceShips();

        // Player attack
        Coordinate Attack();

        // Update list of attacks
        void UpdateAttackResults(Coordinate lastAttack, AttackResult attackResult, bool sunkShip);

        // If user Win -> send notification
        void WinnerNotification(string winnerName);

        // Update Ui State
        void UpdateUiState(UiState currentState);
    }
}