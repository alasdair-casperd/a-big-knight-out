
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // [RequireComponent(typeof(SidebarTool))]
    public class BottomBarItemSelector: Selector
    {
        public Image selectionHighlight;

        // private SidebarTool sidebarTool;

        protected override void UpdateVisuals()
        {
            selectionHighlight.gameObject.SetActive(Selected || Hovered);
            if (Hovered)
            {
                selectionHighlight.color = Selected ? Color.white : new Color(1, 1, 1, 0.03f);
            }
        }

        protected override void OnSelect()
        {
            // if (sidebarTool == null) sidebarTool = GetComponent<SidebarTool>();
            // sidebarTool.levelEditor.SidebarTool = sidebarTool;
            // sidebarTool.OnSelect.Invoke();
        }

        protected override void OnDeselect()
        {
            // if (sidebarTool == null) sidebarTool = GetComponent<SidebarTool>();
            // sidebarTool.OnDeselect.Invoke();
        }
  }
}
