using System.Collections.Generic;
using UnityEngine;

namespace Demo {    
    public class PathFollower : MonoBehaviour
    {
        // Transforms defining the path the looper will follow
        // The first transform should be the follower's initial position
        public List<Transform> targets;

        private int stepCount = 0;
        
        // Move the follower along its path
        public void Step()
        {
            stepCount++;
            Move(targets[stepCount % targets.Count]);
        }

        // Reset the follower
        public void Reset() 
        {
            stepCount = 0;
            Move(targets[0]);
        }

        // Move to a transform
        private void Move(Transform targetTransform)
        {
            var targetPosition = targetTransform.position;
            targetPosition.y = transform.position.y;
            
            LeanTween.move(gameObject, targetPosition, 0.1f);
        }
    }
}
