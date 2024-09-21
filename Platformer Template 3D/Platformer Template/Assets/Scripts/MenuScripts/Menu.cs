using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    protected Selectable[] Selectables { get; set; }

    [Tooltip("The parent menu will be hidden when this menu is opened, and shown when this menu is closed.")]
    [SerializeField] Menu parent;
    public bool isOpen;

    [Header("Events")]
    [Tooltip("This event is called when the menu is opened.")]
    public UnityEvent onMenuOpened;
    [Tooltip("This event is called when the menu is closed.")]
    public UnityEvent onMenuClosed;
    [Space]
    [Tooltip("This event is called when the menu is shown or opened.")]
    public UnityEvent onMenuShown;
    [Tooltip("This event is called when the menu is hidden or closed.")]
    public UnityEvent onMenuHidden;

    // Protip: you can hover your mouse over a method that is blue like this to see when it is called!
    protected virtual void OnEnable()
    {
        Selectables = GetComponentsInChildren<Selectable>();

        if (isOpen)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    // Context child means you can run this method in the inspector by right-clicking the component
    [ContextMenu("Open Menu")]
    public void OpenMenu()
    {
        if (!isOpen) // Only open the menu if we are closed
        {
            isOpen = true;
            onMenuOpened.Invoke();
            ShowMenu();

            if (parent != null)
            {
                parent.HideMenu();
            }
        }
    }

    [ContextMenu("Close Menu")]
    public void CloseMenu()
    {
        if (isOpen) // Only close the menu if we are open
        {
            isOpen = false;
            onMenuClosed.Invoke();
            HideMenu();

            if (parent != null)
            {
                parent.ShowMenu();
            }

            foreach (var child in FindObjectsOfType<Menu>().Where(m => m.parent = this))
            {
                child.CloseMenu();
            }

        }
    }

    [ContextMenu("Show Menu")]
    public void ShowMenu()
    {
        SetSelectables(true);
        onMenuShown.Invoke();
    }

    [ContextMenu("Hide Menu")]
    public void HideMenu()
    {
        SetSelectables(false);
        onMenuHidden.Invoke();
    }

    public void ToggleMenu()
    {
        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    protected void SetSelectables(bool state)
    {
        if (Selectables.Length > 0)
        {
            foreach (var selectable in Selectables)
            {
                selectable.interactable = state;
            }
        }
    }
}
