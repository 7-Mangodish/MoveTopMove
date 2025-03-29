using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransparentObstacle : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private LayerMask obstacleLayerMask;
    private List<MeshRenderer> listObstacleRenderer = new List<MeshRenderer>();
    private Dictionary<MeshRenderer, Material> dictOriginalMaterial = new Dictionary<MeshRenderer, Material>(); 

    // Update is called once per frame
    void Update()
    {
        DoCameraBehindBuilding();
    }

    void DoCameraBehindBuilding() {
        Physics.Linecast(this.transform.position, Camera.main.transform.position, out RaycastHit hit);
        Debug.DrawRay(transform.position, Camera.main.transform.position - this.transform.position, Color.red); // Debug ray
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Obstacle")) {

            MeshRenderer mesh = hit.collider.gameObject.GetComponent<MeshRenderer>();
            if (mesh != null && !listObstacleRenderer.Contains(mesh)) {
                if (!dictOriginalMaterial.ContainsKey(mesh))
                    dictOriginalMaterial.Add(mesh, mesh.material);
                mesh.material = transparentMaterial;

            }

        }
        else {
            if (listObstacleRenderer.Count > 0) {
                foreach (MeshRenderer mesh in listObstacleRenderer) {
                    mesh.material = dictOriginalMaterial[mesh];
                }
                listObstacleRenderer.Clear();
            }
        }
    }
}