using Solcery.GameStateDebug;
using Solcery.Services.Renderer.DTO;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Games.DTO
{
    public sealed class GameInitDto : IGameInitDto
    {
        public Camera MainCamera => _mainCamera;
        public IWidgetCanvas WidgetCanvas => _widgetCanvas;
        public IServiceRenderDto ServiceRenderDto => _serviceRenderDto;
        public GameStateDebugView GameStateDebugView => _gameStateDebugView;
        
        private readonly Camera _mainCamera;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceRenderDto _serviceRenderDto;
        private readonly GameStateDebugView _gameStateDebugView;

        public static IGameInitDto Create(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto, GameStateDebugView gameStateDebugView)
        {
            return new GameInitDto(mainCamera, widgetCanvas, serviceRenderDto, gameStateDebugView);
        }

        private GameInitDto(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto, GameStateDebugView gameStateDebugView)
        {
            _mainCamera = mainCamera;
            _widgetCanvas = widgetCanvas;
            _serviceRenderDto = serviceRenderDto;
            _gameStateDebugView = gameStateDebugView;
        }
    }
}