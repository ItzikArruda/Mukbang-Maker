using UnityEngine.Events;
using UnityEngine.UI;
// using Snake3D.Managers;
using UnityEngine;

// namespace Snake3D.Interface
// {
    public class PrevNextMenu : MonoBehaviour
    {
        public Button PrevButton;
        public Button NextButton;
        public int MinValue;
        public int MaxValue;
        public int Value;
        public UnityEvent OnValueChanged;

        // Start is called before the first frame update
        void Start()
        {
            PrevButton.onClick.AddListener(new UnityAction(delegate { ChangeValue(-1); }));
            NextButton.onClick.AddListener(new UnityAction(delegate { ChangeValue(1); }));
        }

        // Update is called once per frame
        void Update()
        {
            Value = Mathf.Clamp(Value, MinValue, MaxValue);
            PrevButton.gameObject.SetActive((Value != MinValue));
            NextButton.gameObject.SetActive((Value != MaxValue));
        }

        void ChangeValue(int Amount)
        {
            Value += Amount;
            // ProgressManager.Instance.PlaySelectSound();
            OnValueChanged.Invoke();
        }
    }
// }