using System;
using FPS.UI.Pages;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class MainMenuPageController : BasePage
{
    public BasePage findGamePage;
    
    protected void OnEnable()
    {
        var doc = gameObject.GetComponent<UIDocument>();
        
        doc.rootVisualElement.Q<Button>("StartLocal").clicked += () =>
        {
            // Switch to the TestScene Scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("TestScene");
        };
        doc.rootVisualElement.Q<Button>("FindGame").clicked += () =>
        {
            findGamePage.gameObject.SetActive(true);
            gameObject.SetActive(false);
        };
        doc.rootVisualElement.Q<Button>("Settings").clicked += () => { };
        doc.rootVisualElement.Q<Button>("Exit").clicked += Application.Quit;
    }
}
