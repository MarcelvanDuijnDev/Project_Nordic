using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FadeInOut : MonoBehaviour
{
    private enum Fade { In, Out }
    [SerializeField] private Fade _FadeOption = Fade.In;
    [SerializeField] private float _Duration = 0;

    [SerializeField] private Image _Image = null;
    [SerializeField] private TextMeshProUGUI _TextMeshPro = null;

    private float _ChangeSpeed;
    private Color _Color;

    private bool _IsImage;

    void Start()
    {
        if (_Image == null)
            _IsImage = false;

        if (_IsImage)
        {
            if (_FadeOption == Fade.In)
                _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 0);
            else
                _Color = new Color(_Image.color.r, _Image.color.g, _Image.color.b, 1);
        }
        else
        {
            if (_FadeOption == Fade.In)
                _Color = new Color(_TextMeshPro.color.r, _TextMeshPro.color.g, _TextMeshPro.color.b, 0);
            else
                _Color = new Color(_TextMeshPro.color.r, _TextMeshPro.color.g, _TextMeshPro.color.b, 1);
        }

        _ChangeSpeed = 1 / _Duration;
    }

    void Update()
    {
        if (_FadeOption == Fade.In && _Color.a < 1)
            _Color.a += _ChangeSpeed * Time.deltaTime;
        if (_FadeOption == Fade.Out && _Color.a > 0)
            _Color.a -= _ChangeSpeed * Time.deltaTime;

        if (_IsImage)
            _Image.color = _Color;
        else
            _TextMeshPro.color = _Color;
    }
}
