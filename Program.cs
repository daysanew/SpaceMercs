using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceMercs
{
    class Program
    {
        static void Main(string[] args)
        {
            var armyOne = new List<Soldier>();
            var armyTwo = new List<Soldier>();
            var random = new Random();

            CreateArmy(armyOne);
            CreateArmy(armyTwo);

            var round = 1;
            armyOne = SortArmy(armyOne);
            armyTwo = SortArmy(armyTwo);

            var fightOver = false;
            var roundOver = false;
            while (!fightOver)
            {
                while (!roundOver)
                {
                    for (int i = 100; i > 0; i--)
                    {
                        while(true)
                        {
                            var attackingSoldiers = armyOne.Where(a => a.Initiative == i && a.CurrentActions > 0).ToList();
                            var defendingSoldiers = armyTwo.Where(a => a.Initiative == i && a.CurrentActions > 0).ToList();
                            
                            if(attackingSoldiers.Count > 0)
                            {
                                ArmyGroupAttack(attackingSoldiers, armyTwo, "Army Two", random, "Army One");
                            }
                            if(defendingSoldiers.Count > 0)
                            {
                                ArmyGroupAttack(defendingSoldiers, armyOne, "Army One", random, "Army Two");
                            }
                            if(attackingSoldiers.Count == 0 && defendingSoldiers.Count == 0)
                            {
                                break;
                            }
                        }

                        armyOne = RemoveDeadSoldiers(armyOne, "Army One");
                        armyTwo = RemoveDeadSoldiers(armyTwo, "Army Two");
                        if(armyOne.Count == 0 || armyTwo.Count == 0)
                        {
                            break;
                        }
                    }
                    roundOver = true;
                    Console.WriteLine($"****{round} round over. Army One Count: {armyOne.Count}. Army Two Count: {armyTwo.Count}****");
                    round++;
                }
                if(armyOne.Count == 0 || armyTwo.Count == 0 )
                {
                    fightOver = true;
                    Console.WriteLine($"###Fight over over. Army One Count: {armyOne.Count}. Army Two Count: {armyTwo.Count}###");
                }
                
                ResetActions(armyOne);
                ResetActions(armyTwo);
                roundOver = false;
            }

            Console.Read();
        }

        private static void ResetActions(List<Soldier> army)
        {
            foreach (var soldier in army)
            {
                soldier.CurrentActions = soldier.MaxActions;
            }
        }

        private static void ArmyGroupAttack(List<Soldier> attackingSoldiers, List<Soldier> defendingArmy, string defendingArmyName, Random random, string attackingArmyName)
        {
            foreach (var soldier in attackingSoldiers)
            {
                var attackPosition = random.Next(defendingArmy.Count);
                var defender = defendingArmy[attackPosition];
                defender = SoldierAttack(soldier, defender, attackingArmyName);
                soldier.CurrentActions -= 1;
            }
        }

        private static List<Soldier> RemoveDeadSoldiers(List<Soldier> army, string armyName)
        {
            var deathCount = 0;
            while(true)
            {
                var soldierLocation = army.Select((army, index) => new {index, army.Wounds}).Where(a=> a.Wounds.Contains("Death")).FirstOrDefault()?.index;
                if(soldierLocation == null)
                {
                    break;
                }
                else
                {
                    army.RemoveAt(soldierLocation.Value);
                    deathCount++;
                }
            }

            Console.WriteLine($"{armyName} lost {deathCount} soldiers");
            return army;
        }

        private static void CreateArmy(List<Soldier> army)
        {
            var random = new Random();
            for (int i = 0; i < 50; i++)
            {
                var actions = random.Next(1, 3);
                army.Add(new Soldier
                {
                    Attack = random.Next(7, 20),
                    Defense = random.Next(1, 5),
                    //Health = random.Next(7, 30),
                    MaxActions = actions,
                    CurrentActions = actions,
                    Initiative = random.Next(50, 100),
                    Skill = random.Next(5, 30),
                    Wounds = new List<string>()
                });
            }
        }

        private static Soldier SoldierAttack(Soldier attacker, Soldier defender, string attackingArmyName)
        {
            var random = new Random();
            var baseHitChance = random.Next(5, 10) + attacker.Skill - defender.Skill;
            if(baseHitChance <= 0)
            {
                baseHitChance = 10;
            }

            if(baseHitChance >= 100)
            {
                baseHitChance = 95;
            }

            var attackRoll = random.Next(1, 100);

            if(attackRoll <= baseHitChance)
            {
                //defender.Health -= (attacker.Attack - defender.Defense);
                var woundCount = defender.Wounds.Count();
                var deathChance = 50 + (woundCount * 10) + attackRoll;
                if(deathChance >= 100)
                {
                    deathChance = 95;
                }

                var deathRoll = random.Next(100);
                if(deathRoll <= deathChance)
                {
                    defender.Wounds.Add("Death");
                }
                Console.WriteLine($"{attackingArmyName} Attacks for {attacker.Attack}");
            }
            else{
                Console.WriteLine($"{attackingArmyName} misses an attack");
            }


            return defender;
        }

        private static List<Soldier> SortArmy(List<Soldier> army)
        {
            return army.OrderByDescending(army => army.Initiative).ToList();
        }

        private static bool IsArmyActive(List<Soldier> army)
        {
            return army.Where(a => a.CurrentActions > 0).Count() > 0;
        }
    }
}
