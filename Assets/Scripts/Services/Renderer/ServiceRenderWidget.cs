using System.Collections.Generic;
using Solcery.Services.Renderer.DTO;
using Solcery.Services.Renderer.Widgets;
using UnityEngine;

namespace Solcery.Services.Renderer
{
    public sealed class ServiceRenderWidget : IServiceRenderWidget
    {
        private readonly IServiceRenderDto _dto;
        private readonly Dictionary<RectTransform, IWidgetRenderData> _widgetRenderList;

        public static IServiceRenderWidget Create(IServiceRenderDto dto)
        {
            return new ServiceRenderWidget(dto);
        }

        private ServiceRenderWidget(IServiceRenderDto dto)
        {
            _dto = dto;
            _widgetRenderList = new Dictionary<RectTransform, IWidgetRenderData>();
        }

        IWidgetRenderData IServiceRenderWidget.CreateWidgetRender(RectTransform widget)
        {
            if (_widgetRenderList.ContainsKey(widget))
            {
                return _widgetRenderList[widget];
            }
            
            var renderObject = WidgetRenderData.Create(widget, _dto.WidgetRenderPrefab, _dto.Frame);
            _widgetRenderList.Add(widget, renderObject);
            return renderObject;
        }

        void IServiceRenderWidget.ReleaseWidgetRender(RectTransform widget)
        {
            if (!_widgetRenderList.TryGetValue(widget, out var widgetRender))
            {
                return;
            }
            
            widgetRender.Release();
            _widgetRenderList.Remove(widget);
        }
    }
}