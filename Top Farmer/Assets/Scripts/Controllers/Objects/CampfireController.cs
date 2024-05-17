using Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Define;

public class CampfireController : ItemController
{
    public Crafting Crafting { get; private set; }
    private CraftingData _crafingData = null;


    Light2D light;

    float _dayIntensity = 0.3f;
    float _dayRadiusIn = 0.2f;
    float _dayRadiusOut = 3f;
    float _dayStrength = 1;

    float _nightIntensity = 0.5f;
    float _nightRadiusIn = 0.2f;
    float _nightRadiusOut = 5f;
    float _nightStrength = 0.8f;

    float _transitionDuration = 2f;

    float _currentIntensity;
    float _currentRadiusIn;
    float _currentRadiusOut;
    float _currentStrength;

    float _targetIntensity;
    float _targetRadiusIn;
    float _targetRadiusOut;
    float _targetStrength;

    private DayState _state;
    public DayState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateLight();

        }
    }
    protected override void Init()
    {
        base.Init();
        SetItem();
        light = gameObject.GetComponentInChildren<Light2D>();
        Managers.Time.HourPassedRegistered -= UpdateState;
        Managers.Time.HourPassedRegistered += UpdateState;
        State = Managers.Time.State;

    }
    protected  void SetItem()
    {
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(Item.TemplatedId, out itemData);
        _crafingData = (CraftingData)itemData;
        //_sprite.sprite = Managers.Resource.Load<Sprite>($"{_crafingData.iconPath}");

        transform.position = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        //ObjectType = ObjectType.OBJECT_TYPE_INTERACTABLE_OBJECT;
        InteractableObject interactableObject = Util.GetOrAddComponent<InteractableObject>(gameObject);
        interactableObject.InteractableId = InteractableObjectType.INTERACTABLE_OBJECT_NONE;
    }

    void UpdateState()
    {
        State = Managers.Time.State;
    }
    void UpdateLight()
    {
        switch(State)
        {
            case DayState.Dawn:
                {
                    if(_targetIntensity == _nightIntensity)
                        return;

                    _currentIntensity = _dayIntensity;
                    _currentRadiusIn = _dayRadiusIn;
                    _currentRadiusOut = _dayRadiusOut;
                    _currentStrength = _dayStrength;

                    _targetIntensity = _nightIntensity;
                    _targetRadiusIn = _nightRadiusIn;
                    _targetRadiusOut = _nightRadiusOut;
                    _targetStrength = _nightStrength;
                    StartCoroutine("CoUpdateLight");

                }

                break;
            case DayState.Day:
                {
                    _currentIntensity = _nightIntensity;
                    _currentRadiusIn = _nightRadiusIn;
                    _currentRadiusOut = _nightRadiusOut;
                    _currentStrength = _nightStrength;

                    _targetIntensity = _dayIntensity;
                    _targetRadiusIn = _dayRadiusIn;
                    _targetRadiusOut = _dayRadiusOut;
                    _targetStrength = _dayStrength;
                    StartCoroutine("CoUpdateLight");
                }
                break;
            case DayState.Noon:
                {
                    _currentIntensity = _dayIntensity;
                    _currentRadiusIn = _dayRadiusIn;
                    _currentRadiusOut = _dayRadiusOut;
                    _currentStrength = _dayStrength;

                    _targetIntensity = _nightIntensity;
                    _targetRadiusIn = _nightRadiusIn;
                    _targetRadiusOut = _nightRadiusOut;
                    _targetStrength = _nightStrength;
                    StartCoroutine("CoUpdateLight");
                }
                break;
            case DayState.Night:
                {
                    if (_targetIntensity == _nightIntensity)
                        return;

                    _currentIntensity = _dayIntensity;
                    _currentRadiusIn = _dayRadiusIn;
                    _currentRadiusOut = _dayRadiusOut;
                    _currentStrength = _dayStrength;

                    _targetIntensity = _nightIntensity;
                    _targetRadiusIn = _nightRadiusIn;
                    _targetRadiusOut = _nightRadiusOut;
                    _targetStrength = _nightStrength;
                    StartCoroutine("CoUpdateLight");


                }
                break;
        }
    }

    IEnumerator CoUpdateLight()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _transitionDuration;
            float intensity = Mathf.Lerp(_currentIntensity, _targetIntensity, t);
            float radiusIn = Mathf.Lerp(_currentRadiusIn, _targetRadiusIn, t);
            float radiusOut= Mathf.Lerp(_currentRadiusOut, _targetRadiusOut, t);
            float strength = Mathf.Lerp(_currentStrength, _targetStrength, t);

            light.intensity = intensity;
            light.pointLightInnerRadius= radiusIn;
            light.pointLightOuterRadius= radiusOut;
            light.falloffIntensity= strength;

            

            yield return null;
        }
    }
}
