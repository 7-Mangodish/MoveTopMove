using System.Threading.Tasks;
using UnityEngine;

public class Gift : MonoBehaviour
{
    //private void OnCollisionEnter(Collision collision) {
    //    //if(collision.gameObject.CompareTag(GameVariable.ENEMY_TAG) || collision.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
    //    //    this.gameObject.SetActive(false);
    //    //}
    //    if (collision.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
    //        PlayerController.Instance.isUlti = true;
    //        this.gameObject.SetActive(false);
    //    }
    //}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(GameVariable.PLAYER_TAG)) {
            PlayerController.Instance.isUlti = true;
            PlayerController.Instance.playerAttackArea.transform.localScale += new Vector3(.25f, .25f, .25f);
            this.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag(GameVariable.ENEMY_TAG)) {
            EnemyController enemyControl = other.gameObject.GetComponent<EnemyController>();
            enemyControl.isUlti = true;
            enemyControl.enemyAttackArea.transform.localScale += new Vector3(.25f, .25f, .25f);
            this.gameObject.SetActive(false);
        }
    }
    private async void OnEnable() {
        await Task.Delay(1000);
        Destroy(this.gameObject);
    }
}
