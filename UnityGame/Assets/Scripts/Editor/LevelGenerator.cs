using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class LevelGenerator : MonoBehaviour
{
    [MenuItem("Tools/Load level from file")]
    // Start is called before the first frame update
    static void Create()
    {
        string[][] jagged = readFile("./Assets/Scenes/Levels/loadlevel.txt");

        Dictionary<string, string> prefabsPath = new Dictionary<string, string>();

        prefabsPath.Add("c", "Assets/Prefabs/Gameplay/Barriers/CactusL_2x1.prefab");
        prefabsPath.Add("f", "Assets/Prefabs/Gameplay/Barriers/Fence.prefab");
        prefabsPath.Add("w", "Assets/Prefabs/Gameplay/Barriers/Wall_Rock.prefab");
        prefabsPath.Add("i", "Assets/Prefabs/Gameplay/Barriers/WallInvisble.prefab");
        prefabsPath.Add("k", "Assets/Prefabs/Gameplay/Characters/CatGirl.prefab");
        prefabsPath.Add("p", "Assets/Prefabs/Gameplay/Characters/Player.prefab");
        prefabsPath.Add("s", "Assets/Prefabs/Gameplay/Characters/Shooter.prefab");
        prefabsPath.Add("*", "Assets/Prefabs/Gameplay/Collectables/Collectable Item.prefab");
        prefabsPath.Add("d", "Assets/Prefabs/Gameplay/Dynamites/Dynamite.prefab");
        prefabsPath.Add("b", "Assets/Prefabs/Gameplay/Moving/Box.prefab");
        prefabsPath.Add("g", "Assets/Prefabs/new Meshes/mesh_West_Block.prefab");


        for (int y = 0; y < jagged.Length; y++)
        {
            for (int x = 0; x < jagged[0].Length; x++)
            {
                if (jagged[y][x] != "0")
                {
                    if (jagged[y][x] != "g")
                    {
                        UnityEngine.Object ngObj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(prefabsPath[jagged[y][x]], typeof(UnityEngine.Object));
                        Instantiate(ngObj, new Vector3(-2.5f + x, 0, 1.5f - y), Quaternion.identity);
                    }
                    if (jagged[y][x] != "i")
                    {
                        UnityEngine.Object obj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(prefabsPath["g"], typeof(UnityEngine.Object));
                        Instantiate(obj, new Vector3(-2.5f + x, 0, 1.5f - y), Quaternion.identity);
                    }
                }
            }
        }
        Debug.Log("Hi");
    }

    static string[][] readFile(string file)
    {
        string text = System.IO.File.ReadAllText(file);
        string[] lines = Regex.Split(text, "\r\n");
        int rows = lines.Length;

        string[][] levelBase = new string[rows][];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] stringsOfLine = Regex.Split(lines[i], " ");
            levelBase[i] = stringsOfLine;
        }
        return levelBase;
    }
}
