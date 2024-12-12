using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public static class Utilities
    {
        public static bool MouseOverUI
        {
            get
            {
                return EventSystem.current.IsPointerOverGameObject(0);
            }
        }
    }
}