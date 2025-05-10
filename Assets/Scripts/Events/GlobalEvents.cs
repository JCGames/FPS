using System;

public static class GlobalEvents
{
    public static event Action<int, int> OnAmmoChanged;

    public static void CallOnAmmoChanged(int totalAmmo, int currentAmmo)
    {
        OnAmmoChanged?.Invoke(totalAmmo, currentAmmo);
    }
}
