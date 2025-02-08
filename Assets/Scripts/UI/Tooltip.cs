using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Tooltip: MonoBehaviour
    {
        public enum Position
        {
            top, bottom, left, right
        }
        
        public Text text;

        public float VerticalOffset;
        public float HorizontalOffset;

        public void Initialise(string content, Position position)
        {
            text.text = content;
            RectTransform rt = GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            Vector2 targetPivot = new();
            Vector3 targetPosition = new();
            
            switch (position)
            {
                case Position.top:
                    targetPivot = new Vector2(0.5f, 1);
                    targetPosition = new(0, rt.rect.height + VerticalOffset, 0);
                    break;
                case Position.bottom:
                    targetPivot = new Vector2(0.5f, 0);
                    targetPosition = new(0, -rt.rect.height - VerticalOffset, 0);
                    break;
                case Position.left:
                    targetPivot = new Vector2(0, 0.5f);
                    targetPosition = new(-rt.rect.width - HorizontalOffset, 0, 0);
                    break;
                case Position.right:
                    targetPivot = new Vector2(1, 0.5f);
                    targetPosition = new(rt.rect.width + HorizontalOffset, 0, 0);
                    break;
            }

            rt.anchorMax = targetPivot;
            rt.anchorMin = targetPivot;
            rt.pivot = targetPivot;
            
            rt.anchoredPosition = targetPosition;
        }
    }
}