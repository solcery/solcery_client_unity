using System;
using Newtonsoft.Json.Linq;
using Solcery.Models;
using Solcery.Models.Triggers;
using Solcery.Models.Ui;
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
            CreateUiElement(obj);
        }

        private UiBaseWidget GetUiWidget(JObject obj, UiBaseWidget parent = null)
        {
            var type = obj["ui_widget_type"]!.ToObject<UiWidgetTypes>();
            var widget = _widgetService.GetUiWidget(type, parent);
            widget.ApplyTransform(TransformData.Parse(obj["transform"]!.Value<JObject>()));
            return widget;
        }

        private void CreateUiElement(JObject obj, UiBaseWidget parent = null)
        {
            try
            {
                var widget = GetUiWidget(obj, parent);
                CreateUiEntity(obj, widget);
                if (obj.TryGetValue("childs", out var childs))
                {
                    foreach (var child in childs.Children())
                    {
                        CreateUiElement(child as JObject, widget);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Can't parse \"game_content/ui!\": {ex.Message}");
            }
        }

        
        private void CreateUiEntity(JObject obj, UiBaseWidget widget)
        {
            var world = _model.World;
            
            var entity = _model.World.NewEntity();
            
            // ui
            var uiComponents = world.GetPool<ComponentUi>();
            ref var uiComponent = ref uiComponents.Add(entity);
            uiComponent.Guid = obj["guid"]!.Value<int>();;

            // widget
            var uiWidgetComponents = world.GetPool<ComponentUiWidget>();
            ref var uiWidgetComponent = ref uiWidgetComponents.Add(entity);
            uiWidgetComponent.Widget = widget;
            
            // triggers
            if (obj.TryGetValue("triggers", out var triggers))
            {
                var triggersComponents = world.GetPool<ComponentTriggers>();
                ref var triggersComponent = ref triggersComponents.Add(entity);
                triggersComponent.Triggers = TriggersData.Parse(triggers);
            }
        }
    }
}