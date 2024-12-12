using UnityEngine.UI;

namespace UI
{
    public struct Icon {
        
        // The image asset representing the icon
        public Image image;

        // Can the icon be recoloured?
        // For example, some UI components will tint their icons on hover
        public bool recolorable;
    }
}