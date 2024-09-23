using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropInstantiator : MonoBehaviour
{
    private Grid _grid;
    [SerializeField] int _daysSinceDug = -1;
    [SerializeField] int _daySinceWaterd = -1;
    [SerializeField] int _seedId = 0;
    [SerializeField] int _growthDays = 0;


    private void OnEnable()
    {
        Managers.Event.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;
    }
    private void OnDisable()
    {
        Managers.Event.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;
    }

    void InstantiateCropPrefabs()
    {
        _grid = GameObject.FindObjectOfType<Grid>();

        Vector3Int cropGridPostion = _grid.WorldToCell(transform.position);

        SetCropGridProperties(cropGridPostion);

        Managers.Resource.Destroy(gameObject);
    }

    void SetCropGridProperties(Vector3Int cropGridPostion)
    {
        if(_seedId >0)
        {
            GridPropertyDetails gridPropertyDetails;

            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPostion.x, cropGridPostion.y);
            if(gridPropertyDetails == null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            gridPropertyDetails.daysSinceDug = _daysSinceDug;
            gridPropertyDetails.daysSinceWatered = _daySinceWaterd;
            gridPropertyDetails.seedItemId = _seedId;
            gridPropertyDetails.growthDays = _growthDays;

            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPostion.x, cropGridPostion.y, gridPropertyDetails);
        }
    }
}
