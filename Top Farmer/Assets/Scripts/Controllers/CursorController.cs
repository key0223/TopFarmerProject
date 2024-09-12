using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D[] _cursorIcons;


    CursorType _cursorType = CursorType.None;
    public CursorType CursorType {  get { return _cursorType; } }

    void Start()
    {
        SetDefaultCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            return;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawLine(mouseWorldPosition, mouseWorldPosition + Vector2.up * 100.0f, Color.red, 0.1f);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 100.0f);

        if (hit.collider != null)
        {

            IRaycastable raycastable = hit.collider.GetComponent<IRaycastable>();

            if (raycastable != null)
            {
                float distanceToPlayer = Vector2.Distance(PlayerController.Instance.transform.position, hit.collider.transform.position);

                if(distanceToPlayer <= 3)
                {
                    // Ŀ�� Ÿ���� ����� ���� Ŀ�� ������ ������Ʈ
                    if (_cursorType != raycastable.GetCursorType())
                    {
                        _cursorType = raycastable.GetCursorType();
                        UnityEngine.Cursor.SetCursor(
                            _cursorIcons[(int)_cursorType],
                            new Vector2(_cursorIcons[(int)_cursorType].width / 5, 0),
                            CursorMode.Auto
                        );
                    }
                }
               
            }
            else
            {
                SetDefaultCursor();
            }
        }
        else
        {
            SetDefaultCursor();
        }
    }

    private void SetDefaultCursor()
    {
        if (_cursorType != CursorType.None)
        {
            _cursorType = CursorType.None;
            UnityEngine.Cursor.SetCursor(
                   _cursorIcons[(int)_cursorType],
                   new Vector2(_cursorIcons[(int)CursorType.None].width / 5, 0),
                   CursorMode.Auto);
        }
    }
}
