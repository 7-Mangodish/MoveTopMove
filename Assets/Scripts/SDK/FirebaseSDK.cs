using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using System.Xml.Serialization;
using System.Threading.Tasks;

public class FirebaseSDK : MonoBehaviour
{
    private FirebaseApp app;
    private DatabaseReference databaseReference;

    public enum TypeAd {
        reward,
    }

    [Header("Realtime-Database")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private string playerId;

    [Header("Analytics")]
    [SerializeField] private TypeAd typeAd;
    [SerializeField] private int reward;
    void Start()
    {
        CheckFirebase();
    }

    private void CheckFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {

                // Set up database
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                // Set up Firebase Analysis
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("FireBase is set up");

                FirebaseAnalytics.LogEvent("test_event");
            }
            else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    #region RealtimeDatabase
    public async void SavePlayerData() {
        string json = JsonUtility.ToJson(playerData);
        try {
            await databaseReference.Child("players").Child(playerId).SetRawJsonValueAsync(json);
            Debug.Log("Player's data is saved");
        }
        catch (System.Exception e) {
            Debug.Log(e);
            return;
        }
    }

    public async void LoadPlayerData() {
        try {
            DataSnapshot playerDataSnapshot = await databaseReference.Child("players").Child(playerId).GetValueAsync();
            string jsonData = playerDataSnapshot.GetRawJsonValue();
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            Debug.Log(jsonData);
            return;

        }
        catch (System.Exception e){
            Debug.Log(e);
            return;
        }
    }

    [System.Serializable]
    public class PlayerData{
        public string name;
        public int coin;
        public SkinDataManager.SkinData skinData;
    }

    #endregion

    #region Analytics
    public void HandlerClickAdEvent() {
        FirebaseAnalytics.LogEvent(
            "ad_watched",
            new Parameter("ad_type", typeAd.ToString()),
            new Parameter("reward_amount", reward)
        );

        Debug.Log($"Type AD: {typeAd}, Take {reward} coin!");
    }
    #endregion

}

