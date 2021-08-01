using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class PropsPlacer : MonoBehaviour
{
    [MenuItem("Tools/PlaceProps")]
    // Start is called before the first frame update
    static void Create()
    {
        float minX = -4.3f;
        float minZ = -6f;
        float maxX = 6f;
        float maxZ = 9f;
        float minBorderZ = -4f;
        float maxBorderZ = 4f;

        Dictionary<string, int> propsEverywhere = new Dictionary<string, int>();
        Dictionary<string, int> propsOutside = new Dictionary<string, int>();


        
        propsEverywhere.Add("Assets/Prefabs/Props/BigBush.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_animal_skull.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_board.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_Board_group.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_bone_skull_group 2.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_Bone_skull_group skull 1.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_brokenboard.prefab", 3);
        // 
        propsEverywhere.Add("Assets/Prefabs/Props/west_flint_L.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_flint_M.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_flint_S.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_grass_S.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_Ribs_bones_group.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_single_bone.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_single_bones_group_1.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_skull.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_spike_bush.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_spikebones.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_tree.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/west_Wheel.prefab", 3);
        propsEverywhere.Add("Assets/Prefabs/Props/Wheel broke.prefab", 3);


        propsOutside.Add("Assets/Prefabs/Props/TwoCactuses.prefab", 3);
        propsOutside.Add("Assets/Prefabs/Props/west_cactus_spike.prefab", 3);

        //var parent = Selection.activeObject


        foreach (KeyValuePair<string, int> entry in propsEverywhere)
        {
            //entry.Value
            //entry.Key
            var numObjects = Random.Range(0, entry.Value+1);
            for (int i = 0; i < numObjects; i++)
            {
                float xObj = Random.Range(minX, maxX);
                float zObj = Random.Range(minZ, maxZ);
                UnityEngine.Object ngObj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(entry.Key, typeof(UnityEngine.Object));
                Debug.Log(entry.Key);
                Instantiate(ngObj, new Vector3(xObj, 0, zObj), Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));
            }
        }

        foreach (KeyValuePair<string, int> entry in propsOutside)
        {
            //entry.Value
            //entry.Key
            var numObjects = Random.Range(0, entry.Value + 1);
            for (int i = 0; i < numObjects; i++)
            {
                float xObj = Random.Range(minX, maxX);
                float zObj = 0;
                if (Random.value < 0.5f)
                    zObj = Random.Range(minZ, minBorderZ);
                else
                    zObj = Random.Range(maxBorderZ, maxZ);

                UnityEngine.Object ngObj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(entry.Key, typeof(UnityEngine.Object));
                Debug.Log(entry.Key);
                Instantiate(ngObj, new Vector3(xObj, 0, zObj), Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));
            }
        }


        Debug.Log("Hi");
    }
}
