using Newtonsoft.Json.Linq;
using Solcery.React;
using Solcery.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Ui.GameOver
{
    public sealed class GameOverPopup : MonoBehaviourSingleton<GameOverPopup>
    {
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text description;
        [SerializeField] 
        private Button exit;

        public void Open(JObject data)
        {
            canvas.enabled = true;
            exit.onClick.AddListener(OnClick);

            if (data.TryGetValue("title", out string titleText) 
                || data.TryGetValue("Title", out titleText))
            {
                title.text = titleText;
            }
            
            if (data.TryGetValue("description", out string descriptionText) 
                || data.TryGetValue("Description", out descriptionText))
            {
                description.text = descriptionText;
            }
        }

        private void OnClick()
        {
            canvas.enabled = false;
            exit.onClick.RemoveAllListeners();
            UnityToReact.Instance.CallOnGameOverPopupButtonClicked();
        }
    }
}