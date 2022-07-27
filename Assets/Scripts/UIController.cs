using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private TMP_Text _textScore, _textWin;

    private void OnEnable()
    {
        PlayerController.win += ShowWinUI;
        PlayerController.death += ShowGameOverUI;
        PlayerController.getCoin += UpdateUI;
    }
    private void OnDisable()
    {
        PlayerController.win -= ShowWinUI;
        PlayerController.death -= ShowGameOverUI;
        PlayerController.getCoin -= UpdateUI;
    }
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    private void UpdateUI(int coins)
    {
        _textScore.text = $"Coins collected: {coins}";
    }
    private void ShowGameOverUI()
    {
        _restartButton.SetActive(true);
    }
    private void ShowWinUI()
    {
        _textWin.gameObject.SetActive(true);
        _restartButton.SetActive(true);
    }
}
