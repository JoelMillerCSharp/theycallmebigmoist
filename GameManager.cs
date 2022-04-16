using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    private int sceneIndex;
    public bool paused;

    //UI Status Bars
    private PlayerStatusBars dashBar;
    private PlayerStatusBars rollBar;
    private PlayerStatusBars healthBar;
    private PlayerStatusBars gunAmmoBar;
    private PlayerStatusBars shirukenAmmoBar;
    private Text killCounterText;
    private GameObject pauseMenu;
    private GameObject winMenu;
    private GameObject loseMenu;
    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;

        //Getting all necessary references, such as the player, UI, etc.
        player = GameObject.Find("Herbert").GetComponent<Player>();
        dashBar = GameObject.Find("DashBar").GetComponent<PlayerStatusBars>();
        rollBar = GameObject.Find("RollBar").GetComponent<PlayerStatusBars>();
        healthBar = GameObject.Find("HealthBar").GetComponent<PlayerStatusBars>();
        gunAmmoBar = GameObject.Find("GunAmmoBar").GetComponent<PlayerStatusBars>();
        shirukenAmmoBar = GameObject.Find("ShirukenAmmoBar").GetComponent<PlayerStatusBars>();
        killCounterText = GameObject.Find("KillCounter").GetComponent<Text>();
        pauseMenu = GameObject.Find("PauseMenu");
        winMenu = GameObject.Find("WinMenu");
        loseMenu = GameObject.Find("LoseMenu");
    }
    private void Start()
    {
        SetUIBarsToMax();
        killCounterText.text = "Kills: " + player.killCount;
        paused = false;
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
    }

    //Game State Changes
    public void TogglePause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
    public void GameOver()
    {
        loseMenu.SetActive(true);
    }
    public void PlayerWins()
    {
        Time.timeScale = 0f;
        winMenu.SetActive(true);
        player.OnWin();
    }

    //Setting up UI bars, as well as updating on change.
    public void SetUIBarsToMax()
    {
        dashBar.SetToMaxValue(5f);
        rollBar.SetToMaxValue(2f);
        healthBar.SetToMaxValue(player.maxHealth);
        gunAmmoBar.SetToMaxValue(player.maxGunAmmo);
        shirukenAmmoBar.SetToMaxValue(player.maxShirukenAmmo);
    }
    public void UpdateUIBars()
    {
        dashBar.SetToCurrentValue(player.dashCooldownTimer);
        rollBar.SetToCurrentValue(player.rollCooldownTimer);
        healthBar.SetToCurrentValue(player.currentHealth);
        gunAmmoBar.SetToCurrentValue(player.currentGunAmmo);
        shirukenAmmoBar.SetToCurrentValue(player.currentshirukenAmmo);
        killCounterText.text = "Kills: " + player.killCount;
    }

    //Scene Management Stuff From Here.
    public void NextScene()
    {
        sceneIndex++;
        SceneManager.LoadScene(sceneIndex);
    }
    public void PreviousScene()
    {
        sceneIndex--;
        SceneManager.LoadScene(sceneIndex);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadSceneCustom(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#endif
        Application.Quit();
    }
}