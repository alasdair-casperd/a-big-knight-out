
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BrowserItemSelector: Selector
    {
        public Image selectionHighlight;

        protected override void UpdateVisuals()
        {
            selectionHighlight.gameObject.SetActive(Selected || Hovered);
            if (Hovered) selectionHighlight.color = Selected ? Color.white : new Color(1, 1, 1, 0.03f);
        }
    }
}
