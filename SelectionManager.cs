using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    public LayerMask selectionMask;

    public HexGrid hexGrid;

    List<Vector3Int> neighbours = new List<Vector3Int>();
    

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    public void HandleClick(Vector3 mousePosition)
    {
        GameObject result;
        if (FindTarget(mousePosition, out result))
        {
            Hex selectedHex = result.GetComponent<Hex>();

            selectedHex.DisableHighlight();

            foreach (Vector3Int neighbour in neighbours)
            {
                hexGrid.GetTileAt(neighbour).DisableHighlight();
            }
            //neighbours = hexGrid.GetNeighboursFor(selectedHex.HexCoords);
            BFSResult bfsresult = GraphSearch.BFSGetRange(hexGrid, selectedHex.HexCoords, 20); //set default movepoints to 20
            neighbours = new List<Vector3Int>(bfsresult.GetRangePositions());

            foreach (Vector3Int neighbour in neighbours)
            {
                hexGrid.GetTileAt(neighbour).EnableHighlight();
            }

            Debug. Log($"Neighbours for {selectedHex.HexCoords} are:");
            foreach (Vector3Int neighbourPos in neighbours)
            {
                Debug.Log (neighbourPos);
            }
        }
    }

    private bool FindTarget(Vector3 mousePosition, out GameObject result)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, 100, selectionMask))  // Added max distance of 100
        {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }

}
