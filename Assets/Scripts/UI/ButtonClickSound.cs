using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    public AudioSource clickSound;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            clickSound.Play();
        });
    }
}