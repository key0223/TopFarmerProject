using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    Coroutine _coDrop = null;

    public Vector3Int CellPos { get; private set; }
    public float _gravity = -9.8f; // 중력 가속도
    public float _bounceDamping = 0.6f; // 바운스 감쇠율
    public float _initialForceX = 2f; // 초기 x축 힘
    public float _initialForceY = 5f; // 초기 y축 힘
    public float _groundLevel = 0f; // 바닥의 y축 위치


    private Vector2 _velocity; // 속도

    //private void Start()
    //{
    //    _velocity = new Vector2(Random.Range(-_initialForceX, _initialForceX), Random.Range(_initialForceY / 2, _initialForceY));
    //    _coDrop = StartCoroutine("CoStartItemDrop");
    //}

    public void OnDropItem(Vector3Int cellPos)
    {
        transform.position = Managers.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.0f);

        int randLevel = Random.Range(-1, 0);
        _groundLevel = Managers.Map.CurrentGrid.CellToWorld(cellPos).y+randLevel;
        _velocity = new Vector2(Random.Range(-_initialForceX, _initialForceX), Random.Range(_initialForceY / 2, _initialForceY));

        _coDrop = StartCoroutine("CoStartItemDrop");
    }

    IEnumerator CoStartItemDrop()
    {
        while (true)
        {
            // 속도에 중력 적용
            _velocity.y += _gravity * Time.deltaTime;

            // 위치 업데이트
            Vector3 position = transform.position;
            position += (Vector3)_velocity * Time.deltaTime;

            // 바닥에 닿으면 바운스
            if (position.y <= _groundLevel)
            {
                position.y = _groundLevel;
                _velocity.y = -_velocity.y * _bounceDamping; // y축 속도를 반전하고 감쇠

                // 바운스가 충분히 작아지면 정지
                if (Mathf.Abs(_velocity.y) < 0.1f)
                {
                    _velocity.y = 0;
                    break;
                }
            }

            transform.position = position;
            
            yield return null; // 다음 프레임까지 대기
        }

        // 위치를 다시 설정
        CellPos = Managers.Map.CurrentGrid.WorldToCell(transform.position);
        //transform.position = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
    }

}