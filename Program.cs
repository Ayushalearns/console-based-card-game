//namespaces 
using System;
using System.IO;
using System.Collections.Generic;

//main class of the program
class Program
{
    static void Main(string[] Sargs)
    {
        Game game = new Game();
        game.Start();
    }
}

//manages game
class Game
{
    //holds game participants
    private Player player;
    private Computer computer;

    //declaration of 3 rounds of play
    private const int Irounds = 3;

    //stores game log
    private readonly string SlogFile = "game_log.txt";

    //used to generate random numbers
    private readonly Random random = new Random();

    //game's core
    public void Start()
    {
        //starting point
        Console.WriteLine("==========================================================================================|");
        Console.Write("Enter your name: ");
        string Sname = Console.ReadLine();
        
        while (string.IsNullOrWhiteSpace(Sname))
        {
            //In case the name is left empty 
            Console.WriteLine("ERROR HAS OCCURRED!! ");
            Console.Write("Enter your name: ");
            Sname = Console.ReadLine();
        }

        player = new Player(Sname);
        computer = new Computer("Computer");
        Console.WriteLine("==========================================================================================|");
        Console.WriteLine("Hello " + Sname);
        Console.WriteLine("\nGAME CONTENT");
        Console.WriteLine("At first, you are assigned with 2 random number and the participant with the lowest difference between the numbers wins.");
        Console.WriteLine("If you wish to swap any of the random numbers type 'yes' and if not type 'no'.");
        Console.WriteLine("In case you choose yes for swaping, choose the number you want to swap (Either 1 or 2)");
        Console.WriteLine("you are allowed to swap twice(only if the difference is two or less)");
        Console.WriteLine("==========================================================================================|\n");


        //announcement of beginning of the game
        File.WriteAllText(SlogFile, "--- Game Start ---\n");
        Console.WriteLine("****************************** Let The Games Begin ***************************************|\n");

        //for loop used to loop for 3 rounds
        for (int Iround = 1; Iround <= Irounds; Iround++)
        {
            Console.WriteLine("------------------------------------------------------------------------------------------|");
            //displays the running round
            Console.WriteLine($"ROUND {Iround} ");

            DealCards(player);
            DealCards(computer);

            //display the random cards assigned to the player
            Console.WriteLine($"Your cards: {player.Icards[0]} and {player.Icards[1]}");
            Console.WriteLine($"Your score: {player.Iscore()}");

            //initial cards and score is logged
            Log($"Round {Iround} - {player.Sname} initial cards: {player.Icards[0]}, {player.Icards[1]} (Score: {player.Iscore()})");
            Log($"Round {Iround} - {computer.Sname} initial cards: {computer.Icards[0]}, {computer.Icards[1]} (Score: {computer.Iscore()})");

            PlayerTurn();
            ComputerTurn();

            //saves the data of that round
            player.SaveRound();
            computer.SaveRound();
        }

        ShowResults();
    }

    //here 2 random cards are assigned to the player
    private void DealCards(Player p)
    {
        p.Icards[0] = random.Next(1, 9);
        do
        {
            //prevent duplicates
            p.Icards[1] = random.Next(1, 9);
        } while (p.Icards[1] == p.Icards[0]);
    }

    private void PlayerTurn()
    {
        //swap counter
        int IswapAttempts = 0;

        //for card swap for player
        //while loop started to allow the swapping of card
        while (player.Iscore() >= 3 && IswapAttempts < 2)
        {
            Console.Write("Do you want to swap one card? (yes/no): ");
            string Schoice = Console.ReadLine()?.Trim().ToLower();

            while (Schoice != "yes" && Schoice != "no")
            {
                //In case of invalid input
                Console.WriteLine("INVALID INPUT!!");
                Console.Write("Please enter 'yes' or 'no': ");
                Schoice = Console.ReadLine()?.Trim().ToLower();
            }

            if (Schoice == "no")
            {
                Log($"{player.Sname} chose not to swap.");
                break;
            }

            Console.Write("Which card to swap? Enter 1 or 2: ");
            string Sinput = Console.ReadLine();
            int Iindex;

            while (!int.TryParse(Sinput, out Iindex) || Iindex < 1 || Iindex > 2)
            {
                Console.Write("Invalid choice. Please enter 1 or 2: ");
                Sinput = Console.ReadLine();
            }

            int cardIndex = Iindex - 1;
            //stores the swapped value
            int oldValue = player.Icards[cardIndex];
            int otherCard = player.Icards[1 - cardIndex];
            int newValue;

            do
            {
                //prevent duplicates
                newValue = random.Next(1, 9);
            } while (newValue == oldValue || newValue == otherCard);

            player.Icards[cardIndex] = newValue;

            //displays the swapped data 
            Console.WriteLine($"You swapped card {Iindex} from {oldValue} to {newValue}");
            Console.WriteLine($"New cards: {player.Icards[0]} and {player.Icards[1]}");
            Console.WriteLine($"New score: {player.Iscore()}");

            Log($"{player.Sname} swapped card {Iindex} (was {oldValue}, now {newValue})");

            IswapAttempts++;
        }

        if (IswapAttempts == 0 && player.Iscore() < 3)
        {
            Log($"{player.Sname} could not swap due to low score.");
        }
        else if (IswapAttempts == 2)
        {
            Log($"{player.Sname} reached maximum allowed swaps.");
        }
    }

