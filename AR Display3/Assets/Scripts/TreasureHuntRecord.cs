using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TreasureHuntRecord {
    public string deviceId;
    public List<object> foundCoins;
	
    public TreasureHuntRecord()
    {

    }

    public TreasureHuntRecord(string deviceId, List<object> foundCoins)
    {
        this.deviceId = deviceId;
        this.foundCoins = foundCoins;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        result["deviceId"] = this.deviceId; 
        result["foundCoins"] = this.foundCoins;

        return result;
    }

    public bool HasCoin(string coin)
    {
        return this.foundCoins.Contains(coin);
    }

    public void AddCoin(string coin)
    {
        this.foundCoins.Add(coin);
    }
}
