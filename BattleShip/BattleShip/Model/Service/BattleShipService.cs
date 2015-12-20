using Ninject;

namespace BattleShip.Model.Service
{
    public class GameService
    {
        private static GameService _instance;

        private readonly IKernel _kernel;

        private GameService()
        {
            _kernel = new StandardKernel();
            _kernel.Bind<IGameService>().To<BattleShipService>();
        }

        /* Initialize service at one time */
        public static IGameService Init
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameService();
                    return new BattleShipService();
                }

                return _instance._kernel.Get<IGameService>();
            }
        }
    }
}