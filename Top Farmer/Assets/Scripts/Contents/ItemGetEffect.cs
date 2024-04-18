using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetEffect : ObjectController
{
    private Coroutine _coGetItem;
    private Vector3 _initVelocity;
    private Vector3 _targetTransform;
    private Vector3 _startTransform;
    private float _smoothTime = 0.5f;
    private Color _initColor;
    private bool _itemDropped = false;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _initColor = _sprite.material.color;

    }
    public bool ItemDropped
    {
        get { return _itemDropped; }
        set
        {
            _itemDropped = value;

            if (_itemDropped == true)
            {
                _startTransform = transform.position;
                _targetTransform = _startTransform + new Vector3(0, 0.5f);
                _coGetItem = StartCoroutine("CoGetItem");
            }
        }
    }

    public void Init(Sprite sprite,Vector3 pos,Vector3Int cellPos)
    {
        _sprite.sprite = sprite;
        CellPos = cellPos;
        transform.position = pos;
    }

    IEnumerator CoGetItem()
    {
        Vector3 currentVelocity = _initVelocity;
        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _targetTransform, ref currentVelocity, _smoothTime);

            // ���� ��ġ���� ��� ��ġ���� �Ÿ��� ���� ���İ� ����
            float distance = Vector3.Distance(transform.position, _targetTransform);
            float alpha = Mathf.Clamp01(distance / _smoothTime); // �Ÿ��� ���� 0�� 1 ������ ���� �������� Ŭ����
            
            Color color = _initColor;
            color.a = alpha;
            _sprite.material.color = color;


            if (Vector3.Distance(transform.position, _targetTransform) < 0.01f)
            {
                // �̵��� �Ϸ�Ǹ� ����
                _itemDropped = false;
                _coGetItem = null;
                Managers.Object.Remove(gameObject);
                Managers.Resource.Destroy(gameObject);
                yield break;
            }

            yield return null; // ���� �����ӱ��� ���
          

            
        }
       
    }
}
