using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class Registry : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        RegisterDevice();
    }

    private void RegisterDevice()
    {
        // Set these values before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://fintech-expo.firebaseio.com/");
        //FirebaseApp.DefaultInstance.SetEditorP12FileName("YOUR-FIREBASE-APP-P12.p12");
        //FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("SERVICE-ACCOUNT-ID@YOUR-FIREBASE-APP.iam.gserviceaccount.com");
        //FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");

        string deviceId = SystemInfo.deviceUniqueIdentifier;
        FirebaseDatabase db = FirebaseDatabase.DefaultInstance;

        Debug.Log("Registering device...");

        db.GetReference("TreasureHunt").OrderByChild("deviceId")
        .EqualTo(deviceId)
        .LimitToFirst(1)
        .GetValueAsync().ContinueWith
        (
            task =>
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Records found: "+ snapshot.ChildrenCount);
                if (snapshot.ChildrenCount==0)
                {
                    Debug.Log("Writing new record.");

                    TreasureHuntRecord record = new TreasureHuntRecord(deviceId, new List<object>());
                    Dictionary<string, object> recordValues = record.ToDictionary();
                    Dictionary<string, object> childUpdates = new Dictionary<string, object>();
                    childUpdates["/" + deviceId] = recordValues;

                    Debug.Log("Writing to: "+ "/" + deviceId);
                    db.GetReference("TreasureHunt").UpdateChildrenAsync(childUpdates);
                }
                else
                {
                    //List<string> value= (List<string>)snapshot.Value;
                    //Debug.Log("Found record with Key:" + value.Key);
                }
            }
        );
    }
}
