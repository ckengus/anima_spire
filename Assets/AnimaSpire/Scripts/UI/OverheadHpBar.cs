using UnityEngine;

public class OverheadHpBar : MonoBehaviour
{
    [SerializeField] private HeroUnit hero;
    [SerializeField] private EnemyUnit enemy;
    [SerializeField] private Vector2 barSize = new Vector2(0.9f, 0.08f);
    [SerializeField] private float verticalOffset = 0.75f;
    [SerializeField] private float viewportHorizontalPadding = 0.08f;
    [SerializeField] private Color backgroundColor = new Color(0.08f, 0.08f, 0.08f, 0.85f);
    [SerializeField] private Color fillColor = new Color(0.2f, 0.95f, 0.25f, 1f);

    private static Sprite whiteSprite;

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer fillRenderer;

    private void Awake()
    {
        if (hero == null)
        {
            hero = GetComponent<HeroUnit>();
        }

        if (enemy == null)
        {
            enemy = GetComponent<EnemyUnit>();
        }

        EnsureBar();
        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void EnsureBar()
    {
        backgroundRenderer = EnsureRenderer("HpBar_Background", backgroundColor, 20);
        fillRenderer = EnsureRenderer("HpBar_Fill", fillColor, 21);
    }

    private SpriteRenderer EnsureRenderer(string objectName, Color color, int sortingOrder)
    {
        Transform existing = transform.Find(objectName);
        SpriteRenderer renderer;

        if (existing != null && existing.TryGetComponent(out renderer))
        {
            return renderer;
        }

        GameObject barObject = new GameObject(objectName, typeof(SpriteRenderer));
        barObject.transform.SetParent(transform, false);
        barObject.transform.localPosition = new Vector3(0f, verticalOffset, 0f);

        renderer = barObject.GetComponent<SpriteRenderer>();
        renderer.sprite = GetWhiteSprite();
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return renderer;
    }

    private void Refresh()
    {
        if (backgroundRenderer == null || fillRenderer == null)
        {
            return;
        }

        float ratio = GetHpRatio();
        float horizontalOffset = GetViewportClampLocalOffset();

        backgroundRenderer.transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        backgroundRenderer.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);

        fillRenderer.transform.localPosition = new Vector3(horizontalOffset - barSize.x * (1f - ratio) * 0.5f, verticalOffset, -0.01f);
        fillRenderer.transform.localScale = new Vector3(barSize.x * ratio, barSize.y, 1f);
    }

    private float GetViewportClampLocalOffset()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic)
        {
            return 0f;
        }

        float parentScaleX = Mathf.Abs(transform.lossyScale.x);
        if (parentScaleX <= 0f)
        {
            return 0f;
        }

        float halfBarWidth = barSize.x * parentScaleX * 0.5f;
        float halfViewportWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float minX = mainCamera.transform.position.x - halfViewportWidth + halfBarWidth + viewportHorizontalPadding;
        float maxX = mainCamera.transform.position.x + halfViewportWidth - halfBarWidth - viewportHorizontalPadding;

        if (minX > maxX)
        {
            return 0f;
        }

        float clampedWorldX = Mathf.Clamp(transform.position.x, minX, maxX);
        float worldOffset = clampedWorldX - transform.position.x;
        return worldOffset / parentScaleX;
    }

    private float GetHpRatio()
    {
        if (hero != null)
        {
            return CalculateRatio(hero.currentHp, hero.maxHp);
        }

        if (enemy != null)
        {
            return CalculateRatio(enemy.currentHp, enemy.maxHp);
        }

        return 0f;
    }

    private float CalculateRatio(float currentHp, float maxHp)
    {
        if (maxHp <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(currentHp / maxHp);
    }

    private static Sprite GetWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return whiteSprite;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }
}
