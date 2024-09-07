using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasBase : MonoBehaviour
{
    internal Button[] _buttons;
    internal Toggle[] _toggles;
    internal Slider[] _sliders;
    internal Image[] _images;
    internal TextMeshProUGUI[] _texts;

    private GameObject _lastSelectedObject;
    private CanvasGroup _canvas;

    internal bool _slowOrPauseTimeWhileActive = false;
    internal float _timeScale = 0;
    private float _originalTimeScale;

    internal bool _fadeIn = true;
    internal float _fadeInTimer = 1;

    internal bool _fadeOut = false;
    internal float _fadeOutTimer = 1;

    internal EventSystem _eventSystem;

    internal virtual void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        if (_buttons.Length > 0)
        {
            _lastSelectedObject = _buttons[0].gameObject;
        }
        else if (_toggles.Length > 0)
        {
            _lastSelectedObject = _toggles[0].gameObject;
        }
        else if (_sliders.Length > 0)
        {
            _lastSelectedObject = _sliders[0].gameObject;
        }
        else
        {
            Debug.LogWarning("A canvas should have something interactable on it.");
        }
    }
    internal virtual void OnEnable()
    {
        if (_eventSystem != null)
        {
            _eventSystem.firstSelectedGameObject = _lastSelectedObject;
        }
        if (_fadeIn)
        {
            StartCoroutine(Fade(_fadeInTimer, true));
        }
        if (_slowOrPauseTimeWhileActive)
        {
            _originalTimeScale = Time.timeScale;
            Time.timeScale = _timeScale;
        }
    }
    internal virtual void OnDisable()
    {
        if (_slowOrPauseTimeWhileActive)
        {
            Time.timeScale = _originalTimeScale;
        }
        foreach (var button in _buttons)
        {
            if (!button.enabled)
            {
                button.enabled = true;
            }
        }
        foreach (var toggle in _toggles)
        {
            if (!toggle.enabled)
            {
                toggle.enabled = true;
            }
        }
        foreach (var slider in _sliders)
        {
            if (!slider.enabled)
            {
                slider.enabled = true;
            }
        }
    }
    internal virtual void FadeOut()
    {
        StartCoroutine(Fade(_fadeOutTimer, false));
    }
    internal virtual IEnumerator Fade(float fadeTime, bool fadeIn)
    {
        foreach(var button in _buttons)
        {
            if (button.enabled)
            {
                button.enabled = false;
            }
        }
        foreach(var toggle in _toggles)
        {
            if (toggle.enabled)
            {
                toggle.enabled = false;
            }
        }
        foreach(var slider in _sliders)
        {
            if (slider.enabled)
            {
                slider.enabled = false;
            }
        }
        
        if (fadeIn)
        { // fadingIn
            float timer = 0;
            if (_slowOrPauseTimeWhileActive)
            {
                while (timer < fadeTime)
                {
                    _canvas.alpha = timer / fadeTime;
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            else
            {
                while (timer < fadeTime)
                {
                    _canvas.alpha = timer / fadeTime;
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            foreach (var button in _buttons)
            {
                if (!button.enabled)
                {
                    button.enabled = true;
                }
            }
            foreach (var toggle in _toggles)
            {
                if (!toggle.enabled)
                {
                    toggle.enabled = true;
                }
            }
            foreach (var slider in _sliders)
            {
                if (!slider.enabled)
                {
                    slider.enabled = true;
                }
            }
        }
        else
        { // fadingOut
            float timer = fadeTime;
            if (_slowOrPauseTimeWhileActive)
            {
                while (timer > 0)
                {
                    _canvas.alpha = timer / fadeTime;
                    timer -= Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            else
            {
                while (timer > 0)
                {
                    _canvas.alpha = timer / fadeTime;
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            gameObject.SetActive(false);
        }
    }
}
