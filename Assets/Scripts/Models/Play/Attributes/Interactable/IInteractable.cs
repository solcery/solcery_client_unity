using System;

namespace Solcery.Models.Play.Attributes.Interactable
{
    public interface IInteractable
    {
        Action OnClick { get; set; }
        void SetInteractable(bool value);
    }
}