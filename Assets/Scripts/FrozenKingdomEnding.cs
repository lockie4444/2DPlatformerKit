using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Only installs the ending in FrozenKingdom; other kit scenes are unaffected.
public class FrozenKingdomEnding : MonoBehaviour
{
    public float liftX = 481.47f;
    public float corridorHalfWidth = 7f;
    public float fadeStartHeight = 15f;
    public float fullyOrangeHeight = 45f;
    public float winTriggerHeight = 55f;
    public Color orange = new Color32(207, 167, 122, 255);
    private Image overlay;
    private bool loading;
    private bool enteredEnding;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FrozenKingdom" && FindFirstObjectByType<FrozenKingdomEnding>() == null)
            new GameObject("Frozen Kingdom Orange Ending").AddComponent<FrozenKingdomEnding>();
    }

    private void Start()
    {
        var canvasObject = new GameObject("Orange Fade", typeof(RectTransform), typeof(Canvas));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;
        var imageObject = new GameObject("Full Screen Orange", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(canvasObject.transform, false);
        overlay = imageObject.GetComponent<Image>();
        overlay.raycastTarget = false;
        var rect = overlay.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        SetAlpha(0f);
        var triggerObject = new GameObject("Orange Ending Win Trigger");
        triggerObject.transform.SetParent(transform, false);
        triggerObject.transform.position = new Vector3(liftX, winTriggerHeight, 0f);
        var collider = triggerObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(corridorHalfWidth * 2f, 2f);
        triggerObject.AddComponent<FrozenKingdomWinTrigger>().ending = this;
    }

    private void Update()
    {
        var player = PlayerController.instance;
        if (player == null || overlay == null || loading) return;
        var position = player.transform.position;
        if (Mathf.Abs(position.x - liftX) <= corridorHalfWidth && position.y >= fadeStartHeight)
            enteredEnding = true;
        if (!enteredEnding) return;
        // Height controls the fade. No movement, input, or time-scale changes.
        SetAlpha(Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(fadeStartHeight, fullyOrangeHeight, position.y)));
        if (position.y < fadeStartHeight - 2f) enteredEnding = false;
    }

    private void SetAlpha(float alpha)
    {
        var color = orange;
        color.a = alpha;
        overlay.color = color;
    }

    public void ReachWinTrigger(Collider2D other)
    {
        if (loading || overlay == null || overlay.color.a < 0.999f || other.GetComponentInParent<PlayerController>() == null)
            return;
        loading = true;
        SceneManager.LoadScene("WinScreen");
    }
}

public class FrozenKingdomWinTrigger : MonoBehaviour
{
    [System.NonSerialized] public FrozenKingdomEnding ending;
    private void OnTriggerEnter2D(Collider2D other) { ending.ReachWinTrigger(other); }
    private void OnTriggerStay2D(Collider2D other) { ending.ReachWinTrigger(other); }
}