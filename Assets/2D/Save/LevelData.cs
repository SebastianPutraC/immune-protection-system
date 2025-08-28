
[System.Serializable]
public class LevelData
{
    public int LevelId;
    public bool Unlocked;

    public LevelData(int LevelId, bool unlocked)
    {
        this.LevelId = LevelId;
        this.Unlocked = unlocked;
    }
}
