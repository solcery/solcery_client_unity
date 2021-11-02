namespace Solcery.Services.GameContent
{
    public class GameContentService : IGameContentService
    {
        public static IGameContentService Create()
        {
            return new GameContentService();
        }
        
        private GameContentService() { }
    }
}