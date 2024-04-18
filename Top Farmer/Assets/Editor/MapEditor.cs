using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR

    //// %(Ctrl), #(Shift), &(alt)
    //[MenuItem("Tools/GenerateMap %#g")]
    //private static void HelloWorld()
    //{
    //    if(EditorUtility.DisplayDialog("Hello World","Create?","Create","Cancel"))
    //    {
    //        new GameObject("Hello World");
    //    }
    //}

    [MenuItem("Tools/GenerateMap")]
    private static void GenerateMap()
    {

        GenerateByPath("Assets/Resources/Map");
        //GenerateByPath("../Common/MapData");
    }

    private static void GenerateByPath(string pathPrefix)
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {
            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
            Tilemap ti = Util.FindChild<Tilemap>(go, "Tilemap_Interaction", true);

            //Collision Map Data
            using (var writer = File.CreateText($"{pathPrefix}/{go.name}_Collision.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }

            // Interactable Map Data
            using(var writer = File.CreateText($"{pathPrefix}/{go.name}_Interaction.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        TileBase tile = ti.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }
    }

   
#endif
}
