using ProOb_RPG.Entities;
using ProOb_RPG.GameInput;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ProOb_RPG.Entities.Entity;
using static ProOb_RPG.GameModel.DungeonBuilder;

namespace ProOb_RPG.GameModel
{
    internal class DungeonBuilder : AbstractBuilder<Dungeon>
    {

        public class DungeonNotInitializedException : Exception
        {
            public DungeonNotInitializedException() : base("Not able to work on not initialized dungeon") { }
        }

        public static List<Func<Entity>> enemyFactories = new List<Func<Entity>>
        {
            () => new EnemyOrc()
        };
        public static List<Func<Item>> itemFactories = new List<Func<Item>>
        {
            () => new ItemRingOfPower(),
            () => new ItemKremowka(),
            () => new ItemAmogus()
        };
        public static List<Func<Item>> potionFactories = new List<Func<Item>>
        {
            () => new PotionOfStrength(),
            () => new PotionOfDragonBreath(),
            () => new PotionŻytnia(),
            () => new PotionMilk(),
        };
        public static List<Func<Item>> weaponFactories = new List<Func<Item>>
        {
            () => new WeaponGreatSword(),
            () => new WeaponLongSword(),
            () => new WeaponFireWand()
        };
        public static List<Func<IItem, int, IItem>> weaponEffectFactories = new List<Func<IItem, int, IItem>>
        {
            (item, level) => new WeaponEffectGains(item, level),
            (item, level) => new WeaponEffectSharpness(item, level),
            (item, level) => new WeaponEffectSussy(item, level)
        };
        public static List<Func<double, IItem>> coinStacksFactories = new List<Func<double, IItem>>
        {
            (amount) => new CoinsClass(CoinsClass.CoinTypes.Gold, (int)(amount*1)),
            (amount) => new CoinsClass(CoinsClass.CoinTypes.Silver, (int)(amount * 5)),
            (amount) => new CoinsClass(CoinsClass.CoinTypes.Bronze, (int)(amount * 20)),
        };
        private enum DungeonStatus { Initialized, Uninitialized }
        private DungeonStatus Status;

        private readonly int _height;
        private readonly int _width;

        private Dungeon _dungeon;
        private Random _random = new Random();

        public DungeonBuilder(int height, int width)
        {
            Status = DungeonStatus.Uninitialized;
            _height = height;
            _width = width;
            _dungeon = new Dungeon(_height, _width);
            Reset();
        }
        private bool IsInitialized()
        {
            return Status == DungeonStatus.Initialized ? true : false;
        }
        public override void Reset()
        {
            _dungeon = new Dungeon(_height, _width);
        }
        public override IDungeonBuilder BuildEmptyDungeon()
        {
            for (int y = 0; y <= _height + 1; y++)
            {
                for (int x = 0; x <= _width + 1; x++)
                {
                    if (y == 0 || y == _height + 1 || x == 0 || x == _width + 1)
                    {
                        _dungeon.SetTile(y, x, new Tile(Tile.TileTypes.Wall));
                    }
                    else
                    {
                        _dungeon.SetTile(y, x, new Tile(Tile.TileTypes.Floor));
                    }
                }
            }
            Status = DungeonStatus.Initialized;
            return this;
        }
        public override IDungeonBuilder BuildFilledDungeon()
        {
            for (int y = 0; y <= _height + 1; y++)
                for (int x = 0; x <= _width + 1; x++)
                    _dungeon.SetTile(y, x, new Tile(Tile.TileTypes.Wall));
            Status = DungeonStatus.Initialized;
            return this;
        }
        public override IDungeonBuilder AddPaths()
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            bool[,] maze = new bool[(_height + 2), (_width + 2)];
            GenerateMaze(maze, 1, 1);

