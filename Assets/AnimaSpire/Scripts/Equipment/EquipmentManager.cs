using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private const int MagicBookSummonCost = 10;

    [SerializeField] private GameManager gameManager;

    private readonly EquipmentCollectionState collectionState = new EquipmentCollectionState();
    private readonly EquipmentLoadoutState loadoutState = new EquipmentLoadoutState();

    public int MagicBookCost => MagicBookSummonCost;
    public static EquipmentManager Instance { get; private set; }
    public EquipmentCollectionState CollectionState => collectionState;
    public EquipmentLoadoutState LoadoutState => loadoutState;

    private void Awake()
    {
        Instance = this;

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

    public bool TryEquipMagicBook(EquipmentId id, out string message)
    {
        EquipmentTier tier = EquipmentTier.T0;

        if (id != EquipmentId.AMagicBook && id != EquipmentId.BMagicBook)
        {
            message = "Only MagicBook equipment can be equipped now.";
            return false;
        }

        if (!EquipmentCatalog.TryGetDefinition(id, tier, out EquipmentDefinition definition))
        {
            message = "MagicBook data is missing.";
            return false;
        }

        if (!collectionState.HasOwned(id, tier))
        {
            message = $"You do not own {definition.displayName} {tier}.";
            return false;
        }

        loadoutState.EquipMagicBook(new EquipmentStackKey(id, tier));
        message = $"Equipped {definition.displayName} {tier}.";
        return true;
    }

    public bool HasEquippedMagicBook()
    {
        return loadoutState.HasEquippedMagicBook();
    }

    public bool TryGetEquippedMagicBook(out EquipmentStackKey key)
    {
        return loadoutState.TryGetEquippedMagicBook(out key);
    }

    public int GetEquippedMagicBookBonusAttackPower()
    {
        if (!loadoutState.TryGetEquippedMagicBook(out EquipmentStackKey key))
        {
            return 0;
        }

        if (!EquipmentCatalog.TryGetDefinition(key.id, key.tier, out EquipmentDefinition definition))
        {
            return 0;
        }

        return definition.bonusAttackPower;
    }
}
