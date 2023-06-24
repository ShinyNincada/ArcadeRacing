using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private int selectedCarID = 1;

    
    public void SelectCar(int carID){
        selectedCarID = carID;
        PlayerPrefs.SetInt("P1_CarID", carID);
        PlayerPrefs.Save();
    }

    public int SelectedCarID{
        get {return selectedCarID; }
    }
}
