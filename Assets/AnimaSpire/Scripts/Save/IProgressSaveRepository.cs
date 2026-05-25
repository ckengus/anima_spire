public interface IProgressSaveRepository
{
    bool HasSave();
    bool TryLoad(out PlayerProgressData data);
    bool TrySave(PlayerProgressData data);
    bool TryDelete();
}
