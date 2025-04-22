using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CountDownText : MonoBehaviour
{
    private TextMeshProUGUI reviveText;
    private int timeRevive = 5;
    void Start()
    {
        reviveText = GetComponent<TextMeshProUGUI>();
        CountDownRevive();
    }

    private async void CountDownRevive() {
        while (timeRevive >= 0) {
            reviveText.text = timeRevive.ToString();
            await Task.Delay(1000);
            timeRevive -= 1;
        }
    }
}
