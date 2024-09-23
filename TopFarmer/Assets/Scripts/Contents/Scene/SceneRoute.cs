using System;
using System.Collections.Generic;
using static Define;

[Serializable]
public class SceneRoute
{
    public Scene fromSceneName;
    public Scene toSceneName;
    public List<ScenePath> scenePathList;
}