            for (int i = 0;  i <= _height + 1; i++)
            {
                for(int j = 0;  j <= _width + 1; j++)
                {
                    if (maze[i,j])
                    {
                        _dungeon.GetTile((i, j))!.TileType = Tile.TileTypes.Floor; 
                    }
                }
            }
            return this;
        }
        private void GenerateMaze(bool[,] maze, int x, int y)
        {
            // Tworzymy ścieżkę (true to podłoga)
            maze[x, y] = true;
            
            // Losujemy kierunki
            int[] directions = new int[] { 1, 2, 3, 4 }; // 1 - góra, 2 - dół, 3 - lewo, 4 - prawo
            Shuffle(directions); // Tasowanie kierunków, aby były losowe

            foreach (int dir in directions)
            {
                int newX = x, newY = y;

                switch (dir)
                {
                    case 1: // Góra
                        newX -= 2;
                        break;
                    case 2: // Dół
                        newX += 2;
                        break;
                    case 3: // Lewo
                        newY -= 2;
                        break;
                    case 4: // Prawo
                        newY += 2;
                        break;
                }

                // Sprawdzamy, czy nowe pole jest w granicach tablicy i jeszcze nie było odwiedzone
                if (newX > 0 && newX < _height + 1 && newY > 0 && newY < _width + 1 && !maze[newX, newY])
                {
                    // Przerabiamy sąsiednią komórkę na drogę (true)
                    maze[newX, newY] = true;
                    // Tworzymy przejście między bieżącą komórką a sąsiednią (przedzielamy ścianą)
                    maze[(x + newX) / 2, (y + newY) / 2] = true;
                    // Rekurencyjnie generujemy labirynt od tej komórki
                    GenerateMaze(maze, newX, newY);
                }
            }
        }
        private void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i >= 0; i--)
            {
                int j =  _random.Next(0, array.Length - 1);
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
        public override IDungeonBuilder AddRooms(int numberOfRooms)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfRooms; i++)
            {
                int xLength = _random.Next(1, 4)*2 + 1;
                int yLength = _random.Next(1, 2)*2 + 1;
                int x = _random.Next(1, (_width - xLength)/2)*2 + 1;
                int y = _random.Next(1, (_height - yLength)/2)*2 + 1;
                (int, int)[] corners = [(y-1,x-1), (y-1,x + xLength), (y + yLength,x-1), (y + yLength,x + xLength)];
                bool isFit = true;
                for(int j = 0; j < 4; j++)
                {
                    if (_dungeon.GetTile(corners[j])!.TileType != Tile.TileTypes.Wall)
                    {
                        isFit = false;
                        i--;
                        break;
                    }
                }
                if (!isFit) continue;
                for (int j = y; j < y + yLength; j++)
                {
                    for (int k = x; k < x + xLength; k++)
                    {
                        Tile? tile = _dungeon.GetTile((j, k));
                        if (tile == null)
                            continue;
                        tile.TileType = Tile.TileTypes.Floor;
                    }
                }
            }
            return this;
        }
        public override IDungeonBuilder AddCentralRoom(int yLength, int xLength)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            int yStart = (_height - yLength) / 2 + 1;
            int xStart = (_width - xLength) / 2 + 1;
            for (int y = yStart; y < yStart + yLength; y++)
            {
                for (int x = xStart; x < xStart + xLength; x++)
                {
                    Tile? tile = _dungeon.GetTile((y, x));
                    if (tile == null)
                        continue;
                    tile.TileType = Tile.TileTypes.Floor;
                }
            }
            return this;
        }
        public override IDungeonBuilder AddPlayer(EntityStats entityStats)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            Player player = new Player(entityStats, new Entity.EntityCoins());
            ModelGameSystem.GetInstance().playerPort.SwitchPlayer(player);
            for (int y = 1; y <= _height; y++)
            {
                for (int x = 1; x <= _width; x++)
                {
                    if (_dungeon.AddPlayer(player, (y, x)))
                        return this;
                }
            }
            throw new Exception("Player was not Initialized");
        }
        public override IDungeonBuilder AddItems(int numberOfItems)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfItems; i++)
            {
                if (!_dungeon.AddItem(itemFactories[_random.Next(itemFactories.Count)](), GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override IDungeonBuilder AddPotions(int numberOfPotions)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfPotions; i++)
            {
                if (!_dungeon.AddItem(potionFactories[_random.Next(potionFactories.Count)](), GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override IDungeonBuilder AddCoins(int numberOfCoinsStacks)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfCoinsStacks; i++)
            {
                if (!_dungeon.AddItem(coinStacksFactories[_random.Next(coinStacksFactories.Count)](1 + _random.NextDouble()*5), GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override IDungeonBuilder AddWeapons(int numberOfWeapons)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfWeapons; i++)
            {
                if (!_dungeon.AddItem(weaponFactories[_random.Next(weaponFactories.Count)](), GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override IDungeonBuilder AddModifiedWeapons(int numberOfModifiedWeapons, int maxStacks)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfModifiedWeapons; i++)
            {
                IItem item = weaponFactories[_random.Next(weaponFactories.Count)]();
                for (int j = 0; j < _random.Next(maxStacks); j++)
                {
                    item = weaponEffectFactories[_random.Next(weaponEffectFactories.Count)](item, _random.Next(0, WeaponDecorator._maxEfffectLevel));
                }
                if (!_dungeon.AddItem(item, GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override IDungeonBuilder AddEnemies(int numberOfEnemies)
        {
            if (!IsInitialized()) throw new DungeonNotInitializedException();
            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (!_dungeon.AddEntity(enemyFactories[_random.Next(enemyFactories.Count)](), GetRandomPosition()))
                    i--;
            }
            return this;
        }
        public override Dungeon GetResult()
        {
            Dungeon result = _dungeon;
            ModelGameSystem.GetInstance().playerPort.SwitchDunegon(_dungeon);
            Reset();
            return result;
        }
        private (int, int) GetRandomPosition()
        {
            return (_random.Next(1, _height), _random.Next(1, _width));
        }

        
   }

    
}
