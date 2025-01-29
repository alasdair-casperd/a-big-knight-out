
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(SidebarTool))]
    public class SidebarToolSelector: Selector
    {
        public Image icon;
        public GameObject selectionHighlight;

        private SidebarTool sidebarTool;

        protected override void UpdateVisuals()
        {
            if (Selected)
            {
                selectionHighlight.SetActive(true);
                icon.color = Color.white;
            }
            else
            {
                selectionHighlight.SetActive(false);
                icon.color = Color.gray;
            }
        }

        protected override void OnSelect()
        {
            if (sidebarTool == null) sidebarTool = GetComponent<SidebarTool>();
            sidebarTool.levelEditor.SidebarTool = sidebarTool;
            sidebarTool.OnSelect.Invoke();
        }

        protected override void OnDeselect()
        {
            if (sidebarTool == null) sidebarTool = GetComponent<SidebarTool>();
            sidebarTool.OnDeselect.Invoke();
        }
  }
}
