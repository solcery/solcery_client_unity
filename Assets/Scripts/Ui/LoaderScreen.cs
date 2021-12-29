using TMPro;
using UnityEngine;

namespace Solcery.Ui
{
    public sealed class LoaderScreen : MonoBehaviour
    {
        private static LoaderScreen _instance;

        [SerializeField]
        private TMP_Text label;
        [SerializeField]
        private Canvas canvas;

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static void Show()
        {
            if (_instance != null)
            {
                _instance.ShowImpl();
            }
        }

        public static void Hide()
        {
            if (_instance != null)
            {
                _instance.HideImpl();
            }
        }

        public static void SetTitle(string text)
        {
            _instance.SetTitleImpl(text);
        }

        private void ShowImpl()
        {
            canvas.enabled = true;
        }

        private void HideImpl()
        {
            canvas.enabled = false;
        }

        private void SetTitleImpl(string text)
        {
            label.text = text;
        }
    }
}