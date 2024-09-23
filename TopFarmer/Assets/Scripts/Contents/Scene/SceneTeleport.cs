using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] Scene _scene;
    [SerializeField] Vector3 _scenePositioGoTo = new Vector3();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if(player != null )
        {
            float xPos = Mathf.Approximately(_scenePositioGoTo.x, 0f) ? player.transform.position.x : _scenePositioGoTo.x;
            float yPos = Mathf.Approximately(_scenePositioGoTo.y, 0f) ? player.transform.position.y : _scenePositioGoTo.y;
            float zPos = 0f;

            SceneControllerManager.Instance.FadeAndLoadScene(_scene.ToString(), new Vector3(xPos, yPos, zPos));

            
        }
    }
}
