// using System.Collections.Generic;
// using UnityEngine;

// namespace Demo {    
//     public class MovingPlatform : MonoBehaviour
//     {
//         // Transforms defining the path the platform will follow
//         // The first transform should be the platform's initial position
//         public List<Transform> targets;

//         private int stepCount = 0;
        
//         // Move the platform along its path
//         public void Step()
//         {
//             stepCount++;
//             Move(targets[stepCount % targets.Count]);
//         }

//         // Reset the platform
//         public void Reset() 
//         {
//             stepCount = 0;
//             Move(targets[0]);
//         }

//         // Move to a transform
//         private void Move(Transform targetTransform)
//         {
//             var targetPosition = targetTransform.position;
//             targetPosition.y = transform.position.y;
            
//             LeanTween.move(gameObject, targetPosition, 0.1f);

//             Player player = FindAnyObjectByType<Player>();
//             var toPlayer = player.transform.position - transform.position;
//             toPlayer.y = 0;
//             if (toPlayer.magnitude < 0.2)
//             {
//                 player.ManualMove(targetPosition);
//             }
//         }
//     }
// }
