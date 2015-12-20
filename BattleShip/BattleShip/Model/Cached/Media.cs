using System.Media;

namespace BattleShip.Model.Cached
{
    public class Media
    {
        public SoundPlayer Sound { get; }

        public Media(string mediaPath)
        {
            Sound = new SoundPlayer
            {
                SoundLocation = $"../../Resources/audio/{mediaPath}.wav"
            };
        }
    }
}