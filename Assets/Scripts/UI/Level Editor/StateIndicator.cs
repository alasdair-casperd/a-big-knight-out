
using TMPro;
using UnityEngine;

namespace UI
{
    public class StateIndicator : MonoBehaviour
    {
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                SetNumber(value);
            }
        }
        private int _Number;

        [SerializeField]
        private TextMeshPro indicatorText;

        private void SetNumber(int i)
        {
            indicatorText.text = i.ToString();
        }
    }
}