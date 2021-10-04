using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace TheRedactedVariable
{
    /// <summary>
    /// An enum for the type of item, 0 = None / 1 = Defense / 2 = Attack
    /// </summary>
    public enum ItemType
    {
        NONE,
        DEFENSE,
        ATTACK
    }

    /// <summary>
    /// A struct foor the items the player can equip
    /// </summary>
    public struct Item
    {
        public string Name;
        public float StatBoost;
        public ItemType Type;
    }
    class Game
    {
        //Initializes the main variables for the game class
        private int _currentScene = 0;
        private bool _gameOver;
        private float _currentEnemyHealth;
        private Player _player;
        private Entity _currentEnemy;
        private string _playerName;
        private Item[] _items;

        public void Run()
        {
            //The main game loop
            Start();

            //while the game is over...
            while (!_gameOver)
            {
                //...update the game
                Update();
            }

            //Goodbye message
            End();
        }

        /// <summary>
        /// Used to initialize the items and enemies
        /// </summary>
        public void Start()
        {
            InitializeItems();
        }

        /// <summary>
        /// Displays the current scene and gets updated in the main game loop
        /// </summary>
        public void Update()
        {
            DisplayCurrentScene();
        }

        /// <summary>
        /// The ending message after the game is over
        /// </summary>
        public void End()
        {
            TypeOutWords("'You can escape my grasp... for now.'", 75);
        }

        /// <summary>
        /// Saves the current scene and then uses the player.save and entity.save functions
        /// </summary>
        public void Save()
        {
            //Creates a new stream writer 
            StreamWriter writer = new StreamWriter("SaveData.txt");

            //Saves the current scene
            writer.WriteLine(_currentScene);

            //Uses the player save function
            _player.Save(writer);

            //Uses the entity save function
            _currentEnemy.Save(writer);

            //Closes the writer
            writer.Close();
        }

        /// <summary>
        /// Loads the current scene and job, then uses player.load and entity.load
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            //Create a new bool
            bool loadSuccessful = true;

            //If the save file does not exist...
            if (!File.Exists("SaveData.txt"))
            {
                //...return false
                return false;
            }

            //Creates a new stream reader
            StreamReader reader = new StreamReader("SaveData.txt");

            //If the next line isn't an integer...
            if (!int.TryParse(reader.ReadLine(), out _currentScene))
            {
                //...return false
                loadSuccessful = false;
            }

            //Sets the job to the next line read
            string job = reader.ReadLine();

            //Creates a new player with the base items
            _player = new Player(_items);

            //Sets the players job
            _player.Job = job;

            //If the player can't be loaded...
            if (!_player.Load(reader))
            {
                //...return false
                loadSuccessful = false;
            }

            //Sets the players name
            _playerName = _player.Name;

            //Creates a new entity
            _currentEnemy = new Entity();

            //If the current enemy can't be loaded...
            if (!_currentEnemy.Load(reader))
            {
                //...return false
                loadSuccessful = false;
            }

            //Close the reader
            reader.Close();

            //Return the bool
            return loadSuccessful;
        }



        /// <summary>
        /// Initializes the items and puts them in an array
        /// </summary>
        public void InitializeItems()
        {
            //Initializes the 3 items
            Item gun = new Item { Name = "the Handgun", StatBoost = 20, Type = ItemType.ATTACK };

            Item flashlight = new Item { Name = "the Flashlight", StatBoost = 10, Type = ItemType.DEFENSE };

            Item none = new Item { Name = "Nothing", StatBoost = 0, Type = ItemType.NONE };

            //Adds the items into an array
            _items = new Item[] { gun, flashlight, none };
        }

        /// <summary>
        /// Initializes the main enemy. The player will have to fight 1 of 3 enemies throughout the game.
        /// </summary>
        public void InitializeEnemy()
        {
            //The initializes enemies
            Entity cthulu = new Entity("Cthulu", 300, 35, 20);

            Entity god = new Entity("God", 150, 50, 40);

            Entity satan = new Entity("Satan", 200, 40, 30);

            //Random number generator between 1 and 3
            Random rnd = new Random();
            int num = rnd.Next(1, 4);

            //If 1...
            if (num == 1)
            {
                //...the enemy is Cthulu
                _currentEnemy = cthulu;
            }
            //If 2...
            else if (num == 2)
            {
                //...the enemy is God
                _currentEnemy = god;
            }
            //If 3...
            else if (num == 3)
            {
                //...the enemy is Satan
                _currentEnemy = satan;
            }
            //A variable used to see the original health of the boss
            _currentEnemyHealth = _currentEnemy.Health;
        }

        /// <summary>
        /// Displays the opening menu, allowing the player to start a new game or load an old game
        /// </summary>
        public void DisplayOpeningMenu()
        {
            //Asks the player if they want to start a new game or load an old one
            int choice = GetInput("Welcome to the game. Would you like to: \n", "Start a New Game", "Load an Old Save");

            //If the player chooses new game...
            if (choice == 0)
            {
                //...sets the current scene to the get the player name and character selection
                _currentScene = 1;
            }
            //If the player chooses load an old save...
            else if (choice == 1)
            {
                //...check to see if load is true...
                if (Load())
                {
                    //...if load is true then say message and clear
                    Console.WriteLine("Welcome back!");
                    Thread.Sleep(500);
                    Console.Clear();

                }
                else
                {
                    //...if load is false then say message and clear
                    Console.WriteLine("Load Failed.");
                    Thread.Sleep(500);
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Asks the player to input a name and confirms its the name the player wants
        /// </summary>
        void GetPlayerName()
        {
            bool validInputRecieved = true;
            while (validInputRecieved == true)
            {
                //Asks player to enter name
                TypeOutWords("Good choice! Now, please enter your name.\n> ", 50);
                _playerName = Console.ReadLine();
                Console.Clear();

                //Asks player to confirm name
                int choice = GetInput("You've entered " + _playerName + ", are you sure you want to keep this name?\n",
                    "Yes", "No");
                //If yes...
                if (choice == 0)
                {
                    //...break while loop
                    validInputRecieved = false;
                }
                //If no...
                else
                {
                    //...continue in while loop
                    validInputRecieved = true;
                }
            }

        }

        /// <summary>
        /// Gets the players choice of character. Updates player stats based on
        /// the character chosen.
        /// </summary>
        public void CharacterSelection()
        {
            int choice = GetInput("Nice to meet you " + _playerName + ". Please select a character.\n", "Brawler", "Mad Man", "Thief");

            if (choice == 0)
            {
                _player = new Player(_playerName, 150, 40, 15, 100, 0, _items, "Brawler");
            }
            else if (choice == 1)
            {
                _player = new Player(_playerName, 120, 25, 35, 85, 25, _items, "Mad Man");
            }
            else if (choice == 2)
            {
                _player = new Player(_playerName, 125, 25, 30, 95, 10, _items, "Thief");
            }

            _currentScene = 2;
        }

        public void DisplayCurrentScene()
        {
            switch (_currentScene)
            {
                case 0:
                    DisplayOpeningMenu();
                    break;
                case 1:
                    GetPlayerName();
                    CharacterSelection();
                    InitializeEnemy();
                    break;
                case 2:
                    Introduction();
                    break;
                case 3:
                    PrintPlayerStats();
                    FirstEncounter();
                    PlayerIsAlive();
                    break;
                case 4:
                    PrintPlayerStats();
                    BossBattle();
                    PlayerIsAlive();
                    break;
                case 5:
                    DisplayRestartMenu();
                    break;

            }
        }

        /// <summary>
        /// Displays the items available to equip and allows the player to equip
        /// </summary>
        public void DisplayEquipItemMenu()
        {
            //Get item index
            int choice = GetInput("Select an item to equip.\n", _player.GetItemNames());

            //Equip item at given index
            if (!_player.TryEquipItem(choice))
            {
                TypeOutWords("You couldn't find that item in your bag.", 50);
            }

            //Print feedback
            TypeOutWords("You equipped " + _player.CurrentItem.Name + "!", 50);
        }

        /// <summary>
        /// The introduction sequence of the game
        /// </summary>
        public void Introduction()
        {
            Console.Write("Autosaving ");

            //This for loop prints out numbers 1-100% to show a fake loading screen
            for (int i = 0; i < 101; i++)
            {
                //The random number 1-75 is for the time it takes in ms
                //to change between each number
                Random rnd = new Random();
                int num = rnd.Next(1, 75);
                Console.Write(i + "%");
                Thread.Sleep(num);
                Console.SetCursorPosition(11, 0);
                Console.Write(" ");
            }

            //The console waits, clears, and saves the game. 
            //Acting as an actual autosave point
            Thread.Sleep(1000);

            Console.Clear();

            Save();

            TypeOutWords("Game Saved", 50);

            Thread.Sleep(500);

            Console.Clear();

            TypeOutWords("Now that the boring things are over let me introduce myself.\n", 50);
            TypeOutWords(". . .", 150);
            TypeOutWords("I am your enemy", 50);

            Thread.Sleep(400);

            TypeOutWords("\nNow... be rebooted into my world", 100);

            Thread.Sleep(400);

            TypeOutWords("dddddddddddrebootworld\n" + _playerName + "         ootintomyworld\nmyworldmyworld     ERROR\n       orldREB0OT", 10);

            //This for loop is for the 'error' screen that pops up at this point in the game
            for (int i = 0; i < 99; i++)
            {
                //Installing infection pops up and counts to 98 percent
                //to fake a virus loading into the computer
                Random rnd = new Random();
                int num = rnd.Next(1, 75);
                Console.Write("INSTALLING INFECTION ");
                Console.Write(i + "%");
                Thread.Sleep(num);
                Console.SetCursorPosition(16, 8);
                Console.Write(" ");
            }

            //The console waits after this 'error' screen then finishes off the introduction to the game
            Thread.Sleep(1500);

            Console.Clear();

            TypeOutWords(".....", 150);
            TypeOutWords("\n'It's time to wake up'", 50);
            TypeOutWords("\n\nYou hear a voice and open your eyes. You awaken to an empty, dimly lit room" +
                "\nThere is only one door and a table in front of you with a handgun, a flashlight, and a note on it. The note says" +
                "\n'Survive for me. You must always move forward.'\n\nYou pick up the flashlight and handgun. \n\n", 35);

            _currentScene = 3;
        }

        /// <summary>
        /// This is the first encounter menu, where the player decides to either psuh forward, equip/remove an item, save the game or run away
        /// </summary>
        public void FirstEncounter()
        {
            //Asks the player what they would like to do
            int choice = GetInput("What would you like to do?\n", "Push Forward", "Equip Item", "Remove Item", "Save Game", "Run Away");

            //If they push forward...
            if (choice == 0)
            {
                //...randomly select a room to go to
                RoomJourney();
            }
            //If they equip an item...
            else if (choice == 1)
            {
                //...display the equip item menu
                DisplayEquipItemMenu();
                Thread.Sleep(500);
                Console.Clear();
            }
            //If they remove an item...
            else if (choice == 2)
            {
                //...try to remove the current item
                if (!_player.TryRemoveCurrentItem())
                {
                    TypeOutWords("You don't have anything equipped.", 50);
                }
                else
                {
                    TypeOutWords("You placed the item in your bag", 50);
                }
                Thread.Sleep(500);
                Console.Clear();
            }
            //If they save the game...
            else if (choice == 3)
            {
                //...save the game
                Save();
                TypeOutWords("Saved Game", 50);
                Thread.Sleep(500);
                Console.Clear();
            }
            //If they run away...
            else if (choice == 4)
            {
                //...ask if they are sure...
                Console.Clear();
                int choice1 = GetInput("Are you sure you want to run away?\n", "Yes", "No");

                if (choice1 == 0)
                {
                    //...trigger the boss battle if they run away
                    TypeOutWords("'It is time child'\n\n[REDACTED] stands before you. \n\n'It is time to die'\n", 50);
                    _currentScene = 4;
                }


            }


        }

        /// <summary>
        /// Holds all of the rooms that the player can get randomly put in to
        /// </summary>
        public void RoomJourney()
        {
            //Random number generator between 1 and 85, with the rooms being a percentage of what you can get
            Random rnd = new Random();
            int room1 = rnd.Next(1, 86);

            if (room1 > 0 && room1 < 11)
            {
                //Each of these rooms have a specific scene within them
                //this could either be good or bad for the player
                //this is the first room

                TypeOutWords("You walk in to a dimly lit room. A man stands before you bloodied and bruised." +
                    "\n He holds a sword in his hand. He slowly faces you and says 'r-r-re b-boot?' The man then" +
                    "\n lunges at you.\n\n", 15);
                if (_player.CurrentItem.Type == ItemType.ATTACK)
                {
                    TypeOutWords("You pull out your handgun and shoot the man in the chest. He pulls back and tilts his head at you." +
                        "\n're...boot...' He turns away and as he does his body siezes and falls to the ground. He seems dead.\n", 20);
                    TypeOutWords("SUSTAIN 5 SANITY LOSS", 50);
                    _player.GainInfection(5);
                    _player.LoseSanity(5);
                }
                else
                {
                    TypeOutWords("The man swings his sword widly at you. With no weapon yourself you are defenseless." +
                        "\nHe madly slashes your chest and arms screaming the same word over and over again." +
                        "\nre..boot   rebOOT   REBOOT   ??REE??BOOTT?????REEEB?OO?TTT\n He collapses to the floor in the middle of one of his swings at you.\n", 25);
                    TypeOutWords("You take severe injuries\n", 50);
                    TypeOutWords("SUSTAIN 35 DAMAGE\n", 50);
                    TypeOutWords("SUSTAIN 15 SANITY LOSS", 50);
                    _player.GainInfection(5);
                    _player.TakeDamage(35);
                    _player.LoseSanity(15);
                }
            }
            //The second room
            else if (room1 > 10 && room1 < 21)
            {
                TypeOutWords("You walk in to a room with a roaring fireplace and lush carpets. " +
                    "\nOn the table you find a healing potion and drink it, gaining 20 health!", 25);
                _player.TakeDamage(-20);
            }
            //the third room
            else if (room1 > 20 && room1 < 31)
            {
                TypeOutWords("You enter in a sanctuary of sorts. There are pews, symbolic pieces, and scripture on the walls. It feels safe. " +
                    "\nYou regain 20 sanity.", 25);
                _player.LoseSanity(-20);

            }
            //the fourth room
            else if (room1 > 30 && room1 < 41)
            {
                TypeOutWords("You walk in to a room that smells strongly of iron. It is too dark to see but the walls are covered in a warm liquid. \n" +
                    "While it does make you nauseous, you are safe.\n", 25);
                TypeOutWords("SUSTAIN 30 SANITY LOSS", 50);
                _player.LoseSanity(30);
            }
            //the fifth room
            else if (room1 > 40 && room1 < 51)
            {
                TypeOutWords("You stumble into a long hallway that splits off into two different paths. One on the left, and one on the right." +
                    "\n", 25);

                GetInput("Which path would you like to take?\n", "Left", "Right");

                //This random number is for this room alone. There is a 50/50 choice the player must make and every time
                //they come into this room the choice will be different
                Random random = new Random();
                int num = random.Next(1, 3);

                if (num == 1)
                {
                    TypeOutWords("You take this path and walk along it for some time. As you go the walls get closer and closer to you. " +
                        "\nYou look back for an escape but the floor crumbles to a black pit behind you. You start to panic and sprint forward." +
                        "\nYou stumble... landing on the floor. You look up and meet the eyeless gaze of [REDACTED]. Hope is fleeting.", 25);
                    Thread.Sleep(400);
                    TypeOutWords("Press Enter to Continue", 50);
                    Console.ReadKey(true);
                    Console.Clear();
                    TypeOutWords("'Fall'\n", 150);
                    Thread.Sleep(400);
                    TypeOutWords("A demonically similar voice.\n\n", 100);
                    TypeOutWords("The ground crumbles beneath you and you fall", 50);
                    Console.SetCursorPosition(0, 0);
                    TypeOutWords("fallfallfallfallfallfallfallfallfallfallfall", 150);
                    Thread.Sleep(400);
                    Console.Clear();
                    TypeOutWords("You hit the ground with a thud. You open your eyes and you are in an empty concrete room with a door in front of you." +
                        "\n\nSUSTAIN 40 DAMAGE\nSUSTAIN 20 SANITY LOSS", 50);

                    _player.TakeDamage(40);
                    _player.LoseSanity(20);
                    _player.GainInfection(15);
                }
                else
                {
                    TypeOutWords("You stumble down this path for a few minutes. It opens up to a large elegant room with a pool in the center." +
                        "\nAs you set into the pool you immediately feel refreshed. You heal 15 health and 10 sanity.", 50);
                    _player.TakeDamage(-15);
                    _player.LoseSanity(-10);
                }
            }
            //the sixth room
            else if (room1 > 50 && room1 < 61)
            {
                TypeOutWords("Walking into this room you smell tar and asphalt. The air is humid and thick. Breathing is an almost impossible task. " +
                    "\nYou see a vial of purple sludge on the ground half spilled over. You inspect it. \n", 25);

                int input = GetInput("Would you like to drink?\n", "Yes", "No");

                if (input == 0)
                {
                    TypeOutWords("You decide to drink this putrid liquid. As it goes down your esophagus it has the taste of rotting flesh " +
                        "\nand the consistancy of spoiled milk." +
                        "\nImmediately you vomit. What comes out is a viscous black liquid. As this matter leaves your body, " +
                        "\nyour mind feels less cluttered.", 25);
                    _player.GainInfection(-15);
                }

                else if (input == 1)
                {
                    TypeOutWords("You use your better judgement and decide not to drink the vial of mysterious liquid." +
                        "\nYou move forward.", 25);
                    _player.GainInfection(5);
                }
            }
            //the seventh room
            else if (room1 > 60 && room1 < 71)
            {
                TypeOutWords("You open the next door and inside you see ", 25);

                if (_player.CurrentItem.Type == ItemType.DEFENSE)
                {
                    TypeOutWords("a hideous creature shrouded in the darkness. You try to shine your flashlight directly at it, but it's gone." +
                        "\nAs you walk in you feel a drip on your shoulder and look up. Your eyes meet a souless, black grin. " +
                        "\nIt's skin hangs loosely from its bones as its head spins slowly towards you. You become paralyzed with fear." +
                        "\n\nAs you try to take a step back the creature falls upon you, scratching at your face as it tries to grab it." +
                        "\nYou rush forward to the next door and before you open it you look back for the creature, but see nothing." +
                        "\nIt is gone.\n\nSUSTAIN 30 SANITY LOSS\nSUSTAIN 15 DAMAGE", 25);
                    _player.TakeDamage(15);
                    _player.LoseSanity(30);
                    _player.GainInfection(8);
                }
                else
                {
                    TypeOutWords("nothing. You think of taking out your flashlight but decide its better to have a weapon than light." +
                        "\nYou walk through the room and seemingly nothing happens. Your anxious, but alive.\n\nSUSTAIN 5 SANITY LOSS", 25);
                    _player.LoseSanity(5);
                }
            }
            //the eighth room
            else if (room1 > 70 && room1 < 81)
            {
                TypeOutWords("This room is beyond dark. ", 25);

                if (_player.CurrentItem.Type == ItemType.DEFENSE)
                {
                    TypeOutWords("You hold your flashlight infront of you and see small fleshy creatures run away from your light." +
                        "\nYou make it through the room with ease.", 25);
                }
                else if (_player.CurrentItem.Type == ItemType.ATTACK)
                {
                    TypeOutWords("You hold your gun infront of you and then feel scratches on your leg. " +
                        "\nYou feel something dense crawl up your leg, so you shoot your gun at it. The flash scares these creatures away," +
                        "\nleaving you with only light injuries.\n\nSUSTAIN 5 DAMAGE", 25);
                    _player.TakeDamage(5);

                }
                else
                {
                    TypeOutWords("You stumble your way through this room and trip over something squishy." +
                        "\nIt makes a high pitched noise and as you are getting up you feel many small, dense, fleshy creatures jump on you." +
                        "\nYou scramble to get up and rush to the next door, but they continue to scratch and slash you\n\nSUSTAIN 35 DAMAGE\nSUSTAIN 10 SANITY LOSS", 25);
                    _player.TakeDamage(35);
                    _player.LoseSanity(10);
                    _player.GainInfection(5);
                }
            }
            //This is the only other way to reach the boss without running away
            else if (room1 > 80 && room1 < 86)
            {
                TypeOutWords("'I see you'\n\n'You have made a grave mistake coming here " + _playerName + "'\n\n", 150);

                int choice = GetInput("'Do you wish to fight me?'\n", "Yes", "No");

                if (choice == 0)
                {
                    _currentScene = 4;
                    TypeOutWords("'It is time child'\n\n[REDACTED] stands before you. \n\n'It is time to die'\n", 50);
                    return;
                }
                else
                {
                    TypeOutWords("'Then fall'", 75);
                    Console.SetCursorPosition(0, 6);
                    TypeOutWords("faLL", 75);
                    Console.SetCursorPosition(0, 6);
                    TypeOutWords("FALL", 75);
                    Thread.Sleep(400);
                    Console.Clear();
                    TypeOutWords("You fall into an abyss within your mind. Lifetimes pass before your eyes. " +
                        "\nExistence as you know it crumbles and reality shakes. A faint whisper..." +
                        "\n'It's time to wake up'\n", 50);
                    _player.GainInfection(15);
                }
            }
            Console.WriteLine("");

            TypeOutWords("Press ENTER to continue", 50);
            Console.ReadKey();
            Console.Clear();
        }

        /// <summary>
        /// This is where the player fights one of the three main bosses
        /// </summary>
        public void BossBattle()
        {
            //The player can attack, equip an item, run away, or save
            int choice = GetInput("\nWhat would you like to do?\n", "Attack", "Equip Item", "Run Away", "Save");

            //If the player attacks...
            if (choice == 0)
            {
                //...the player and boss attack and take damage
                float damageTaken = _player.Attack(_currentEnemy);
                TypeOutWords("You dealt " + damageTaken + " damage!\n", 45);

                damageTaken = _currentEnemy.Attack(_player);
                TypeOutWords("[REDACTED] has dealt " + damageTaken + ".", 45);

                //Every time the player fights they lose sanity and gain infection
                _player.GainInfection(5);
                _player.LoseSanity(5);

                Thread.Sleep(500);
                Console.Clear();
            }
            //If the player equips an item...
            else if (choice == 1)
            {
                //...they are shown the items they can equip
                DisplayEquipItemMenu();
                Console.ReadKey(true);
                Console.Clear();
            }
            //If the player tries to run away...
            else if (choice == 2)
            {
                //...they are only able to run away successfully if the boss is at half health
                if (_currentEnemy.Health <= _currentEnemyHealth / 2)
                {
                    TypeOutWords("You are able to run away. You go back through every door you have been to." +
                        "\nYou trudge through the dark and make your way back to the first room. There's a new door." +
                        "\nYou go to it and it opens to a bright and sunny field. You run through and everything is actually there." +
                        "\nYou touch the dewy grass with your bloodied fingertips. Your grasp the tall floawers growing all around." +
                        "\nYou feel the warmth all around you. You are happy. A familiar whisper...\n\n", 50);
                    TypeOutWords("'It's time to wake up'\n", 200);
                    TypeOutWords("Press ENTER to continue", 50);
                    Console.ReadKey();
                    Console.Clear();
                    _currentScene = 5;
                }
                else
                {
                    TypeOutWords("'You cannot escape'", 50);
                    Thread.Sleep(400);
                    Console.Clear();
                }
            }
            //If the player tries to save...
            else if (choice == 3)
            {
                //...they are not allowed
                TypeOutWords("You cannot save here, [REDACTED] has control.", 50);
                Thread.Sleep(400);
                Console.Clear();
            }

            //If the boss is dead...
            if (_currentEnemy.Health <= 0)
            {
                //...the player wins
                Console.Clear();
                TypeOutWords("'You have defeated me'\n\n'You have defeated " + _currentEnemy.Name + ". I am so proud of you child.'\n", 75);
                _currentScene = 5;
            }
        }

        /// <summary>
        /// Prints player stats neatly in a box
        /// </summary>
        void PrintPlayerStats()
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("Health: " + _player.Health);
            Console.WriteLine("Sanity: " + _player.Sanity);
            Console.WriteLine("[REDACTED]: " + _player.Infection);
            Console.WriteLine("----------------------------");
            Console.WriteLine("");
        }

        /// <summary>
        /// If player is dead it returns false
        /// If alive it returns true
        /// </summary>
        /// <returns></returns>
        bool PlayerIsAlive()
        {
            //If the player is dead...
            if (_player.Health <= 0 || _player.Sanity <= 0)
            {
                //...send them to the restart screen
                TypeOutWords("You have perished\n", 50);
                Thread.Sleep(400);
                _currentScene = 5;
                return false;
            }
            //If the player has over 100 infection or [REDACTED] stat...
            else if (_player.Infection >= 100)
            {
                //the player becomes 'infected'
                Console.WriteLine("[REDACTED] has corrupted your thoughts. It has been in your brain this whole time.");
                Console.WriteLine("The end truly was not the end. You have been...");
                Console.WriteLine("\nPress ENTER to become INFECTED");
                Console.ReadKey();

                Console.SetCursorPosition(0, 0);

                TypeOutWords("dddddddddddrebootworld\n" + _playerName + "         ootintomyworld\nmyworldmyworld     ERROR\n       orldREB0OT", 10);
                //Prints out an error screen
                for (int i = 0; i < 101; i++)
                {
                    Random rnd = new Random();
                    int num = rnd.Next(1, 75);
                    Console.Write("INSTALLING INFECTION ");
                    Console.Write(i + "%");
                    Thread.Sleep(num);
                    Console.SetCursorPosition(20, 8);
                    Console.Write(" ");
                }

                Thread.Sleep(1500);

                Console.Clear();

                _currentScene = 5;
            }
            return true;
        }

        /// <summary>
        /// Displays the restart menu and allows the player to restart
        /// </summary>
        void DisplayRestartMenu()
        {
            //Asks the player if they want to play again
            int choice = GetInput("Play Again?\n", "Yes", "No");

            //If the player says yes...
            if (choice == 0)
            {
                //...restart the game
                _currentScene = 1;
            }
            //If the player says no...
            else if (choice == 1)
            {
                //...end the game
                _gameOver = true;
            }
        }

        /// <summary>
        /// A custom function the prints a sentence to the console one letter at a time
        /// </summary>
        /// <param name="sentence">Whatever sentence that needs to be printed to the console</param>
        /// <param name="timeBetweenLetters">The time in milliseconds between each letter being printed</param>
        public void TypeOutWords(string sentence, int timeBetweenLetters)
        {
            //Creates a new array and sets it equal to each letter in the sentence
            char[] chars = sentence.ToCharArray();

            //For each letter in the sentence
            for (int i = 0; i < chars.Length; i++)
            {
                //Print the letter then wait
                Console.Write(chars[i]);
                Thread.Sleep(timeBetweenLetters);
            }
        }

        /// <summary>
        /// Gets an input from the player based on some given decision
        /// </summary>
        /// <param name="description">The context for the input</param>
        /// <param name="option1">The first option the player can choose</param>
        /// <param name="option2">The second option the player can choose</param>
        /// <returns></returns>
        int GetInput(string description, params string[] options)
        {
            string input = "";
            int inputRecieved = -1;

            while (inputRecieved == -1)
            {
                //Print options
                TypeOutWords(description, 50);

                for (int i = 0; i < options.Length; i++)
                {
                    TypeOutWords((i + 1) + "." + options[i], 30);
                    Console.WriteLine(" ");
                }
                TypeOutWords("> ", 50);

                //Get input from player
                input = Console.ReadLine();

                //If the player typed an int...
                if (int.TryParse(input, out inputRecieved))
                {
                    //...decrement the input and check if it's within the bounds of the array
                    inputRecieved--;
                    if (inputRecieved < 0 || inputRecieved >= options.Length)
                    {
                        //Set input recieved to be the default value
                        inputRecieved = -1;
                        //Display error message
                        Console.WriteLine("Invalid Input");
                        Console.ReadKey(true);
                    }
                }
                //If the player didn't type an int
                else
                {
                    //Set input recieved to be the default value
                    inputRecieved = -1;
                    Console.WriteLine("Invalid Input");
                    Console.ReadKey(true);
                }

                Console.Clear();
            }

            return inputRecieved;
        }
    }
}