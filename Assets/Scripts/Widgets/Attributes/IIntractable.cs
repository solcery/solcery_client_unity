using System;

namespace Solcery.Widgets.Attributes
{
    public interface IIntractable
    {
        Action OnClick { get; set; }
        void SetIntractable(bool value);
    }
}