#if UNITY_EDITOR
using UnityEngine;
[ExecuteInEditMode]
public class VertexColorUpdate : MonoBehaviour
{
    [SerializeField]
    private MeshFilter[] meshes;
    int numberOfChildren;
    public float sampleDistance =0.1F;
    private void Start()
    {
        GetMeshes();
    }

    private void GetMeshes()
    {
        if (Application.isEditor)
        {
            meshes = new MeshFilter[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                meshes[i] = transform.GetChild(i).GetComponent<MeshFilter>();
            }
            numberOfChildren = transform.childCount;
        }
    }

    void Update()
    {
        if (Application.isEditor)
        {
            if (numberOfChildren != transform.childCount)
            {
                GetMeshes();
            }
            for (int i = 0; i < meshes.Length; i++)
            {
                if (i > 0)
                {
                    meshes[i].sharedMesh.colors = meshes[0].sharedMesh.colors;
                }
                meshes[i].transform.localPosition = new Vector3(0, i * sampleDistance, 0);
            }
        }
        
    }
}
#endif