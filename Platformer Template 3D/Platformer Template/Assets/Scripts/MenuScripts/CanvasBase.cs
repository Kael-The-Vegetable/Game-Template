using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasBase : MonoBehaviour
{
    [SerializeField] internal Button[] _buttons;
    [SerializeField] internal Toggle[] _toggles;
    [SerializeField] internal Slider[] _sliders;
    [SerializeField] internal TMP_Dropdown[] _dropdowns;
    [SerializeField] internal TMP_InputField[] _inputFields;

    [SerializeField] internal Image[] _images;
    [SerializeField] internal TextMeshProUGUI[] _texts;

    internal GameObject _lastSelectedObject;
    internal CanvasGroup _canvas;

    [SerializeField] internal bool _slowOrPauseTimeWhileActive = false;
    [SerializeField] internal float _timeScale = 0;
    internal float _originalTimeScale;

    [SerializeField] internal bool _fadeIn = true;
    [SerializeField] internal float _fadeInTimer = 1;

    [SerializeField] internal bool _fadeOut = false;
    [SerializeField] internal float _fadeOutTimer = 1;

    [SerializeField] internal EventSystem _eventSystem;

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
        else if (_dropdowns.Length > 0)
        {
            _lastSelectedObject = _dropdowns[0].gameObject;
        }
        else if (_inputFields.Length > 0)
        {
            _lastSelectedObject = _inputFields[0].gameObject;
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
            FadeIn();
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
        _canvas.interactable = true;
        if (_eventSystem != null)
        {
            _lastSelectedObject = _eventSystem.currentSelectedGameObject;
        }
    }
    internal virtual void FadeIn()
    {
        StartCoroutine(Fade(_fadeInTimer, true));
    }
    internal virtual void FadeOut()
    {
        StartCoroutine(Fade(_fadeOutTimer, false));
    }
    internal virtual IEnumerator Fade(float fadeTime, bool fadeIn)
    {
        _canvas.interactable = false;
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
            _canvas.alpha = 1;
            _canvas.interactable = true;
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
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
            _canvas.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}
