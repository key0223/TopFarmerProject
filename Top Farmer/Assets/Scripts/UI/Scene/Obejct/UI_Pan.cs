using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Pan : UI_Base
{
    enum Images
    {
        PanTimerImage,
        FrameImage,
        StateImage
    }

    int _stoveId;
    float _time;
    public float RemainingTime { get; private set; }


    OvenController _ovenCotroller;
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        GetImage((int)Images.FrameImage).gameObject.SetActive(false);
    }

    public void OnCook(int stoveId,float time)
    {
        #region 이벤트 구독
        Managers.Time.MinutePassedRegisterd -= UpdateGauge;
        Managers.Time.MinutePassedRegisterd += UpdateGauge;

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Oven ovenUI = gameSceneUI.OvenUI;

        ovenUI.Stoves[stoveId].CompleteCookRegistered -= Destroy;
        ovenUI.Stoves[stoveId].CompleteCookRegistered += Destroy;
        #endregion

        GetImage((int)Images.FrameImage).gameObject.SetActive(false);

        _ovenCotroller = FindObjectOfType<OvenController>();

        _stoveId = stoveId;
        _time = time;
        RemainingTime = _time;
        GetImage((int)Images.PanTimerImage).fillAmount = 0;

        _ovenCotroller.SetStateMark(StateMarkState.ProgressingMark);

    }

    void UpdateGauge()
    {
        if(RemainingTime>0)
        {
            RemainingTime -= 1;
            GetImage((int)Images.PanTimerImage).fillAmount = 1 - (RemainingTime / _time);

            if(RemainingTime <= 0)
            {
                // 요리를 완료합니다.
                Managers.Time.MinutePassedRegisterd -= UpdateGauge;
                GetImage((int)Images.FrameImage).gameObject.SetActive(true);

                UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
                UI_Oven ovenUI = gameSceneUI.OvenUI;
                ovenUI.Stoves[_stoveId].State = StoveState.Completed;

                _ovenCotroller.SetStateMark(StateMarkState.ExclamationMark);
            }
        }
    }

    public void Destroy()
    {
        _ovenCotroller.DestroyStateMark();
        
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Oven ovenUI = gameSceneUI.OvenUI;
        ovenUI.Stoves[_stoveId].CompleteCookRegistered -= Destroy;
        Managers.Resource.Destroy(gameObject);
    }

}
