using System.Collections.Generic;

public static class EquipmentCatalog
{
    private static readonly EquipmentDefinition[] definitions =
    {
        new EquipmentDefinition(
            EquipmentId.AMagicBook,
            EquipmentType.MagicBook,
            EquipmentCategory.Weapon,
            EquipmentTier.T0,
            "마법서 A",
            "MVP 도감용 마법서 A.",
            3,
            0),
        new EquipmentDefinition(
            EquipmentId.BMagicBook,
            EquipmentType.MagicBook,
            EquipmentCategory.Weapon,
            EquipmentTier.T0,
            "마법서 B",
            "MVP 도감용 마법서 B.",
            3,
            2),
        new EquipmentDefinition(
            EquipmentId.CMagicBook,
            EquipmentType.MagicBook,
            EquipmentCategory.Weapon,
            EquipmentTier.T0,
            "마법서 C",
            "MVP 도감용 마법서 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.DMagicBook,
            EquipmentType.MagicBook,
            EquipmentCategory.Weapon,
            EquipmentTier.T0,
            "마법서 D",
            "MVP 도감용 마법서 D.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.EMagicBook,
            EquipmentType.MagicBook,
            EquipmentCategory.Weapon,
            EquipmentTier.T0,
            "마법서 E",
            "MVP 도감용 마법서 E.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.ANecklace,
            EquipmentType.Necklace,
            EquipmentCategory.Necklace,
            EquipmentTier.T0,
            "목걸이 A",
            "MVP 도감용 목걸이 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BNecklace,
            EquipmentType.Necklace,
            EquipmentCategory.Necklace,
            EquipmentTier.T0,
            "목걸이 B",
            "MVP 도감용 목걸이 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CNecklace,
            EquipmentType.Necklace,
            EquipmentCategory.Necklace,
            EquipmentTier.T0,
            "목걸이 C",
            "MVP 도감용 목걸이 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.AEarring,
            EquipmentType.Necklace,
            EquipmentCategory.Earring,
            EquipmentTier.T0,
            "귀고리 A",
            "MVP 도감용 귀고리 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BEarring,
            EquipmentType.Necklace,
            EquipmentCategory.Earring,
            EquipmentTier.T0,
            "귀고리 B",
            "MVP 도감용 귀고리 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CEarring,
            EquipmentType.Necklace,
            EquipmentCategory.Earring,
            EquipmentTier.T0,
            "귀고리 C",
            "MVP 도감용 귀고리 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.ARing,
            EquipmentType.Ring,
            EquipmentCategory.Ring,
            EquipmentTier.T0,
            "반지 A",
            "MVP 도감용 반지 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BRing,
            EquipmentType.Ring,
            EquipmentCategory.Ring,
            EquipmentTier.T0,
            "반지 B",
            "MVP 도감용 반지 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CRing,
            EquipmentType.Ring,
            EquipmentCategory.Ring,
            EquipmentTier.T0,
            "반지 C",
            "MVP 도감용 반지 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.AHat,
            EquipmentType.Hat,
            EquipmentCategory.Hat,
            EquipmentTier.T0,
            "모자 A",
            "MVP 도감용 모자 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BHat,
            EquipmentType.Hat,
            EquipmentCategory.Hat,
            EquipmentTier.T0,
            "모자 B",
            "MVP 도감용 모자 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CHat,
            EquipmentType.Hat,
            EquipmentCategory.Hat,
            EquipmentTier.T0,
            "모자 C",
            "MVP 도감용 모자 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.AClothes,
            EquipmentType.Robe,
            EquipmentCategory.Clothes,
            EquipmentTier.T0,
            "옷 A",
            "MVP 도감용 옷 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BClothes,
            EquipmentType.Robe,
            EquipmentCategory.Clothes,
            EquipmentTier.T0,
            "옷 B",
            "MVP 도감용 옷 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CClothes,
            EquipmentType.Robe,
            EquipmentCategory.Clothes,
            EquipmentTier.T0,
            "옷 C",
            "MVP 도감용 옷 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.AGloves,
            EquipmentType.Gloves,
            EquipmentCategory.Gloves,
            EquipmentTier.T0,
            "장갑 A",
            "MVP 도감용 장갑 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BGloves,
            EquipmentType.Gloves,
            EquipmentCategory.Gloves,
            EquipmentTier.T0,
            "장갑 B",
            "MVP 도감용 장갑 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CGloves,
            EquipmentType.Gloves,
            EquipmentCategory.Gloves,
            EquipmentTier.T0,
            "장갑 C",
            "MVP 도감용 장갑 C.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.AShoes,
            EquipmentType.Shoes,
            EquipmentCategory.Shoes,
            EquipmentTier.T0,
            "신발 A",
            "MVP 도감용 신발 A.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.BShoes,
            EquipmentType.Shoes,
            EquipmentCategory.Shoes,
            EquipmentTier.T0,
            "신발 B",
            "MVP 도감용 신발 B.",
            0,
            0),
        new EquipmentDefinition(
            EquipmentId.CShoes,
            EquipmentType.Shoes,
            EquipmentCategory.Shoes,
            EquipmentTier.T0,
            "신발 C",
            "MVP 도감용 신발 C.",
            0,
            0)
    };

    private static readonly EquipmentDefinition[] summonableDefinitions =
    {
        definitions[0],
        definitions[1]
    };

    public static IReadOnlyList<EquipmentDefinition> Definitions => definitions;

    public static IReadOnlyList<EquipmentDefinition> GetAllCodexDefinitions()
    {
        return definitions;
    }

    public static IReadOnlyList<EquipmentDefinition> GetSummonableDefinitions()
    {
        return summonableDefinitions;
    }

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
