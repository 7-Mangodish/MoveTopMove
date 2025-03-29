using UnityEngine;

public class TransparentCup : MonoBehaviour
{

    [Header("Transparent Obstacle")]
    [SerializeField] private Material transparentMaterial;
    private Material oldMaterial;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Obstacle")) {
            oldMaterial = other.GetComponent<MeshRenderer>().material;
            other.GetComponent<MeshRenderer>().material = transparentMaterial;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Obstacle")) {
            other.GetComponent<MeshRenderer>().material = oldMaterial;
        }
    }
}
