
[System.Serializable]
public class QuizScoreData
{
    public int quizId;
    public int quizScore;

    public QuizScoreData(int id, int score)
    {
        this.quizId = id;
        this.quizScore = score;
    }
}
