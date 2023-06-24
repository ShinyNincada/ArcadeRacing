using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverInfo {
    public int PlayerNumber = 0;
    public string name = "";
    public int carUniqueId = 0;
    public bool isAI = false;
    public int lastRacePosition = 0;
    public int championPoints = 0;
    

    public DriverInfo(int PlayerNumber, string name, int carUniqueId, bool isAI){
        this.PlayerNumber = PlayerNumber;
        this.name = name;
        this.carUniqueId = carUniqueId;
        this.isAI = isAI;
    }
}
