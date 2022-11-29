using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainMenu.Helpers
{
    public class ScrollRectAutoScroll : MonoBehaviour
    {
        private int elementCount;

        private readonly List<Selectable> selectables = new List<Selectable>();

        #region Properties

        private ScrollRect ScrollRectComponent { get; set; }

        #endregion

        private void Awake()
        {
            ScrollRectComponent = GetComponent<ScrollRect>();
        }

        private void Start()
        {
            ScrollRectComponent.content.GetComponentsInChildren(selectables);
            elementCount = selectables.Count;
        }

        private void Update()
        {
            if (elementCount > 0)
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                {
                    var selectedIndex = -1;
                    var selectedElement = EventSystem.current.currentSelectedGameObject
                        ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>()
                        : null;

                    if (selectedElement)
                        selectedIndex = selectables.IndexOf(selectedElement);

                    if (selectedIndex > -1)
                        ScrollRectComponent.verticalNormalizedPosition = 1 - selectedIndex / ((float) elementCount - 1);
                }
        }

        private void OnEnable()
        {
            if (ScrollRectComponent)
            {
                ScrollRectComponent.content.GetComponentsInChildren(selectables);
                elementCount = selectables.Count;
            }
        }
    }
}