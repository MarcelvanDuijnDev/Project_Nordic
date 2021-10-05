using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [Range(0, 24)] private float _CurrentTime_Hours = 6;
    [SerializeField] [Range(0, 100)] private float _BadWeather = 0;
    [SerializeField] [Range(-40, 40)] private float _Temperature = 0;

    
    [SerializeField] private float _SnowIncreaseSpeed = 0.05f;
    [SerializeField] private float _SnowDecreaseSpeed = 0.1f;

    [Header("Refs")]
    [SerializeField] private Transform _Sun = null;
    [SerializeField] private Material _SkyBox = null;
    [SerializeField] private List<Material> _SnowMaterials = new List<Material>();

    [Header("Refs Particles")]
    [SerializeField] private ParticleSystem _Rain;
    [SerializeField] private float _ParticleRainAmount = 1000;
    [SerializeField] private ParticleSystem _Snow;
    [SerializeField] private float _ParticleSnowAmount = 1000;

    private bool _IsRain;

    private float _SnowAmount = 0;
    private float _Current_PSnowAmount;
    private float _Current_PRainAmount;

    void Start()
    {
        
    }

    [System.Obsolete]
    void Update()
    {
        _Sun.transform.eulerAngles = new Vector3((360 /25) * _CurrentTime_Hours, 0, 0);

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
        _SkyBox.SetFloat("C_DistanceFadePower", cloudcalc);

        //DayLight
        float daylightcalc = 0;
        if (_CurrentTime_Hours >= 0 && _CurrentTime_Hours < 12)
            daylightcalc = 1 - (_CurrentTime_Hours / 24);
        if (_CurrentTime_Hours >= 12 && _CurrentTime_Hours < 24)
            daylightcalc = (_CurrentTime_Hours / 24) - 1;


        _SkyBox.SetFloat("DayLight", daylightcalc);


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
            if (_Current_PRainAmount > 0)
                _Current_PRainAmount -= 100 * Time.deltaTime;
            if (_Current_PSnowAmount > 0)
                _Current_PSnowAmount -= 100 * Time.deltaTime;
        }
        if (_Current_PRainAmount > 0 && !_IsRain)
            _Current_PRainAmount -= 100 * Time.deltaTime;
        if (_Current_PSnowAmount > 0 && _IsRain)
            _Current_PSnowAmount -= 100 * Time.deltaTime;
    }
}
