using UnityEngine;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    TowerControl towerObject; // Переменная для башни
    [SerializeField]
    Sprite dragSprite; // Переменная , которая будет считывать изображение.
    [SerializeField]
    int towerPrice; // Цена за башню



    public TowerControl TowerObject
    { 
        get
        {

            return towerObject;
        }
    }

    public Sprite DragSprite // Метод  , который возвращает выбранную картинку
    {
        get
        {

            return dragSprite;
        }
    }

    public int TowerPrice
    {
        get
        {
            return towerPrice;
        }
    }

}
