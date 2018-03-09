using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class Coin : DefaultTrackableEventHandler
{
    [SerializeField]
    private GameObject particle;

    [SerializeField]
    private GameObject audioSource;


    [SerializeField]
    private GameObject renderOnTo;

    [SerializeField]
    private float bingTimeout = 1;

    override protected void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        ShowBling();
    }

    private void ShowBling()
    {
        GameObject newParticle = Instantiate(particle, renderOnTo.transform.position, transform.rotation);
        newParticle.transform.parent = mTrackableBehaviour.transform;

        //Play Audio
        mTrackableBehaviour.GetComponent<AudioSource>().Play();

        newParticle.transform.localScale = new Vector3(80f, 80f, 80f);
        newParticle.SetActive(true);

        //Destroy(newParticle, bingTimeout);

        RegisterCoinFound();
    }

    private void RegisterCoinFound()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        string foundCoin = mTrackableBehaviour.TrackableName;

        Debug.Log("Device ID:" + deviceId);
        Debug.Log("Found Coin: " + foundCoin);

        //Pull the user's record.
        FirebaseDatabase db = FirebaseDatabase.DefaultInstance;

        db.GetReference("TreasureHunt").OrderByChild("deviceId")
        .EqualTo(deviceId)
        .LimitToFirst(1)
        .GetValueAsync().ContinueWith
        (
            task =>
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Records found: " + snapshot.ChildrenCount);
                if (snapshot.ChildrenCount > 0)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        Debug.Log("Found Record.");
                        Debug.Log(childSnapshot.GetRawJsonValue());
                        string _deviceId = childSnapshot.Child("deviceId").Value.ToString();

                        List<object> foundCoins = new List<object>();
                        if (childSnapshot.Child("foundCoins").Exists)
                        {
                            foundCoins = (List<object>)childSnapshot.Child("foundCoins").GetValue(false);
                        }

                        TreasureHuntRecord record = new TreasureHuntRecord(_deviceId, foundCoins);
                        if (!record.HasCoin(foundCoin))
                        {
                            Debug.Log("Adding Coin: " + foundCoin);
                            record.AddCoin(foundCoin);
                            Dictionary<string, object> recordValues = record.ToDictionary();
                            Dictionary<string, object> childUpdates = new Dictionary<string, object>();
                            childUpdates["/" + deviceId] = recordValues;

                            Debug.Log("Writing to: " + "/" + deviceId);
                            db.GetReference("TreasureHunt").UpdateChildrenAsync(childUpdates);

                        }
                        //Only process 1 record (should ONLY have 1 record anyway!)
                        break;
                    }
                }
                else
                {
                    Debug.LogError("Unable to find record with Device ID: " + deviceId);
                }
            }
        );

        FirebaseDatabase.DefaultInstance
        .GetReference("TreasureHunt")
        .RunTransaction(FoundCoinTransaction)
      .ContinueWith(task => {
          if (task.Exception != null)
          {
              
          }
          else if (task.IsCompleted)
          {
              
          }
      });

        /*
       FirebaseDatabase.DefaultInstance
          //.GetReference("TreasureHunt")
          .GetReference("users")
          .GetValueAsync().ContinueWith(user => {
              //user.Result.Child
              DataSnapshot snapshot = user.Result;

              Debug.Log(snapshot.ChildrenCount);

              foreach (var childSnapshot in snapshot.Children)
              {
                  Debug.Log("email:" + childSnapshot.Child("email").Value.ToString());
              }
                  
          });
          */
    }

    TransactionResult FoundCoinTransaction(MutableData mutableData)
    {
        return TransactionResult.Abort();
    }
}