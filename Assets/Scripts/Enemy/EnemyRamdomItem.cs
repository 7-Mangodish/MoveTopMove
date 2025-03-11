using UnityEngine;

public class EnemyRamdomItem : MonoBehaviour
{

    private void Awake() {
        Transform weaponHolder = this.transform.Find("mixamorig:RightHandThumb4_end_end_end");
        if(weaponHolder != null) {
            Debug.Log(weaponHolder.transform.position);
        }
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
