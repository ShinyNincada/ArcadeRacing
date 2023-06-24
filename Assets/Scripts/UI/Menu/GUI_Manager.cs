using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GUI_Manager : MonoBehaviour
{
    [Header("Menu objects")]
    public GameObject StartMenu;
    public GameObject SettingMenu;
    public GameObject ModeMenu;
    public GameObject MapMenu;
    public GameObject CarMenu;
    public GameObject QuitMenu;
    public TMPro.TMP_InputField nameInput;
    public MenuState state = MenuState.MAIN;

    //Variable
    public enum MenuState {
        MAIN, SETTINGS, MODE, MAP, CAR, QUIT
    }

    private void Start() {
        nameInput.text = GameManager.instance.playerName;
    }

    // Update is called once per frame
    public void Update()
    {
        //Khi Esc
        if(Input.GetKeyDown(KeyCode.Escape)){
            switch(state){
                case MenuState.MAIN:
                    ActiveQuit();
                    break;
                
                case MenuState.SETTINGS:
                    DeactiveSettingsMenu();
                    break;
                
                case MenuState.MODE:
                    DeactiveModeMenu();
                    break;
                
                case MenuState.MAP:
                    DeactiveMapMenu();
                    break;

                case MenuState.CAR:
                    DeactiveCarMenu();
                    break;
                
                case MenuState.QUIT:
                    DeactiveQuitMenu();
                    break;
            }
        }
    }


    public void ActiveQuit(){
        ChangeMenu(StartMenu, QuitMenu, MenuState.QUIT);
    }

    public void ActiveSettings(){
       ChangeMenu(StartMenu, SettingMenu, MenuState.SETTINGS);
    }

    public void ActiveMode(){
        ChangeMenu(StartMenu, ModeMenu, MenuState.MODE);
    }

    public void ActiveMap(){
        ChangeMenu(ModeMenu, MapMenu, MenuState.MAP);
    }

    public void ActiveCarMenu(){
        ChangeMenu(MapMenu, CarMenu, MenuState.CAR);
    }

    public void DeactiveCarMenu(){
        ChangeMenu(CarMenu, MapMenu, MenuState.MAP);
    }
    public void DeactiveMapMenu(){
        ChangeMenu(MapMenu, ModeMenu, MenuState.MODE);
    }
    public void DeactiveModeMenu(){
        ChangeMenu(ModeMenu, StartMenu, MenuState.MAIN);
    }
    public void DeactiveSettingsMenu(){
        ChangeMenu(SettingMenu, StartMenu, MenuState.MAIN);
    }
    public void DeactiveQuitMenu(){
        ChangeMenu(QuitMenu, StartMenu, MenuState.MAIN);
    }

    public void ChangeMenu(GameObject currentMenu, GameObject nextMenu, MenuState newState){
        currentMenu.SetActive(false);
        nextMenu.SetActive(true);
        state = newState;
    }

    public void OnClickTween(GameObject tweeningObject){
        tweeningObject.transform.DOScale(1.1f, 0.1f);
    }

    public void OnBlurTween(GameObject tweeningObject){
        tweeningObject.transform.DOScale(1f, 0.1f);
    }

    public void SetTimetrial(){
        GameManager.instance.SetGameMode(GameManager.MODE.TIME);
    }
    public void SetSurvivor(){
        GameManager.instance.SetGameMode(GameManager.MODE.SURVIVOR);
    }
    public void SetChampion(){
        GameManager.instance.SetGameMode(GameManager.MODE.RACE);
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void UpdatePlayerName(TMPro.TMP_InputField input){
        GameManager.instance.playerName = input.text;
    }
   
}
