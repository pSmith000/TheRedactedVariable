using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TheRedactedVariable
{
    class Player : Entity
    {
        //Initializing the Player variables
        private float _sanity;
        private float _infection;
        private Item[] _items;
        private Item _currentItem;
        private int _currentItemIndex;
        private string _job;

        /// <summary>
        /// A property to read the player sanity
        /// </summary>
        public float Sanity
        {
            get { return _sanity; }
        }

        /// <summary>
        /// A property to read the player infection levels, or [REDACTED]
        /// </summary>
        public float Infection
        {
            get { return _infection; }
        }

        /// <summary>
        /// Override property that changes the attack power of the player based on
        /// what item the player is holding
        /// </summary>
        public override float AttackPower
        {
            get
            {
                if (_currentItem.Type == ItemType.ATTACK)
                {
                    return base.AttackPower + CurrentItem.StatBoost;
                }
                return base.AttackPower;
            }

        }

        /// <summary>
        /// Override property that changes the defense power of the player based on
        /// what item the player is holding
        /// </summary>
        public override float DefensePower
        {
            get
            {
                if (_currentItem.Type == ItemType.DEFENSE)
                {
                    return base.DefensePower + CurrentItem.StatBoost;
                }
                return base.DefensePower;

            }
        }

        /// <summary>
        /// A property to read the players current item
        /// </summary>
        public Item CurrentItem
        {
            get { return _currentItem; }

        }

        /// <summary>
        /// A property to read and set the players job
        /// </summary>
        public string Job
        {
            get
            {
                return _job;
            }
            set
            {
                _job = value;
            }
        }

        /// <summary>
        /// A new instance of the player with base stats
        /// </summary>
        public Player()
        {
            _items = new Item[0];
            _currentItem.Name = "Nothing";
            _currentItemIndex = -1;
        }

        /// <summary>
        /// A new player with items. Used to make a new player in the load function
        /// </summary>
        /// <param name="items">the items that the player has</param>
        public Player(Item[] items) : base()
        {
            _currentItem.Name = "Nothing";
            _items = items;
            _currentItemIndex = -1;
        }

        /// <summary>
        /// Used to create a player based on the character that they select
        /// </summary>
        /// <param name="name"></param>
        /// <param name="health"></param>
        /// <param name="attackPower"></param>
        /// <param name="defensePower"></param>
        /// <param name="sanity"></param>
        /// <param name="infection"></param>
        /// <param name="items"></param>
        /// <param name="job"></param>
        public Player(string name, float health, float attackPower, float defensePower, float sanity, float infection, Item[] items, string job) : base(name, health, attackPower, defensePower)
        {
            _items = items;
            _currentItem.Name = "Nothing";
            _job = job;
            _currentItemIndex = -1;
            _sanity = sanity;
            _infection = infection;
        }

        /// <summary>
        /// Takes away sanity from the player by a certain amount
        /// </summary>
        /// <param name="damageAmount">the amount you want to decrease sanity by</param>
        /// <returns></returns>
        public float LoseSanity(float damageAmount)
        {
            float damageTaken = Sanity - damageAmount;

            _sanity = damageTaken;

            if (_sanity > 100)
            {
                _sanity = 100;
            }

            return damageTaken;
        }

        /// <summary>
        /// Adds infection levels to the player by a certain amount
        /// </summary>
        /// <param name="damageAmount">the amount you want to incerease infection by</param>
        /// <returns></returns>
        public float GainInfection(float damageAmount)
        {
            float damageTaken = Infection + damageAmount;

            _infection = damageTaken;

            if (_infection < 0)
            {
                _infection = 0;
            }

            return damageTaken;
        }

        /// <summary>
        /// Sets the item at the given index to be the current item
        /// </summary>
        /// <param name="index">the index of the item in the array</param>
        /// <returns>false if the index is outside the bounds of the array</returns>
        public bool TryEquipItem(int index)
        {
            //If the index is out of bounds...
            if (index >= _items.Length || index < 0)
            {
                //...return false
                return false;
            }

            _currentItemIndex = index;

            //Set the current item to be the array at the given index
            _currentItem = _items[_currentItemIndex];

            return true;
        }

        /// <summary>
        /// Set the current item to be nothing
        /// </summary>
        /// <returns>false if there is no item equipped</returns>
        public bool TryRemoveCurrentItem()
        {
            //Returns false if there is no item equipped
            if (CurrentItem.Name == "Nothing")
            {
                return false;
            }

            _currentItemIndex = -1;

            //Set item to be nothing
            _currentItem = new Item();
            _currentItem.Name = "Nothing";
            return true;
        }

        /// <returns>Gets the names of all the items in the player inventory</returns>
        public string[] GetItemNames()
        {
            string[] itemNames = new string[_items.Length];

            for (int i = 0; i < _items.Length; i++)
            {
                itemNames[i] = _items[i].Name;
            }

            return itemNames;


        }

        /// <summary>
        /// Saves the player specific variables as well as the base save
        /// </summary>
        /// <param name="writer"></param>
        public override void Save(StreamWriter writer)
        {
            writer.WriteLine(_job);
            base.Save(writer);
            writer.WriteLine(_currentItemIndex);
            writer.WriteLine(_sanity);
            writer.WriteLine(_infection);
        }

        /// <summary>
        /// Loads the player specific variables as well as the base load
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override bool Load(StreamReader reader)
        {
            //if the base loading function fails...
            if (!base.Load(reader))
            {
                //...return false
                return false;
            }

            //If the current line can't be converte into an int...
            if (!int.TryParse(reader.ReadLine(), out _currentItemIndex))
            {
                //...return false
                return false;
            }
            //If the current line can't be converte into an int...
            if (!float.TryParse(reader.ReadLine(), out _sanity))
            {
                //...return false
                return false;
            }
            //If the current line can't be converte into an int...
            if (!float.TryParse(reader.ReadLine(), out _infection))
            {
                //...return false
                return false;
            }

            //Return true
            return true;
        }
    }
}
