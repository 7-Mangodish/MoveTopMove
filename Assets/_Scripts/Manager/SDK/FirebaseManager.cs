using Firebase;
using Firebase.Analytics;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance {get { return instance;}}

    private DatabaseReference databaseReference;
    private FirebaseApp app;

    public bool isInit;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        isInit = false;
    }


    public void InitFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {

                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                app = Firebase.FirebaseApp.DefaultInstance;
                //Debug.Log("FireBase is set up");

                FirebaseAnalytics.LogEvent("test_event");
                isInit = true;
            }
            else {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }


    // Save and Load Data
    public async void SavePlayerData(string playerId, PlayerData data) {
        DataSnapshot dataSnapshot = await databaseReference.Child("players").Child(playerId).GetValueAsync();
        if (!dataSnapshot.Exists) {
            Debug.Log(playerId + " not exists");
            return;
        }

        string json = JsonUtility.ToJson(data);
        await databaseReference.Child("players").Child(playerId).SetRawJsonValueAsync(json);
        Debug.Log("PlayerData is saved");
    }

    public async Task<PlayerData> LoadPlayerData(string playerId) {
        DataSnapshot dataSnapshot = await databaseReference.Child("players").Child(playerId).GetValueAsync();
        if (!dataSnapshot.Exists) {
            Debug.Log(playerId + " not exists");
            return null;
        }
        string json = dataSnapshot.GetRawJsonValue();
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        return data;

    }
    public class PlayerData {
        public string name;
        public int coin;
        public SkinDataManager.SkinData skinData;
    }
}
