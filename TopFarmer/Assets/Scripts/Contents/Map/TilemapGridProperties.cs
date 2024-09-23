using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

//스크립트가 편집 모드와 플레이 모드 모두에서 실행되도록 한다.
[ExecuteAlways] 
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap _tilemap;
    [SerializeField] private SO_GridProperties _gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.Diggable;

    private void OnEnable()
    {
        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            _tilemap = GetComponent<Tilemap>();

            if (_gridProperties != null)
            {
                _gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (_gridProperties != null)
            {
                // 특정 객체의 변경 사항을 저장하도록 알린다.
                EditorUtility.SetDirty(_gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        // 빈 공간 제거
        _tilemap.CompressBounds();

        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if (_gridProperties != null)
            {
                Vector3Int startCell = _tilemap.cellBounds.min;
                Vector3Int endCell = _tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = _tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            _gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
#endif
}