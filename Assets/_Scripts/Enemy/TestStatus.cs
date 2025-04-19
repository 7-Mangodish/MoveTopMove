using UnityEngine;

public class TestStatus : MonoBehaviour
{
    public GameObject enemyStatus;
    private Vector3 standardScale = new Vector3(0.5f, 0.4f, 0.5f);
    private static float standardDistance;

    private Vector3 eulerRot;
    public Vector3 offset = Vector3.up;
    //public float scaleAtDefaultDistance = 1f;
    //public float defaultDistance = 5f; 
    //private Vector3 initialTargetScale;

    private float distance;
    private void Awake() {
        if(this.gameObject.CompareTag("Player"))
            standardDistance = (enemyStatus.transform.position - Camera.main.transform.position).magnitude;
        //initialTargetScale = this.gameObject.transform.localScale ;
    }
    void Start() {
        eulerRot = enemyStatus.transform.rotation.eulerAngles;
    }

    void LateUpdate() {
        enemyStatus.transform.rotation = Quaternion.Euler(eulerRot);//Rotation cua FloatingText 

        Vector3 worldPos = this.transform.position + offset;

        Vector3 camToObj = worldPos - Camera.main.transform.position;
        float distanceOnViewAxis = Vector3.Dot(camToObj, Camera.main.transform.forward);

        //float scale = scaleAtDefaultDistance * (distanceOnViewAxis / defaultDistance);
        //Debug.Log(scale);
        //enemyStatus.transform.localScale *= scale;

        //if (this.transform.localScale != initialTargetScale) {
        //    float scaleFactor = this.transform.localScale.x / initialTargetScale.x;

        //    transform.localScale /= scaleFactor;
        //}

    }
}
