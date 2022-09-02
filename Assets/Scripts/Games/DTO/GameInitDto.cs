using Solcery.Services.Renderer.DTO;
using Solcery.Services.Sound;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Games.DTO
{
    public sealed class GameInitDto : IGameInitDto
    {
        public Camera MainCamera => _mainCamera;
        public IWidgetCanvas WidgetCanvas => _widgetCanvas;
        public IServiceRenderDto ServiceRenderDto => _serviceRenderDto;
        public SoundsLayout SoundsLayout => _soundsLayout;
        
        private readonly Camera _mainCamera;
        private readonly IWidgetCanvas _widgetCanvas;
        private readonly IServiceRenderDto _serviceRenderDto;
        private readonly SoundsLayout _soundsLayout;

        public static IGameInitDto Create(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto, SoundsLayout soundsLayout)
        {
            return new GameInitDto(mainCamera, widgetCanvas, serviceRenderDto, soundsLayout);
        }

        private GameInitDto(Camera mainCamera, IWidgetCanvas widgetCanvas, IServiceRenderDto serviceRenderDto, SoundsLayout soundsLayout)
        {
            _mainCamera = mainCamera;
            _widgetCanvas = widgetCanvas;
            _serviceRenderDto = serviceRenderDto;
            _soundsLayout = soundsLayout;
        }
    }
}