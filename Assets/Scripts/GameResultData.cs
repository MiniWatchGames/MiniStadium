public static class GameResultData
{
    public static int totalRounds = 12;
    public static int playerWins = 7;
    public static int opponentWins = 5;

    public static bool IsWin => playerWins > opponentWins;
}
