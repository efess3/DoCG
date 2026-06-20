using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public LeaderboardEntryUI entryPrefab;
    public int maxEntries = 20; // Increased default to allow scrolling more entries

    private bool scrollViewCreated = false;

    private void Awake()
    {
        SetupScrollView();
    }

    private void SetupScrollView()
    {
        if (scrollViewCreated) return;
        scrollViewCreated = true;

        // The current object (Entries) will act as the Content of the ScrollRect.
        RectTransform contentRect = GetComponent<RectTransform>();
        if (contentRect == null) return;

        Transform originalParent = transform.parent;
        int siblingIndex = transform.GetSiblingIndex();

        // 1. Create the ScrollView root object
        GameObject scrollViewGo = new GameObject("LeaderboardScrollView", typeof(RectTransform));
        RectTransform scrollViewRect = scrollViewGo.GetComponent<RectTransform>();
        scrollViewRect.SetParent(originalParent, false);
        scrollViewRect.SetSiblingIndex(siblingIndex);

        // Copy position and size constraints from Content (Entries) to ScrollView
        scrollViewRect.anchorMin = contentRect.anchorMin;
        scrollViewRect.anchorMax = contentRect.anchorMax;
        scrollViewRect.anchoredPosition = contentRect.anchoredPosition;
        scrollViewRect.sizeDelta = contentRect.sizeDelta;
        scrollViewRect.pivot = contentRect.pivot;

        // 2. Create the Viewport object
        GameObject viewportGo = new GameObject("Viewport", typeof(RectTransform));
        RectTransform viewportRect = viewportGo.GetComponent<RectTransform>();
        viewportRect.SetParent(scrollViewRect, false);

        // Viewport should stretch to fill the ScrollView
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;
        viewportRect.pivot = new Vector2(0.5f, 0.5f);

        // Add RectMask2D to Viewport for clipping
        viewportGo.AddComponent<RectMask2D>();

        // 3. Reparent Content (Entries) to Viewport
        contentRect.SetParent(viewportRect, false);

        // Reset/Adjust Content layout constraints for scrolling
        // Top-aligned content (pivot at top), matching width of Viewport
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        // Keep horizontal size matching parent (0 sizeDelta width with anchors 0 to 1)
        contentRect.sizeDelta = new Vector2(0f, contentRect.sizeDelta.y);

        // Add ContentSizeFitter to Content (Entries) so it resizes based on children
        ContentSizeFitter sizeFitter = gameObject.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // 4. Add ScrollRect component to ScrollView
        ScrollRect scrollRect = scrollViewGo.AddComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 25f;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
    }

    private void OnEnable()
    {
        // usuń stare wpisy
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // dodaj nowe wpisy
        LeaderboardManager.LeaderboardData data = LeaderboardManager.instance.LoadScores();
        var entries = data.entries.OrderByDescending(e => e.survivedTimespan).Take(maxEntries);
        foreach (var entry in entries)
        {
            Instantiate(entryPrefab, transform).Setup(entry);
        }
    }
}
