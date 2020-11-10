using UnityEngine;

public class GeneralConfiguration : MonoBehaviour
{
    [Header("Visual Items")]

    [SerializeField]
    [Tooltip("Prefab : Used to visualise where an object will be placed on planes - instanced by 'PlacementCursorManager'")]
    public GameObject placementCursorPrefab;
}
