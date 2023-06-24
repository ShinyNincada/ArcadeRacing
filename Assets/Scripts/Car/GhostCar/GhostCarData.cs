using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GhostCarData
{
   [SerializeField]
   List<GhostCarReplay> ghostCarRecord = new List<GhostCarReplay>();

   public void AddDataItem(GhostCarReplay record){
        ghostCarRecord.Add(record);
   }

   public List<GhostCarReplay> GetDataList(){
    return ghostCarRecord;
   }
}
