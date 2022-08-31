using Solcery.Services.Renderer.DTO;
using Solcery.Services.Sound;
using Solcery.Widgets_new.Canvas;
using UnityEngine;

namespace Solcery.Games.DTO
{
    public interface IGameInitDto
    {
        Camera MainCamera { get; }
        IWidgetCanvas WidgetCanvas { get; }
        IServiceRenderDto ServiceRenderDto { get; }
        SoundsLayout SoundsLayout { get; }
    }
}