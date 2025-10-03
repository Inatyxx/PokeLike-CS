using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Projet1BaseDuCsharpGrp5
{
    public class Item
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set;  }
        public int Value { get; set; }
        public int Quantity { get; set; }

        public Item() { }

        public void OnInitItem()
        {
            
        }
        // Ancien constructeur :
        //public Item(string name, ItemType itemType, int value = 0, int quantity = 1)
        //{
        //    Name = name;
        //    ItemType = itemType;
        //    Value = value;
        //    Quantity = quantity;
        //}

        public bool DoEffectOfItemOn(Pokemon cible)
        {
            if (cible == null) throw new ArgumentNullException(nameof(cible));

            switch (ItemType)
            {
                case ItemType.PotionHeal:
                    cible.RestoreLife(Value);
                    return true;

                case ItemType.PotionMana:
                    cible.RestoreMana(Value);
                    return true;

                case ItemType.PotionRappel:
                    if (cible.Life <= 0)
                    {
                        cible.RestoreMidLife();
                        return true;
                    }
                    return false;

                case ItemType.PotionRappelMax:
                    if (cible.Life <= 0)
                    {
                        cible.RestoreAllLife();
                        return true;
                    }
                    return false;

                case ItemType.BoostDamage:
                    cible.BoostDamageAttack(Value);
                    return true;

                case ItemType.BoostSpeed:
                    cible.BoostSpeedAtack(Value);
                    return true;

                case ItemType.BoostDefense:
                    cible.BoostDefense(Value);
                    return true;

                case ItemType.Pokeball:
                    return true;

                default:
                    return false;
            }
        }
    }
}
