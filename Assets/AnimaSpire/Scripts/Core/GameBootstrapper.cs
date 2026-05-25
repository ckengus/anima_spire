using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private ProgressSaveManager progressSaveManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private EquipmentManager equipmentManager;

    private void Start()
    {
        EnsureRuntimeManagers();
        progressSaveManager.LoadAndApplyData();
        progressSaveManager.BindRuntimeEvents();
    }

    private void EnsureRuntimeManagers()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
        }

        if (combatManager == null)
        {
            combatManager = FindAnyObjectByType<CombatManager>();
        }

        equipmentManager = equipmentManager != null ? equipmentManager : EquipmentManager.EnsureInstance();

        if (progressSaveManager == null)
        {
            progressSaveManager = FindAnyObjectByType<ProgressSaveManager>();
        }

        if (progressSaveManager == null)
        {
            GameObject saveManagerObject = new GameObject("ProgressSaveManager");
            progressSaveManager = saveManagerObject.AddComponent<ProgressSaveManager>();
        }

        if (gameManager == null)
        {
            Debug.LogError("GameBootstrapper could not find GameManager.");
        }

        if (stageManager == null)
        {
            Debug.LogError("GameBootstrapper could not find StageManager.");
        }

        if (combatManager == null)
        {
            Debug.LogError("GameBootstrapper could not find CombatManager.");
        }
    }
}
