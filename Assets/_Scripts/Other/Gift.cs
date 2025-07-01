using UnityEngine;

public class Gift : MonoBehaviour
{
    public SoundController soundController;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            PlayerController.Instance.isUlti = true;
            PlayerController.Instance.playerAttackArea.transform.localScale += new Vector3(.25f, .25f, .25f);
            this.gameObject.SetActive(false);
            soundController.PlaySound(SoundData.SoundName.gift);
        }
        else if (other.gameObject.CompareTag(GameVariable.ENEMY_TAG)) {
            EnemyController enemyControl = other.gameObject.GetComponent<EnemyController>();
            enemyControl.isUlti = true;
            enemyControl.enemyAttackArea.transform.localScale += new Vector3(.25f, .25f, .25f);
            this.gameObject.SetActive(false);
            soundController.PlaySound(SoundData.SoundName.gift);
        }
    }
    //private async void OnEnable() {
    //    await Task.Delay(1000);
    //    Destroy(this.gameObject);
    //}
}
