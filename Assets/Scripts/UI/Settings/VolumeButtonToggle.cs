using UnityEngine;
using UnityEngine.UI;

public class VolumeButtonToggle : MonoBehaviour
{
    private const string VolumePrefKey = "MasterVolume";

    [Header("UI")]
    public Button volumeButton;
    public Image iconImage;

    [Header("Icons")]
    public Sprite volumeOnIcon;
    public Sprite volumeOffIcon;

    private bool isVolumeOn = true;

    void Start()
    {
        // Load saved volume state (default = ON)
        isVolumeOn = PlayerPrefs.GetInt(VolumePrefKey, 1) == 1;

        ApplyVolume();
        UpdateIcon();

        // Assign button click
        volumeButton.onClick.AddListener(ToggleVolume);
    }

    public void ToggleVolume()
    {
        isVolumeOn = !isVolumeOn;

        ApplyVolume();
        SaveVolume();
        UpdateIcon();
    }

    private void ApplyVolume()
    {
        AudioListener.volume = isVolumeOn ? 1f : 0f;
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetInt(VolumePrefKey, isVolumeOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void UpdateIcon()
    {
        if (iconImage != null)
            iconImage.sprite = isVolumeOn ? volumeOnIcon : volumeOffIcon;
    }
}