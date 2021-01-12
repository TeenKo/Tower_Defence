using UnityEngine;

public enum projectileType { // Нумиратор
    rock, arrow, fireball
};

public class Projectile : MonoBehaviour
{
    [SerializeField]
    int attackDamage; // Переменная отвечающая за урон.

    [SerializeField]
    projectileType pType;

    public int AttackDamage // Метод получает информацию Урон
    {
        get
        {

            return attackDamage; // возврат урона 
        }
    }

    public projectileType PType // Метод ссылвается на наши снаряды.
    {
        get
        {

            return pType;
        }
    }
}
