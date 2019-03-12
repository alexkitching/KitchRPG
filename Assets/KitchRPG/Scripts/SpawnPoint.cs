using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : Point
{
    [SerializeField]
    private string PrefabPath;

    private void Awake()
    {
        if (string.IsNullOrEmpty(PrefabPath) == false)
        {
            GameObject prefab = ResourceManager.LoadResource<GameObject>(PrefabPath, AssetType.Prefab);

            if (prefab)
                Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endif
}
