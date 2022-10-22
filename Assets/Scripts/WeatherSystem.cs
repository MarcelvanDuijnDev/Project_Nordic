using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0, 24)] private float _CurrentTime_Hours = 6;
    [SerializeField] [Range(0, 100)] private float _BadWeather = 0;
    [SerializeField] [Range(-40, 40)] private float _Temperature = 0;
    [SerializeField] [Range(0, 1000)] private float _Time_Multiplier = 1;

    [Header("Snow")]
    [SerializeField] private float _SnowIncreaseSpeed = 0.05f;
    [SerializeField] private float _SnowDecreaseSpeed = 0.1f;
    [SerializeField] private float _ParticleSnowAmount = 100;
    [SerializeField] private ParticleSystem _Snow;

    [Header("Rain")]
    [SerializeField] private float _ParticleRainAmount = 1000;
    [SerializeField] private ParticleSystem _Rain;

    [Header("Ice")]
    [SerializeField] private float _IceIncreaseSpeed = 0.02f;
    [SerializeField] private float _IceDecreaseSpeed = 0.03f;

    [Header("Volume Effects")]
    [SerializeField] private VolumeProfile _SkyAndFogVolume;
    VolumetricClouds vClouds;

    [Header("Refs")]
    [SerializeField] private Transform _Sun = null;
    [SerializeField] private Material _Water = null;
    [SerializeField] private List<Material> _SnowMaterials = new List<Material>();
    [SerializeField] private Slider _TimeSlider;

    private bool _IsRain;

    private float _SnowAmount = 0;
    private float _IceAmount = 0;
    private float _Current_PSnowAmount;
    private float _Current_PRainAmount;

    private int _CloudState;
    private int _CheckCloudState;


    void Update()
    {
        _Sun.transform.eulerAngles = new Vector3((360 /24) * _CurrentTime_Hours - 90, 0, 0);

        //Update Time
        _TimeSlider.value += (0.0002777f * _Time_Multiplier) * Time.deltaTime;

        //RainOrSnow
        if (_Temperature >= 0)
            _IsRain = true;
                else
            _IsRain = false;

        //Clouds
        float cloudcalc = 0;
        if (_BadWeather >= 60)
            cloudcalc = .5f;
        else
            cloudcalc = 60.5f - _BadWeather;


        //DayLight
        float daylightcalc = 0;
        if (_CurrentTime_Hours >= 0 && _CurrentTime_Hours < 12)
            daylightcalc = 1 - (_CurrentTime_Hours / 24);
        if (_CurrentTime_Hours >= 12 && _CurrentTime_Hours < 24)
            daylightcalc = (_CurrentTime_Hours / 24) - 1;

        //LoopDay
        if(_CurrentTime_Hours > 24)
            _TimeSlider.value = 0;

        //Snow
        if (_BadWeather >= 60 && !_IsRain && _SnowAmount < 1)
            _SnowAmount += _SnowIncreaseSpeed * Time.deltaTime;

        for (int i = 0; i < _SnowMaterials.Count; i++)
        {
            _SnowMaterials[i].SetFloat("SnowAmount", _SnowAmount);
        }
        if (_IsRain && _SnowAmount > 0)
            _SnowAmount -= _SnowDecreaseSpeed * Time.deltaTime;

        //Rain/Snow
        _Rain.emissionRate = _Current_PRainAmount;
        _Snow.emissionRate = _Current_PSnowAmount;

        _Rain.transform.eulerAngles = Vector3.zero;
        _Snow.transform.eulerAngles = Vector3.zero;

        if (_BadWeather >= 60)
        {
            Stormy();
            if (_IsRain)
            {
                if (_Current_PRainAmount < _ParticleRainAmount)
                    _Current_PRainAmount += 10 * _BadWeather - 60 * Time.deltaTime;
                else
                    _Current_PRainAmount = _ParticleRainAmount;
            }
            else
            {
                if (_Current_PSnowAmount < _ParticleSnowAmount)
                    _Current_PSnowAmount += 10 * _BadWeather - 60 * Time.deltaTime;
                else
                    _Current_PSnowAmount = _ParticleSnowAmount;
            }
        }
        else
        {
            Cloudy();
            if (_Current_PRainAmount > 0)
                _Current_PRainAmount -= 1000 * Time.deltaTime;
            if (_Current_PSnowAmount > 0)
                _Current_PSnowAmount -= 1000 * Time.deltaTime;
        }
        if (_Current_PRainAmount > 0 && !_IsRain)
            _Current_PRainAmount -= 100 * Time.deltaTime;
        if (_Current_PSnowAmount > 0 && _IsRain)
            _Current_PSnowAmount -= 100 * Time.deltaTime;

        //Water
        if (_Temperature < 0)
        {
            if (_IceAmount < 1)
                _IceAmount += _IceIncreaseSpeed * Time.deltaTime;
            else
                _IceAmount = 1;
        }
        else
        {
            if (_IceAmount > 0)
                _IceAmount -= _IceDecreaseSpeed * Time.deltaTime;
            else
                _IceAmount = 0;
        }

        _Water.SetFloat("IceWater", 1-_IceAmount);
    }

    public void Sparse()
    {
        _CloudState = 1;
        if (_CloudState != _CheckCloudState)
        {
            if (_SkyAndFogVolume.TryGet<VolumetricClouds>(out vClouds))
            {
                vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Sparse;
                Debug.Log(vClouds.cloudPreset);
            }
            _CheckCloudState = _CloudState;
        }
    }
    public void Cloudy()
    {
        _CloudState = 2;
        if (_CloudState != _CheckCloudState)
        {
            if (_SkyAndFogVolume.TryGet<VolumetricClouds>(out vClouds))
            {
                vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Cloudy;
                Debug.Log(vClouds.cloudPreset);
                _CheckCloudState = _CloudState;
            }
        }
    }
    public void Overcast()
    {
        _CloudState = 3;
        if (_CloudState != _CheckCloudState)
        {
            if (_SkyAndFogVolume.TryGet<VolumetricClouds>(out vClouds))
            {
                vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Overcast;
                Debug.Log(vClouds.cloudPreset);
                _CheckCloudState = _CloudState;
            }
        }
    }
    public void Stormy()
    {
        _CloudState = 4;
        if (_CloudState != _CheckCloudState)
        {
            if (_SkyAndFogVolume.TryGet<VolumetricClouds>(out vClouds))
            {
                vClouds.cloudPreset.value = VolumetricClouds.CloudPresets.Stormy;
                Debug.Log(vClouds.cloudPreset);
                _CheckCloudState = _CloudState;
            }
        }
    }

    //GetSet
    public float Get_CurrentTime
    {
        get { return _CurrentTime_Hours; }
        set { _CurrentTime_Hours = value; }
    }
    public float Get_TimeMultiplier
    {
        get { return _Time_Multiplier; }
        set { _Time_Multiplier = value; }
    }
    public float Get_Weather
    {
        get { return _BadWeather; }
        set { _BadWeather = value; }
    }
    public float Get_Temperature
    {
        get { return _Temperature; }
        set { _Temperature = value; }
    }
}
