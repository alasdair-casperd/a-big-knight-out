using System.Collections.Generic;
using UnityEngine;

namespace Demo {    
    public class Alternator : MonoBehaviour
    {
        // A list of booleans which the alternator loops through to determine when to show
        public List<bool> appearancePattern;

        // The object which will show and hide
        [SerializeField]
        private GameObject movingObject;

        // The hidden and visible positions for this object
        [SerializeField]
        private Vector3 hiddenPosition;
        [SerializeField]
        private Vector3 visiblePosition;

        private int stepCount = 0;
        
        // Move the follower along its path
        public void Step()
        {
            stepCount++;
            bool visible = appearancePattern[stepCount % appearancePattern.Count];
            var targetPosition = visible ? visiblePosition: hiddenPosition;
            LeanTween.cancel(movingObject);
            LeanTween.moveLocal(movingObject, targetPosition, 0.1f);
        }

        // Reset the follower
        public void Reset() 
        {
            stepCount = -1;
            Step();
        }
    }
}
