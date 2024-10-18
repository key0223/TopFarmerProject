using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave 
{
    // string ket = scene name 씬별로 저장
    public Dictionary<string, SceneSave> sceneData;

    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
