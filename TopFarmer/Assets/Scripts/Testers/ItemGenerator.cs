
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private GameObject Parent;
    [SerializeField] private int ItemId;

    public void GenerateItem()
    {
        if (ItemId != 0)
        {
            GameObject newItem = Instantiate(ItemPrefab);
            newItem.transform.SetParent(Parent.transform, false);
            Item item = newItem.GetComponent<Item>();
            item.ItemId= ItemId;
        }
    }
}
