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

        public WidgetPicture(IWidgetCanvas widgetCanvas, IServiceResource serviceResource, WidgetPictureViewData viewData) : base(widgetCanvas, serviceResource)
        {
            _viewData = viewData;
            ServiceResource.TryGetWidgetPrefabForKey("ui/picture", out _gameObject);
        }

        public override WidgetViewBase CreateView()
        {
            if (_pictureView == null)
            {
                _pictureView = WidgetCanvas.GetWidgetPool().GetFromPool<WidgetPictureView>(_gameObject);
                if (ServiceResource.TryGetTextureForKey(_viewData.Picture, out var texture))
                {
                    _pictureView.Image.material.mainTexture = texture;
                }

                _pictureView.Init();
            }

            return _pictureView;
        }

        public override void ClearView()
        {
            if (_pictureView == null)
            {
                _pictureView.Clear();
                WidgetCanvas.GetWidgetPool().ReturnToPool(_pictureView);
                _pictureView = null;
            }
        }
    }
}
