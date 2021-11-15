using TMPro;
using UnityEngine;

namespace Solcery.Ui
{
    public sealed class LoaderScreen : MonoBehaviour
    {
        private static LoaderScreen _instance;

        [SerializeField]
        private TMP_Text label;

        private bool _showing = false;
        private float _labelTimer;
        private int _labelIndex;
        private string[] _labelTextList;

        private void Awake()
        {
            _instance = this;

            _showing = true;
            _labelIndex = 0;
            _labelTimer = 1f;
            _labelTextList = new []
            {
                "Loading",
                "Loading.",
                "Loading..",
                "Loading..."
            };

            label.text = _labelTextList[_labelIndex];
        }

        private void Update()
        {
            if (!_showing)
            {
                return;
            }

            _labelTimer -= Time.deltaTime;
            if (!(_labelTimer <= 0f))
            {
                return;
            }
            
            ++_labelIndex;
            _labelIndex = _labelIndex < _labelTextList.Length ? _labelIndex : 0;
            label.text = _labelTextList[_labelIndex];
            _labelTimer = 1f;
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

        private void ShowImpl()
        {
            gameObject.SetActive(true);
        }

        private void HideImpl()
        {
            gameObject.SetActive(false);
        }
    }
}