using System.IO;
using System;
using System.Collections.Generic;

class Player
{
    public string Name;
    public int Shooting = 0;
    public int Height = 0;
    public bool InGame = false;
    public int Rank = 1;
    public int Played = 0;
}

class Team
{
    public int Entries;
    public int Minutes;
    public int TeamMates;
    public List<Player> Players = new List<Player>();
}

class League
{
    public int Entries;
    public List<Team> Teams = new List<Team>();
}

class Game
{
    List<Player> Team1 = new List<Player>();
    List<Player> Team2 = new List<Player>();

    public List<string> players = new List<string>();

    int Minutes = 0;

    Team team;

    public Game(Team players)
    {
        team = players;
        SplitTeams();
        for (int i = 0; i < players.TeamMates;i++ )
        {
            NextIn();
        }
        Validate_FullTeam();
    }
    public void Rotate(int minutes = 0)
    {
        if (minutes == 0)
        {
            minutes = team.Minutes;
        }
        while(Minutes < minutes)
        {
            PlayMinute();
            Validate_FullTeam();
            NextOut();
            Validate_RotateOut();
            NextIn();
            Validate_FullTeam();
        }
    }
    public void PlayerList()
    {
        players.Clear();
        foreach (Player player in Team1)
        {
            if (player.InGame)
            {
                players.Add(player.Name);
            }
        }
        foreach (Player player in Team2)
        {
            if (player.InGame)
            {
                players.Add(player.Name);
            }
        }
        players.Sort();
    }
    private void SplitTeams()
    {
        foreach (Player player in team.Players)
        {
            foreach (Player mate in team.Players)
            {
                if (mate.Shooting > player.Shooting)
                {
                    player.Rank++;
                }
                else if (mate.Shooting == player.Shooting && mate.Height > player.Height)
                {
                    player.Rank++;
                }
            }
        }
        foreach (Player player in team.Players)
        {
            if (player.Rank % 2 == 0)
            {
                Team2.Add(player);
            }
            else
            {
                Team1.Add(player);
            }
        }
    }
    private void NextIn()
    {
        Player next = Team1[0];
        foreach (Player player in Team1)
        {
            if (!player.InGame)
            {
                if (next.InGame)
                {
                    next = player;
                }
                if (player.Played < next.Played)
                {
                    next = player;
                }
                if (player.Played == next.Played && player.Rank < next.Rank)
                {
                    next = player;
                }
            }
        }
        next.InGame = true;
        next = Team2[0];
        foreach (Player player in Team2)
        {
            if (!player.InGame)
            {
                if (next.InGame)
                {
                    next = player;
                }
                if (player.Played < next.Played)
                {
                    next = player;
                }
                if (player.Played == next.Played && player.Rank < next.Rank)
                {
                    next = player;
                }
            }
        }
        next.InGame = true;
    }
    private void PlayMinute()
    {
        foreach (Player player in Team1)
        {
            if (player.InGame)
            {
                player.Played++;
            }
        }
        foreach (Player player in Team2)
        {
            if (player.InGame)
            {
                player.Played++;
            }
        }
        Minutes++;
    }
    private void NextOut()
    {
        Player next = Team1[0];
        foreach (Player player in Team1)
        {
            if (player.InGame)
            {
                if (!next.InGame)
                {
                    next = player;
                }
                if (player.Played > next.Played)
                {
                    next = player;
                }
                if (player.Played == next.Played && player.Rank > next.Rank)
                {
                    next = player;
                }
            }
        }
        next.InGame = false;

        next = Team2[0];
        foreach (Player player in Team2)
        {
            if (player.InGame)
            {
                if (!next.InGame)
                {
                    next = player;
                }
                if (player.Played > next.Played)
                {
                    next = player;
                }
                if (player.Played == next.Played && player.Rank > next.Rank)
                {
                    next = player;
                }
            }
        }
        next.InGame = false;
    }

    private void Validate_FullTeam()
    {
        int OnCourt = 0;
        foreach (Player player in Team1)
        {
            if (player.InGame)
            {
                OnCourt++;
            }
        }
        if (OnCourt != team.TeamMates)
        {
            throw new Exception();
        }
        OnCourt = 0;
        foreach (Player player in Team2)
        {
            if (player.InGame)
            {
                OnCourt++;
            }
        }
        if (OnCourt != team.TeamMates)
        {
            throw new Exception();
        }
    }
    private void Validate_RotateOut()
    {
        int OnCourt = 0;
        foreach (Player player in Team1)
        {
            if (player.InGame)
            {
                OnCourt++;
            }
        }
        if (OnCourt != team.TeamMates -1)
        {
            throw new Exception();
        }
        OnCourt = 0;
        foreach (Player player in Team2)
        {
            if (player.InGame)
            {
                OnCourt++;
            }
        }
        if (OnCourt != team.TeamMates - 1)
        {
            throw new Exception();
        }
    }
}

class Program
{
    static League league = new League();
    static string[] lines = new String[6];

    static void Main()
    {
        // Read in every line in the file.
        int rowNumber = 0;

        using (StreamReader reader = new StreamReader("../../input.txt"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Array.Resize(ref lines, rowNumber + 1);
                lines[rowNumber] = line;
                rowNumber++;
            }
        }

        int caseNum = 1;

        league.Entries = Convert.ToInt32(lines[0]);
        for (int i = 1; i < rowNumber; i++)
        {
            string[] gameStats = lines[i].Split(' ');

            Team team = new Team();
            team.Entries = Convert.ToInt32(gameStats[0]);
            team.Minutes = Convert.ToInt32(gameStats[1]);
            team.TeamMates = Convert.ToInt32(gameStats[2]);

            for (int j = 0; j < team.Entries; j++)
            {
                i++;

                string[] playerStats = lines[i].Split(' ');

                Player player = new Player();

                player.Name = playerStats[0];
                player.Shooting = Convert.ToInt32(playerStats[1]);
                player.Height = Convert.ToInt32(playerStats[2]);

                team.Players.Add(player);

            }
            league.Teams.Add(team);
            Game game = new Game(team);
            game.Rotate();
            game.PlayerList();
            Console.Write("Case #" + caseNum + ": ");
            WriteAnswer(game);
            Console.WriteLine();
            caseNum++;
        }
        //dumpObjects();
        Console.ReadLine();
    }

    static void WriteAnswer(Game game)
    {
        foreach (string name in game.players)
        {
            Console.Write(name + " ");
        }
    }

    static void dumpObjects()
    {
        Console.WriteLine(league.Entries);
        foreach (Team team in league.Teams)
        {
            Console.Write(team.Entries + ",");
            Console.Write(team.Minutes + ",");
            Console.Write(team.TeamMates + ",");
            Console.WriteLine();
            foreach (Player player in team.Players)
            {
                Console.Write(player.Name + ",");
                Console.Write(player.Shooting + ",");
                Console.Write(player.Height + ",");
                Console.Write(player.Rank + ",");
                Console.Write(player.Played + ",");
                Console.Write(player.InGame + ",");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
