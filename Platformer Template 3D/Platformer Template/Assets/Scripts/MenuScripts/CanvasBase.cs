using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasBase : MonoBehaviour
{
    [field:SerializeField] public Button[] Buttons { get; internal set; }
    [field:SerializeField] public Toggle[] Toggles { get; internal set; }
    [field:SerializeField] public Slider[] Sliders { get; internal set; }
    [field:SerializeField] public TMP_Dropdown[] Dropdowns { get; internal set; }
    [field:SerializeField] public TMP_InputField[] InputFields { get; internal set; }

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
        if (Buttons.Length > 0)
        {
            _lastSelectedObject = Buttons[0].gameObject;
        }
        else if (Toggles.Length > 0)
        {
            _lastSelectedObject = Toggles[0].gameObject;
        }
        else if (Sliders.Length > 0)
        {
            _lastSelectedObject = Sliders[0].gameObject;
        }
        else if (Dropdowns.Length > 0)
        {
            _lastSelectedObject = Dropdowns[0].gameObject;
        }
        else if (InputFields.Length > 0)
        {
            _lastSelectedObject = InputFields[0].gameObject;
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
