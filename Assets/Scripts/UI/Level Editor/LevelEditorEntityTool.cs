using UnityEngine;
using TMPro;
using System.Linq;

namespace UI
{
    [System.Serializable]
    public class LevelEditorEntityTool: LevelEditorTool
    {
        private EntityType _entityType;
        public EntityType EntityType
        {
            get => _entityType;
            set
            {
                _entityType = value;
                UpdateVisuals();
            }
        }

        public TextMeshProUGUI ToolName;

        private void UpdateVisuals()
        {   
            gameObject.name = EntityType.DisplayName + " Tool";
            ToolName.text = EntityType.DisplayName;
        }
    }
}