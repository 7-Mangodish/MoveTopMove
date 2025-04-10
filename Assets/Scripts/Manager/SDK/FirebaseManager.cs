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
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    //public enum CollectionName {
    //    players,
    //}

    //public CollectionName collectionName;
    void Start()
    {
        CheckFirebase();
    }

    void Update()
    {
        
    }

    private void CheckFirebase() {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {

                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

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

    //public async Task<string> GetPlayerName(string playerId) {
    //    PlayerData data = await LoadPlayerData(playerId);
    //    return data.name;
    //}

    //public async void SetPlayerName(string playerId, string name) {
    //    PlayerData data = await LoadPlayerData(playerId);
    //    Debug.Log(data.name);
    //}


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
