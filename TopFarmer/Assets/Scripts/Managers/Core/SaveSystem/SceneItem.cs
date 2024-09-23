[System.Serializable]
public class SceneItem
{
    public int itemId;
    public Vector3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}
