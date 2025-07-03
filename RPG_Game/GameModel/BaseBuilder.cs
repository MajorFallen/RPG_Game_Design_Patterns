using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProOb_RPG.Entities.Entity;

namespace ProOb_RPG.GameModel
{
    internal interface IDungeonBuilder
    {
        void Reset();
        IDungeonBuilder BuildEmptyDungeon();
        IDungeonBuilder BuildFilledDungeon();
        IDungeonBuilder AddPaths();
        IDungeonBuilder AddRooms(int numberOfRooms);
        IDungeonBuilder AddCentralRoom(int yLength, int xLength);
        IDungeonBuilder AddPlayer(EntityStats entityStats);
        IDungeonBuilder AddItems(int numberOfItems);
        IDungeonBuilder AddPotions(int numberOfPotions);
        IDungeonBuilder AddCoins(int numberOfCoinsStacks);
        IDungeonBuilder AddWeapons(int numberOfWeapons);
        IDungeonBuilder AddModifiedWeapons(int numberOfModifiedWeapons, int maxStacks);
        IDungeonBuilder AddEnemies(int numberOfEnemies);
    }

    internal abstract class AbstractBuilder<T> : IDungeonBuilder
    {
        protected T? _result;

        public AbstractBuilder()
        {
            Reset();
        }
        public abstract void Reset();
        public virtual T GetResult()
        {
            T result = _result!;
            Reset();
            return result;
        }

        public virtual IDungeonBuilder AddCentralRoom(int yLength, int xLength)
        {
            return this;
        }

        public virtual IDungeonBuilder AddCoins(int numberOfCoinsStacks)
        {
            return this;
        }

        public virtual IDungeonBuilder AddEnemies(int numberOfEnemies)
        {
            return this;
        }

        public virtual IDungeonBuilder AddItems(int numberOfItems)
        {
            return this;
        }

        public virtual IDungeonBuilder AddModifiedWeapons(int numberOfModifiedWeapons, int maxStacks)
        {
            return this;
        }

        public virtual IDungeonBuilder AddPaths()
        {
            return this;
        }

        public virtual IDungeonBuilder AddPlayer(EntityStats entityStats)
        {
            return this;
        }

        public virtual IDungeonBuilder AddPotions(int numberOfPotions)
        {
            return this;
        }

        public virtual IDungeonBuilder AddRooms(int numberOfRooms)
        {
            return this;
        }

        public virtual IDungeonBuilder AddWeapons(int numberOfWeapons)
        {
            return this;
        }

        public virtual IDungeonBuilder BuildEmptyDungeon()
        {
            return this;
        }

        public virtual IDungeonBuilder BuildFilledDungeon()
        {
            return this;
        }
    }

    internal class DungeonDirector
    {
        public DungeonDirector()
        {

        }
        public void ConstructSimpleDungeon(IDungeonBuilder builder)
        {
            builder.BuildEmptyDungeon()
                .AddPlayer(new EntityStats(100, 14, 12, 10, 10, 12, 2))
                .AddItems(20)
                .AddWeapons(15)
                .AddModifiedWeapons(10, 4)
                .AddPotions(5)
                .AddEnemies(5);
        }
        public void ConstructComplexDungeon(IDungeonBuilder builder)
        {
            builder.BuildFilledDungeon()
                .AddPaths()
                .AddCentralRoom(7, 11)
                .AddRooms(10)
                .AddPlayer(new EntityStats(500, 14, 12, 10, 10, 12, 2))
                .AddItems(60)
                .AddPotions(200)
                .AddCoins(30)
                .AddWeapons(60)
                .AddModifiedWeapons(30, 5)
                .AddEnemies(50);
        }
    }
}
