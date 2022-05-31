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
        
        private readonly Camera _mainCamera;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceRenderDto _serviceRenderDto;

        public static IGameInitDto Create(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto)
        {
            return new GameInitDto(mainCamera, widgetCanvas, serviceRenderDto);
        }

        private GameInitDto(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto)
        {
            _mainCamera = mainCamera;
            _widgetCanvas = widgetCanvas;
            _serviceRenderDto = serviceRenderDto;
        }
    }
}