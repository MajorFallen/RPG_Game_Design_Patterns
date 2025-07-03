using ProOb_RPG.GameModel;
using ProOb_RPG.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProOb_RPG.Entities;

namespace ProOb_RPG.Entities
{
    internal partial class Player : Entity
    {
        protected override string EntityName => "Player";

        public event ModelGameSystem.PlayerPort.OutputPort.MessageEventHandler? onMessageFromPlayerThrown;
        public event ModelGameSystem.PlayerPort.OutputPort.PlayerStatsEventHandler? OnPlayerStatsChanged;
        public event ModelGameSystem.PlayerPort.OutputPort.PlayerEffectsEventHandler? OnPlayerEffectsChanged;
        public event ModelGameSystem.PlayerPort.OutputPort.PlayerCoinsEventHandler? OnPlayerCoinsChanged;
        public event ModelGameSystem.PlayerPort.OutputPort.PlayerHandEventHandler? OnPlayerHandChanged;
        public event ModelGameSystem.PlayerPort.OutputPort.PlayerInventoryEventHandler? OnPlayerInventoryChanged;

        public override bool PickUpItem(Dungeon dungeon, int item_number = 0)
        {
            IItem? item = dungeon.GetTile(Position)!.GetTileItem(item_number);
            if (base.PickUpItem(dungeon, item_number))
            {
                onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} picked up {item!.GetItemName()}."));
                return true;
            }
            else if (item != null)
            {
                onMessageFromPlayerThrown?.Invoke(new StringBuilder($"Unable to pick up {item!.GetItemName()}."));
            }
            return false;
        }
        public override bool DropItemFromInventory(Dungeon dungeon, int whichItemInInventory)
        {
            if (whichItemInInventory >= inventory.Count)
                return false;
            IItem item = inventory[whichItemInInventory];
            if (!item.DropItem(dungeon, this, item))
                return false;
            onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} dropped {item.GetItemName()}."));
            return true;
        }
        public override bool EquipItemInHand(Hands hand, int whichItemInInventory)
        {
            if (!base.EquipItemInHand(hand, whichItemInInventory))
                return false;
            onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} equipped {(hand == Hands.Left ? leftHand!.GetItemName() : rightHand!.GetItemName())} to {(hand == Hands.Left ? "left" : "right")} hand"));
            return true;
        }
        public override bool UnEquipItemInHand(Hands hand)
        {
            IItem? item;
            if (hand == Hands.Left && leftHand != null)
            {
                item = leftHand;
                if (leftHand.UnEquipItemInHand(this, hand))
                {
                    onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} unequipped {item.GetItemName()}."));
                    return true;
                }
                return false;
            }
            if (hand == Hands.Right && rightHand != null)
            {
                item = rightHand;
                if (rightHand.UnEquipItemInHand(this, hand))
                {
                    onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} unequipped {item.GetItemName()}."));
                    return true;
                }
                return false;
            }
            return true;
        }
        public override void AddToInventory(IItem item)
        {
            base.AddToInventory(item);
            OnPlayerInventoryChanged?.Invoke(inventory);
        }
        public override IItem? RemoveItemFromInventory(IItem item)
        {
            IItem? removed = base.RemoveItemFromInventory(item);
            OnPlayerInventoryChanged?.Invoke(inventory);
            return removed;
        }
        public override IItem? AddItemToLeftHand(IItem item)
        {
            IItem? result = base.AddItemToLeftHand(item);
            OnPlayerHandChanged?.Invoke(leftHand, rightHand);
            return result;
        }

        public override IItem? AddItemToRightHand(IItem item)
        {
            IItem? result = base.AddItemToRightHand(item);
            OnPlayerHandChanged?.Invoke(leftHand, rightHand);
            return result;
        }

        public override IItem? RemoveItemFromLeftHand()
        {
            IItem? item = base.RemoveItemFromLeftHand();
            OnPlayerHandChanged?.Invoke(leftHand, rightHand);
            return item;
        }

        public override IItem? RemoveItemFromRightHand()
        {
            IItem? item = base.RemoveItemFromRightHand();
            OnPlayerHandChanged?.Invoke(leftHand, rightHand);
            return item;
        }
        public override bool UseItem(Hands hand)
        {
            IItem? item;
            if (hand == Hands.Left)
                item = leftHand;
            else
                item = rightHand;
            if (item == null)
            {
                onMessageFromPlayerThrown?.Invoke(new StringBuilder($"No item in this hand!"));
                return false;
            }
            if (!base.UseItem(hand))
            {
                onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{item.GetItemName()} is not usable item!"));
                return false;
            }
            onMessageFromPlayerThrown?.Invoke(new StringBuilder($"{EntityName} used {item.GetItemName()}"));
            return true;
        }
        public Player(EntityStats stats, EntityCoins coins)
        : base(stats, coins)
        {
            Coins.OnCoinsChanged += () => OnPlayerCoinsChanged?.Invoke(Coins);
            GetModifiedStatsToChange().OnStatsChanged += () => OnPlayerStatsChanged?.Invoke(BaseStats, ModifiedStats);
        }

        public void UpdatePlayerEvents()
        {
            OnPlayerCoinsChanged?.Invoke(Coins);
            OnPlayerStatsChanged?.Invoke(BaseStats, ModifiedStats);
            OnPlayerEffectsChanged?.Invoke(_effectsMenager.GetEffects());
            OnPlayerHandChanged?.Invoke(leftHand, rightHand);
            OnPlayerInventoryChanged?.Invoke(inventory);
        }
    }
}
