using UnityEngine;
using Cinemachine;
using System;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    //void Start()
    //{
    //    SwitchBoundingShape();
    //}
    private void OnEnable()
    {
        Managers.Event.AfterSceneLoadEvent += SwitchBoundingShape;
    }
    private void OnDisable()
    {
        Managers.Event.AfterSceneLoadEvent -= SwitchBoundingShape;
    }
    /// <summary>
    /// Switch the collider that cinemachine user to define the edges of the screen
    /// </summary>
    private void SwitchBoundingShape()
    {
        // Get the polygon collider on the 'boundsConfiner' gameObject which is used by Cinemachine to prevent the camera going beyond the screen edges 
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();
        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

        // since the confiner bounds have changed need to call this to clear the cache
        cinemachineConfiner.InvalidatePathCache();
    }

}
