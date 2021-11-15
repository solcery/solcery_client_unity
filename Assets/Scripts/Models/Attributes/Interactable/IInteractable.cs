using System;

namespace Solcery.Models.Attributes.Interactable
{
    public interface IInteractable
    {
        Action OnClick { get; set; }
        void SetInteractable(bool value);
    }
}