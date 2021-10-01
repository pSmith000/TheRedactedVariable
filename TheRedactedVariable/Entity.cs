using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TheRedactedVariable
{
    class Entity
    {
        //Initializeing the entity variables
        private string _name;
        private float _health;
        private float _attackPower;
        private float _defensePower;

        /// <returns>Gets the name of the entity</returns>
        public string Name
        {
            get { return _name; }
        }

        /// <returns>Gets the health of the entity</returns>
        public float Health
        {
            get { return _health; }
        }

        /// <returns>Gets the attack power of the entity</returns>
        public virtual float AttackPower
        {
            get { return _attackPower; }
        }

        /// <returns>Gets the defense power of the entity</returns>
        public virtual float DefensePower
        {
            get { return _defensePower; }
        }

        /// <summary>
        /// A base entity with base values
        /// </summary>
        public Entity()
        {
            _name = "Default";
            _health = 0;
            _attackPower = 0;
            _defensePower = 0;
        }

        /// <summary>
        /// Used to make specific entities
        /// </summary>
        /// <param name="name"></param>
        /// <param name="health"></param>
        /// <param name="attackPower"></param>
        /// <param name="defensePower"></param>
        public Entity(string name, float health, float attackPower, float defensePower)
        {
            _name = name;
            _health = health;
            _attackPower = attackPower;
            _defensePower = defensePower;
        }

        /// <summary>
        /// Takes damage not based on attack power or defense power
        /// </summary>
        /// <param name="damageAmount">the amount of damage you want the entity/player to take</param>
        /// <returns></returns>
        public float TakeDamage(float damageAmount)
        {
            float damageTaken = Health - damageAmount;

            _health = damageTaken;

            if (_health > 200)
            {
                _health = 200;
            }

            return damageTaken;
        }

        /// <summary>
        /// Takes damage based on the defense power of the entity being attacked
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <returns></returns>
        public float TakeDamageFromBoss(float damageAmount)
        {
            float damageTaken = damageAmount - DefensePower;

            if (damageTaken < 0)
            {
                damageTaken = 0;
            }

            _health -= damageTaken;

            return damageTaken;
        }

        /// <summary>
        /// Attacks a certain entity and goes into the take damage function
        /// </summary>
        /// <param name="defender"></param>
        /// <returns></returns>
        public float Attack(Entity defender)
        {
            return defender.TakeDamageFromBoss(AttackPower);
        }

        /// <summary>
        /// Saves the entity variables
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Save(StreamWriter writer)
        {
            writer.WriteLine(_name);
            writer.WriteLine(_health);
            writer.WriteLine(_attackPower);
            writer.WriteLine(_defensePower);
        }

        /// <summary>
        /// Loads the entity variables
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public virtual bool Load(StreamReader reader)
        {
            _name = reader.ReadLine();

            if (!float.TryParse(reader.ReadLine(), out _health))
            {
                return false;
            }

            if (!float.TryParse(reader.ReadLine(), out _attackPower))
            {
                return false;
            }

            if (!float.TryParse(reader.ReadLine(), out _defensePower))
            {
                return false;
            }

            return true;
        }

    }

}