using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private const int MagicBookSummonCost = 10;

    [SerializeField] private GameManager gameManager;

    private readonly EquipmentCollectionState collectionState = new EquipmentCollectionState();

    public int MagicBookCost => MagicBookSummonCost;
    public EquipmentCollectionState CollectionState => collectionState;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    public bool TrySummonMagicBook(out EquipmentDefinition summonedDefinition, out string message)
    {
        summonedDefinition = null;

        if (gameManager == null)
        {
            message = "Equipment system is not ready.";
            return false;
        }

        EquipmentId summonedId = Random.value < 0.5f ? EquipmentId.AMagicBook : EquipmentId.BMagicBook;
        EquipmentTier tier = EquipmentTier.T0;

        if (!EquipmentCatalog.TryGetDefinition(summonedId, tier, out summonedDefinition))
        {
            message = "MagicBook data is missing.";
            return false;
        }

        if (!gameManager.TrySpendGold(MagicBookSummonCost))
        {
            summonedDefinition = null;
            message = "Not enough Gold.";
            return false;
        }

        collectionState.AddOwnedCount(summonedId, tier, 1);
        message = $"Summoned {summonedDefinition.displayName} {tier}";
        return true;
    }

    public int GetOwnedCount(EquipmentId id, EquipmentTier tier)
    {
        return collectionState.GetOwnedCount(id, tier);
    }

    public bool HasOwned(EquipmentId id, EquipmentTier tier)
    {
        return collectionState.HasOwned(id, tier);
    }
}
