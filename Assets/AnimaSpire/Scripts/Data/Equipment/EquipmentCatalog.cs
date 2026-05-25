using System.Collections.Generic;

public static class EquipmentCatalog
{
    private static readonly EquipmentDefinition[] definitions =
    {
        new EquipmentDefinition(
            EquipmentId.AMagicBook,
            EquipmentType.MagicBook,
            EquipmentTier.T0,
            "A MagicBook",
            "Basic MagicBook A for MVP testing.",
            3,
            0),
        new EquipmentDefinition(
            EquipmentId.BMagicBook,
            EquipmentType.MagicBook,
            EquipmentTier.T0,
            "B MagicBook",
            "Basic MagicBook B for MVP testing.",
            1,
            2)
    };

    public static IReadOnlyList<EquipmentDefinition> Definitions => definitions;

    public static bool TryGetDefinition(EquipmentId id, EquipmentTier tier, out EquipmentDefinition definition)
    {
        for (int i = 0; i < definitions.Length; i++)
        {
            EquipmentDefinition current = definitions[i];
            if (current.id == id && current.tier == tier)
            {
                definition = current;
                return true;
            }
        }

        definition = null;
        return false;
    }
}
