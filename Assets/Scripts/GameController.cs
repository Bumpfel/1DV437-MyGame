using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour {

    public GameObject m_Menu;
    public GameObject m_Player;
    private bool m_Paused = false;

    void Start() {
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    // private void MenuNavigation() {
    //     if(!m_Paused) {
    //         TogglePause();
    //     }
    //     else {
    //         print("is paused");
    //         GameObject[] children = m_Menu.GetComponentsInChildren<GameObject>();
    //         print("Found children: " + children.Length);


    //         GameObject[] menus = m_Menu.GetComponentsInChildren<GameObject>();
    //         print("Found " + menus.Length);
    //         foreach(GameObject menu in menus) {
    //             print(menu.name);
    //             if(menu.activeInHierarchy) {
    //                 print("is active");
    //                 if(isMainMenu(menu)) {
    //                     print("is main menu. unpausing");
    //                     TogglePause();
    //                 }
    //                 else {
    //                     print("is not main menu. hiding");
    //                     menu.SetActive(false);
    //                 }
    //             }
    //             else if(isMainMenu(menu)) {
    //                 print("goin back to main menu");
    //                 menu.SetActive(true);
    //             }
    //             print("---------------------");
    //         }
    //     }
    // }


    public void TogglePause() {
        m_Paused = !m_Paused;
        m_Player.GetComponent<PlayerMovement>().m_GamePaused = m_Paused;
        m_Player.GetComponent<PlayerAttack>().m_GamePaused = m_Paused;
        Time.timeScale = m_Paused ? 0 : 1;
        m_Menu.SetActive(m_Paused);
    }

    // private bool isMainMenu(GameObject menu) {
    //     return menu.name == "MainMenu";
    // }
}