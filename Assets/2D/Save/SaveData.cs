using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string profileName;
    public List<EncyclopediaData> enyclopediaUnlock;
    public List<LevelData> levelUnlock;
    public List<QuizScoreData> quizScores;
    public float sfxVolume;
    public float musicVolume;

    public SaveData(string name, List<int> levelIds, List<int> encylopediaIds, List<int> quizIds)
    {
        profileName = name;
        enyclopediaUnlock = new List<EncyclopediaData>();
        levelUnlock = new List<LevelData>();
        quizScores = new List<QuizScoreData>();

        sfxVolume = 0.5f;
        musicVolume = 0.5f;

        for (int i = 0; i < levelIds.Count; i++)
        {
            LevelData newLevelData = new LevelData(levelIds[i], false);
            if (newLevelData.LevelId == (int)LevelId.LEVEL_TUTORIAL || newLevelData.LevelId == (int)LevelId.LEVEL_ONE)
            {
                newLevelData.Unlocked = true;
            }
            levelUnlock.Add(newLevelData);
        }

        for (int i = 0; i < encylopediaIds.Count; i++)
        {
            EncyclopediaData newEncylopediaData = new EncyclopediaData(encylopediaIds[i], false);
            enyclopediaUnlock.Add(newEncylopediaData);
        }

        for (int i = 0; i < quizIds.Count; i++)
        {
            QuizScoreData newQuizScore = new QuizScoreData(quizIds[i], 0);
            quizScores.Add(newQuizScore);
        }
    }
}
