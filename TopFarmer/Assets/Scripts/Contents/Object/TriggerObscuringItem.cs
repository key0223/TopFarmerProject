using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObscuringItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get all the obscuring Item Fader components on it and its children - and then trigger the fade out
        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // fade in

        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeIn();
            }
        }
    }
}
