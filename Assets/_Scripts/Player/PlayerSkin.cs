using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    public ConfigPlayerSkin configPlayerSkin;
    [Header("-----Hat-----")]
    public Transform hatHolderTrans;
    public GameObject[] arrHatSkin = new GameObject[0];
    //
    [Header("-----Pant-----")]
    public GameObject playerPant;
    public SkinnedMeshRenderer pantSkinMesh;
    //
    [Header("-----Armor----")]
    public Transform armorHolderTrans;
    public GameObject[] arrArmorSkin = new GameObject[0];
    //
    [Header("-----Set------")]
    public Transform wingHolderTrans;
    public Transform tailHolderTrans;
    public SkinnedMeshRenderer playerSkinMesh;
    public PlayerSet[] arrSetSkin = new PlayerSet[0];
    //
    [Header("-----Start----")]
    public Material pantStartMat;
    public Material playerStartMat;

    public void Awake() {
        if (DataManager.Instance.playerData.isSet == false) {
            ChangeSkinHat(DataManager.Instance.playerData.currentHatId);
            ChangeSkinPant(DataManager.Instance.playerData.currentPantId);
            ChangeSkinArmor(DataManager.Instance.playerData.currentArmorId);
        }
        else
            ChangeSkinSet(DataManager.Instance.playerData.currentSetId);
    }

    public void ChangeSkinHat(int id) {
        SetActivePant(true);
        SetActiveSet(false);
        SetPlayerSkinMat(playerStartMat);
        DataManager.Instance.playerData.isSet = false;
        if (arrHatSkin.Length == 0)
            arrHatSkin = new GameObject[configPlayerSkin.listHatObject.Length];
        // Tat het hatSkin
        for(int i = 0; i < arrHatSkin.Length; i++) {
            if (arrHatSkin[i] == null) 
                continue;
            else
                arrHatSkin[i].SetActive(false);
        }
        //Neu id = -1, tuc la chua mua gi
        if (id == 0) return;
        //
        if (arrHatSkin[id] == null)
            arrHatSkin[id] = Instantiate(configPlayerSkin.listHatObject[id], hatHolderTrans);
        else
            arrHatSkin[id].SetActive(true);
    }

    public void ChangeSkinPant(int id) {
        SetActivePant(true);
        SetActiveSet(false);
        SetPlayerSkinMat(playerStartMat);
        DataManager.Instance.playerData.isSet = false;
        if (id == 0) {
            pantSkinMesh.material = pantStartMat;
            return;
        }
        pantSkinMesh.material = configPlayerSkin.listPantMaterial[id];
    }

    public void ChangeSkinArmor(int id) {
        SetActivePant(true);
        SetActiveSet(false);
        SetPlayerSkinMat(playerStartMat);
        DataManager.Instance.playerData.isSet = false;
        //
        if (arrArmorSkin.Length == 0)
            arrArmorSkin = new GameObject[configPlayerSkin.listArmorObject.Length];
        //Tat het armorSkin
        for(int i=0; i< arrArmorSkin.Length; i++) {
            if (arrArmorSkin[i] == null)
                continue;
            else
                arrArmorSkin[i].SetActive(false);
        }
        // neu id = -1, tuc la chua mua gi,
        if(id == 0) return;
        //
        if (arrArmorSkin[id] == null)
            arrArmorSkin[id] = Instantiate(configPlayerSkin.listArmorObject[id], armorHolderTrans);
        else
            arrArmorSkin[id].SetActive(true);
    }

    public void ChangeSkinSet(int id) {
        ChangeSkinHat(0);
        ChangeSkinPant(0);
        ChangeSkinArmor(0);
        SetActivePant(false);
        //
        DataManager.Instance.playerData.isSet = true;
        if (arrSetSkin.Length == 0) {        
            arrSetSkin = new PlayerSet[configPlayerSkin.listPlayerSetObject.Length];
        }
        //
        SetActiveSet(false);
        if (id == 0)
            return;
        if (arrSetSkin[id] != null) {
            if (arrSetSkin[id].hatSetObject != null)
                arrSetSkin[id].hatSetObject.gameObject.SetActive(true);
            if (arrSetSkin[id].armorSetObject != null)
                arrSetSkin[id].armorSetObject.gameObject.SetActive(true);
            if (arrSetSkin[id].tailSetObject != null)
                arrSetSkin[id].tailSetObject.gameObject.SetActive(true);
            if (arrSetSkin[id].wingSetObject != null)
                arrSetSkin[id].wingSetObject.gameObject.SetActive(true);
            if (arrSetSkin[id].matSetObject != null) 
                playerSkinMesh.material = arrSetSkin[id].matSetObject;               
        }
        else {
            arrSetSkin[id] = new PlayerSet();
            if (configPlayerSkin.listPlayerSetObject[id].hatSetObject != null)
                arrSetSkin[id].hatSetObject = Instantiate(configPlayerSkin.listPlayerSetObject[id].hatSetObject, hatHolderTrans);
            if (configPlayerSkin.listPlayerSetObject[id].armorSetObject != null)
                arrSetSkin[id].armorSetObject = Instantiate(configPlayerSkin.listPlayerSetObject[id].armorSetObject, armorHolderTrans);
            if (configPlayerSkin.listPlayerSetObject[id].tailSetObject != null)
                arrSetSkin[id].tailSetObject = Instantiate(configPlayerSkin.listPlayerSetObject[id].tailSetObject, tailHolderTrans);
            if (configPlayerSkin.listPlayerSetObject[id].wingSetObject != null)
                arrSetSkin[id].wingSetObject = Instantiate(configPlayerSkin.listPlayerSetObject[id].wingSetObject, wingHolderTrans);
            if (configPlayerSkin.listPlayerSetObject[id].matSetObject != null) {
                arrSetSkin[id].matSetObject = configPlayerSkin.listPlayerSetObject[id].matSetObject;
                playerSkinMesh.material = arrSetSkin[id].matSetObject;
            }
        }
    }

    public void SetActivePant(bool active) {
        playerPant.gameObject.SetActive(active);
    }

    public void SetActiveSet(bool active) {
        for (int i = 0; i < arrSetSkin.Length; i++) {
            if (arrSetSkin[i] != null) {
                if (arrSetSkin[i].hatSetObject != null)
                    arrSetSkin[i].hatSetObject.gameObject.SetActive(active);
                if (arrSetSkin[i].armorSetObject != null)
                    arrSetSkin[i].armorSetObject.gameObject.SetActive(active);
                if (arrSetSkin[i].tailSetObject != null)
                    arrSetSkin[i].tailSetObject.gameObject.SetActive(active);
                if (arrSetSkin[i].wingSetObject != null)
                    arrSetSkin[i].wingSetObject.gameObject.SetActive(active);
            }
        }
    }

    public void SetPlayerSkinMat(Material mat) {
        playerSkinMesh.material = mat;
    }
}
