using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    [SerializeField] public Define.Scene startingSceneName;
    [SerializeField] List<Define.Scene> _listNonStartingSceneNames = new List<Define.Scene>();

    private IEnumerator Start()
    {
        // Set the initial alpha to start off with a black screen.
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        foreach (Define.Scene sceneName in _listNonStartingSceneNames)
        {
            yield return StartCoroutine(LoadSceneAndSetActive(sceneName.ToString()));

            Managers.Event.CallAfterSceneLoadEvent();
           Managers.Save.RestoreCurrentSceneData();
           Managers.Save.StoreCurrentSceneData();
            yield return SceneManager.UnloadSceneAsync(sceneName.ToString());


        }
        // Start the first scene loading and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // If this event has any subscribers, call it.
        Managers.Event.CallAfterSceneLoadEvent();

       Managers.Save.RestoreCurrentSceneData();

        // Once the scene is finished loading, start fading in.
        StartCoroutine(Fade(0f));
    }

    // This is the coroutine where the 'building blocks' of the script are put together.
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Call before scene unload fade out event
        Managers.Event.CallBeforeSceneUnloadFadeOutEvent();

        // FadeOut
        yield return StartCoroutine(Fade(1f));

        // Store scene data
       Managers.Save.StoreCurrentSceneData();

        // Set player position
        PlayerController.Instance.gameObject.transform.position = spawnPosition;

        //  Call before scene unload event.
        Managers.Event.CallBeforeSceneUnloadEvent();

        // Unload the current active scene.
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Start loading the given scene and wait for it to finish.
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Call after scene load event
        Managers.Event.CallAfterSceneLoadEvent();

       Managers.Save.RestoreCurrentSceneData();

        // FadeIn
        yield return StartCoroutine(Fade(0f));

        // Call after scene load fade in event
        Managers.Event.CallAfterSceneLoadFadeInEvent();
    }
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        UnityEngine.SceneManagement.Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene(newlyLoadedScene);
    }
    private IEnumerator Fade(float finalAlpha)
    {
        // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the CanvasGroup hasn't reached the final alpha yet...
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            yield return null;
        }

        // Set the flag to false since the fade has finished.
        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }


    public void FadeAndLoadScene(string sceneName, Vector3 spwanPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spwanPosition));
        }
    }
}
