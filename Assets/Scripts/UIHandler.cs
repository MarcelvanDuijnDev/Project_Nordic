using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [Header("Text / Sliders")]
    [SerializeField] private TextMeshProUGUI _Text_Time = null;
    [SerializeField] private TextMeshProUGUI _Text_TimeMultiplier = null;
    [SerializeField] private TextMeshProUGUI _Text_Temp = null;
    [SerializeField] private Slider _Slider_Time, _Slider_TimeMultiplier, _Slider_Weather, _Slider_Temp = null;

    [Header("Menu Positions / Object")]
    [SerializeField] private Transform _MenuObj = null;
    [SerializeField] private Transform _Pos1 = null;
    [SerializeField] private Transform _Pos2 = null;

    [Header("Settings")]
    [SerializeField] private float _MenuSpeed = 2;

    [Header("Audio")]
    [SerializeField] private Slider _Slider_Volume;
    [SerializeField] private TextMeshProUGUI _Text_Volume;

    private WeatherSystem _WeatherSystemScript;
    private bool _ShowMenu;

    private void Start()
    {
        _WeatherSystemScript = GetComponent<WeatherSystem>();
    }

    void Update()
    {
        //Text
        float currenttime = _WeatherSystemScript.Get_CurrentTime * 3600;
        _Text_Time.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(currenttime / 3600), Mathf.Floor((currenttime / 60) % 60), currenttime % 60);
        _Text_Temp.text = _WeatherSystemScript.Get_Temperature.ToString("0.00");
        _Text_TimeMultiplier.text = "x" + _WeatherSystemScript.Get_TimeMultiplier.ToString("0");

        //Menu Position
        if (_ShowMenu)
            _MenuObj.transform.position = Vector3.Lerp(_MenuObj.transform.position, _Pos2.position, _MenuSpeed * Time.deltaTime);
        else
            _MenuObj.transform.position = Vector3.Lerp(_MenuObj.transform.position, _Pos1.position, _MenuSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
            _ShowMenu = !_ShowMenu;

        //Sliders
        _WeatherSystemScript.Get_CurrentTime = _Slider_Time.value;
        _WeatherSystemScript.Get_TimeMultiplier = _Slider_TimeMultiplier.value;
        _WeatherSystemScript.Get_Weather = _Slider_Weather.value;
        _WeatherSystemScript.Get_Temperature = _Slider_Temp.value;

        //Audio
        AudioHandler.AUDIO.AudioVolume = _Slider_Volume.value;
        _Text_Volume.text = (_Slider_Volume.value * 100).ToString("0");
    }

    public void ShowHide_Menu()
    {
        _ShowMenu = !_ShowMenu;
    }

    public void Button_Quit()
    {
        Application.Quit();
    }
}
