using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Interactor : MonoBehaviour
{
    public float _interactionDistance = 3f;
    protected bool _isUIOn = false;
    GameObject _currentTarget;
    [SerializeField]
    protected GameObject _uiGameObject;


    private void Awake()
    {
        _uiGameObject.SetActive(false);

    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, _interactionDistance, LayerMask.GetMask("Player"));

        Debug.DrawRay(transform.position,-transform.up*_interactionDistance,Color.red,hit.distance);

        if(hit.collider != null && hit.collider.transform.parent.CompareTag("Player"))
        {
            if (hit.collider.transform.parent.TryGetComponent<PlayerController>(out PlayerController player))
            {
                _currentTarget = hit.collider.gameObject;

                ProcessInput(player);
            }
        }
        else
        {
            _currentTarget = null;
        }
    }
     void ProcessInput(PlayerController player)
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
        {
            Debug.Log("Interaction key pressed");
            ToggleUI(player);
        }
    }

    public void ToggleUI(PlayerController player)
    {
        _isUIOn = !_isUIOn;
        player.PlayerInputDisabled = _isUIOn;
        Time.timeScale = _isUIOn ? 0 : 1;
        _uiGameObject.SetActive(_isUIOn);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -_interactionDistance, 0));
    }
}
