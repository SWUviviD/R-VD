#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : MonoBehaviour
{
    private int selectedObjectIndex = -1;
    private int ID;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;
}

#endif
