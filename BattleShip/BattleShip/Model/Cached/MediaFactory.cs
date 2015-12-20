using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace BattleShip.Model.Cached
{
    public class MediaFactory
    {
        private static MediaFactory _instance;

        private static Dictionary<string, Image> _imageDict;
        private static Dictionary<string, Media> _mediaDict;

        private MediaFactory()
        {
            _imageDict = new Dictionary<string, Image>();
            _mediaDict = new Dictionary<string, Media>();
        }

        /* Initialize instance */
        public static MediaFactory Init => _instance ?? (
            _instance = new MediaFactory()
        );

        /**
         * Add an image if it has not yet established
         * otherwise use an existing image
         */
        public BitmapImage GetImage(string name)
        {
            if (_imageDict.ContainsKey(name))
            {
                return _imageDict[name].Source;
            }

            var image = new Image(name);
            _imageDict[name] = image;
            
            return image.Source;
        }

        /**
          * Add an audio if it has not yet established
          * otherwise use an existing audio and play it
          */
        public void PlayMedia(string name)
        {
            if (_mediaDict.ContainsKey(name))
                _mediaDict[name].Sound.Play();
            else
            {
                var media = new Media(name);
                _mediaDict[name] = media;
                media.Sound.Play();
            }
        }
    }
}