    private void ComputerTurn()
    {
        //swap attempt counter
        int Iattempts = 0;
        while (computer.Iscore() >= 3 && Iattempts < 2)
        {
            int Iindex = computer.Icards[0] >= computer.Icards[1] ? 0 : 1;
            //IoldValue is the card being swapped
            int IoldValue = computer.Icards[Iindex];
            int IotherValue = computer.Icards[1 - Iindex];

            //generates new random number
            int InewValue;
            do
            {
                //prevents duplicates
                InewValue = random.Next(1, 9);
            } while (InewValue == IoldValue || InewValue == IotherValue);

            computer.Icards[Iindex] = InewValue;
            Log($"Computer swapped card {Iindex + 1} (was {IoldValue}, now {InewValue})");
            Iattempts++;
        }

        if (Iattempts == 0)
        {
            Log("Computer chose not to swap.");
        }
    }

    private void ShowResults()
    {
        //declaration of end of game
        Console.WriteLine("\n==========================================================================================|");
        Console.WriteLine("************************************ Game Over *******************************************|\n");
        Log("\n *** Game Results ***");

        //tracks the total of both participants
        int IplayerTotal = 0;
        int IcomputerTotal = 0;
        Console.WriteLine("------------------------------------------------------------------------------------------|");
        Console.WriteLine("************************************ Results *********************************************|");
        Console.WriteLine("------------------------------------------------------------------------------------------|");

        //results of each round
        for (int i = 0; i < Irounds; i++)
        {
            //retrives scores from score list
            int Ips = player.RoundScores[i];
            int Ics = computer.RoundScores[i];

            //displays current round number
            Console.WriteLine($"Round {i + 1}:");

            //print players name, their cards and score
            Console.WriteLine($"{player.Sname} - Cards: {player.RoundCards[i][0]}, {player.RoundCards[i][1]} | Score: {Ips}");
            Console.WriteLine($"Computer - Cards: {computer.RoundCards[i][0]}, {computer.RoundCards[i][1]} | Score: {Ics}");

            //adds current round score to the total score
            IplayerTotal += Ips;
            IcomputerTotal += Ics;
            Console.WriteLine("==========================================================================================|\n");
        }
        Console.WriteLine("==========================================================================================|");

        //participants total score is logged to the logfile
        Log($"{player.Sname} total score: {IplayerTotal}");
        Log($"Computer total score: {IcomputerTotal}");

        //displays final score
        Console.WriteLine($"\nFinal Scores:\n{player.Sname}: {IplayerTotal}\nComputer: {IcomputerTotal}");
        Console.WriteLine("------------------------------------------------------------------------------------------|");

        //final verdict declaration
        //if condition is applied here to find out who the winner is
        if (IplayerTotal < IcomputerTotal)
        {
            //if the player wins
            Console.WriteLine($"{player.Sname} WINS!");
            Log("Result: Player wins.");
        }
        else if (IplayerTotal > IcomputerTotal)
        {
            //if the computer wins
            Console.WriteLine("Computer WINS!");
            Log("Result: Computer wins.");
        }
        else
        {
            //if it's a draw
            int IplayerSum = player.ITotalCardSum();
            int IcomputerSum = computer.ITotalCardSum();
            Console.WriteLine("It was a draw!\n");
            Console.WriteLine("Applying tiebreaker...");

            //tie breaker system
            if (IplayerSum < IcomputerSum)
            {
                //if the player wins after the lowest card sum
                Console
                    .WriteLine($"{player.Sname} wins by lowest card sum!");
                Log("Result: Player wins by lowest card sum.");
            }
            else if (IplayerSum > IcomputerSum)
            {
                //if the computer wins after the lowest card sum
                Console.WriteLine("Computer wins by lowest card sum!");
                Log("Result: Computer wins by lowest card sum.");
            }
            else
            {
                //if the result is a tie even after the tie breaker
                Console.WriteLine("It's a ultimate tie!");
                Log("Result: Complete tie - both players have identical scores and card sums.");
            }
        }
        Console.WriteLine("==========================================================================================|\n");
        Console.WriteLine("\nGame log written to: game_log.txt");
    }

    private void Log(string Smessage)
    {
        //appends the Smessage to the logfile
        File.AppendAllText(SlogFile, Smessage + "\n");
    }
}

//stores information about participants
class Player
{
    
    public string Sname { get; set; }

    //stores the two cards the player usually holds
    public int[] Icards { get; set; } = new int[2];

    //stores all the set of cards of each round
    public List<int[]> RoundCards { get; private set; } = new List<int[]>();

    //Stores the calculated score of each round.
    public List<int> RoundScores { get; private set; } = new List<int>();

    public Player(string Sname)
    {
        this.Sname = Sname;
    }

    //saves the cards of current round and the score 
    public void SaveRound()
    {
        RoundCards.Add(new int[] { Icards[0], Icards[1] });
        RoundScores.Add(Iscore());
    }

    //Calculates and returns the score for the current round
    public int Iscore()
    {
        //absolute difference is used
        return Math.Abs(Icards[0] - Icards[1]);
    }

    //calculates and returns the sum of all the cards throughout all rounds
    public int ITotalCardSum()
    {
        int Isum = 0;
        foreach (var round in RoundCards)
        {
            Isum += round[0] + round[1];
        }
        return Isum;
    }
}

//uses all the methods and properties similar to class player (only difference is name and behaviour logic
class Computer : Player
{
    public Computer(string Sname) : base(Sname) { }
}