using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Services.Resources;
using Solcery.Widgets.Canvas;
using UnityEngine;

namespace Solcery.Widgets.Picture
{
    public class WidgetPicture : Widget
    {
        private readonly WidgetPictureViewData _viewData;
        private readonly GameObject _gameObject;
        
        private WidgetPictureView _pictureView;
        public override WidgetViewBase View => _pictureView;

        public static WidgetPicture Create(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, JObject jsonData)
        {
            var viewData = new WidgetPictureViewData();
            if (viewData.TryParse(jsonData))
            {
                return new WidgetPicture(widgetCanvas, serviceResource, viewData);
            }
            
            return null;
        }
        
        private WidgetPicture(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPictureViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            _gameObject = (GameObject) Resources.Load("ui/picture");
            CreateView();
        }

        private void CreateView()
        {
            _pictureView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetPictureView>(_gameObject);
            if (ServiceResource.GetTextureByKey(_viewData.Picture, out var texture))
            {
                _pictureView.Image.material.mainTexture = texture;
            }
            _pictureView.Init();
        }
        
        protected override Widget AddInternalWidget(EcsWorld world, int entityId, JObject data)
        {
            return null;
        }

        protected override void ClearInternalView()
        {
            _pictureView.Clear();
            WidgetCanvas.GetWidgetPool().ReturnToPool(_pictureView);
            _pictureView = null;
        }
    }
}
