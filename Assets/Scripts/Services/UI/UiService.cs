using System;
using Newtonsoft.Json.Linq;
using Solcery.Models;
using Solcery.Models.Components;
using Solcery.Services.GameContent;
using Solcery.Services.Widget;
using Solcery.Services.Widget.Data;
using Solcery.Widgets.UI;
using UnityEngine;

namespace Solcery.Services.Ui
{
    public class UiService : IUiService
    {
        private readonly IGameContentService _gameContentService;
        private readonly IWidgetService _widgetService;
        private readonly IModel _model;
        
        public static UiService Create(IGameContentService gameContentService, IWidgetService widgetService, IModel model)
        {
            return new UiService(gameContentService, widgetService, model);
        }

        private UiService(IGameContentService gameContentService, IWidgetService widgetService, IModel model)
        {
            _gameContentService = gameContentService;
            _widgetService = widgetService;
            _model = model;
        }

        public void Init()
        {
            _gameContentService.EventOnReceivingUi += OnReceivingUi;
        }

        public void Cleanup()
        {
        }

        public void Destroy()
        {
        }

        private void OnReceivingUi(JObject obj)
        {
            CreateUiWidget(obj);
        }

        private void CreateUiWidget(JObject obj, UiBaseWidget parent = null)
        {
            try
            {
                var gui = obj["guid"]!.Value<int>();
                var type = obj["ui_widget_type"]!.ToObject<UiWidgetTypes>();
                var widget = _widgetService.GetUiWidget(type, parent);
                widget.ApplyTransform(TransformData.Parse(obj["transform"]!.Value<JObject>()));
                CreateUiEntity(gui, widget);
                if (obj.TryGetValue("childs", out var childs))
                {
                    foreach (var child in childs.Children())
                    {
                       CreateUiWidget(child as JObject, widget);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogError($"Can't parse \"game_content/ui!\": {ex.Message}");
            }
        }

        private void CreateUiEntity(int guid, UiBaseWidget widget)
        {
            var world = _model.World;
            
            var entity = _model.World.NewEntity();
            
            var uiComponents = world.GetPool<UiComponent>();
            ref var uiComponent = ref uiComponents.Add(entity);
            uiComponent.Guid = guid;

            var uiWidgetComponents = world.GetPool<UiWidgetComponent>();
            ref var uiWidgetComponent = ref uiWidgetComponents.Add(entity);
            uiWidgetComponent.Widget = widget;
        }
    }
}