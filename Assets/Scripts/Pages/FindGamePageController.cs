using System;
using FPS.UI.Pages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FindGamePageController : BasePage
{
    public BasePage mainMenuPage;

    private void Awake()
    {
        var doc = gameObject.GetComponent<UIDocument>();
        
        doc.rootVisualElement.Q<Button>("Back").clicked += () =>
        {
            mainMenuPage.gameObject.SetActive(true);
            gameObject.SetActive(false);
        };
        doc.rootVisualElement.Q<Button>("Refresh").clicked += () => { };
    }
}
