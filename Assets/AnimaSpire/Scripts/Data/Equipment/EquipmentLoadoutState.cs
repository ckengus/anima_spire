using System;

public sealed class EquipmentLoadoutState
{
    private EquipmentStackKey? equippedMagicBook;

    public event Action<EquipmentStackKey?> OnEquippedMagicBookChanged;

    public void EquipMagicBook(EquipmentStackKey key)
    {
        equippedMagicBook = key;
        OnEquippedMagicBookChanged?.Invoke(equippedMagicBook);
    }

    public void SetEquippedMagicBookForLoad(EquipmentStackKey? key)
    {
        equippedMagicBook = key;
        OnEquippedMagicBookChanged?.Invoke(equippedMagicBook);
    }

    public bool HasEquippedMagicBook()
    {
        return equippedMagicBook.HasValue;
    }

    public bool TryGetEquippedMagicBook(out EquipmentStackKey key)
    {
        if (equippedMagicBook.HasValue)
        {
            key = equippedMagicBook.Value;
            return true;
        }

        key = default;
        return false;
    }
}
