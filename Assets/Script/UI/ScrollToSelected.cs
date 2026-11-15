
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    private GameObject lastSelected;

    private void LateUpdate()
    {
        if (EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == lastSelected) return;

        lastSelected = selected;

        if (selected == null) return;

        RectTransform target = selected.GetComponent<RectTransform>();

        if (target == null || !target.IsChildOf(scrollRect.content))
            return;

        ScrollTo(target);
    }

    private void ScrollTo(RectTransform target)
    {
        RectTransform viewport = scrollRect.viewport;
        RectTransform content = scrollRect.content;

        Bounds targetBounds =
            RectTransformUtility.CalculateRelativeRectTransformBounds(
                viewport, target);

        Rect viewRect = viewport.rect;

        float offset = 0f;

        // 选中的按钮超过下边界
        if (targetBounds.min.y < viewRect.yMin)
        {
            offset = viewRect.yMin - targetBounds.min.y;
        }
        // 选中的按钮超过上边界
        else if (targetBounds.max.y > viewRect.yMax)
        {
            offset = viewRect.yMax - targetBounds.max.y;
        }

        if (Mathf.Approximately(offset, 0f)) return;

        scrollRect.StopMovement();

        content.anchoredPosition += new Vector2(0f, offset);
    }
}
