using UnityEngine;
using UnityEngine.UIElements;

public class Hud : MonoBehaviour
{
    [SerializeField] private UIDocument _document;

    private Label _ammoTxt;
    
    private void Awake()
    {
        _ammoTxt = _document.rootVisualElement.Q<Label>("AmmoTxt");

        GlobalEvents.OnAmmoChanged += OnAmmoChanged;
    }

    private void OnDestroy()
    {
        GlobalEvents.OnAmmoChanged -= OnAmmoChanged;
    }

    private void OnAmmoChanged(int totalAmmo, int currentAmmo)
    {
        _ammoTxt.text = $"{currentAmmo}/{totalAmmo}";
    }
}
