using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems; // Библиотека ивентов , которая позволит реализовать код ниже.
using UnityEngine;


public class TowerManager : Loader<TowerManager>
{
    public TowerBtn towerBtnPressed { get; set; }

    public List<TowerControl> TowerList = new List<TowerControl>();
    private List<Collider2D> BuildList = new List<Collider2D>(); // Лист с местоположением башен
    private Collider2D buildTile;

    SpriteRenderer spriteRenderer; // Переменная отвечающая за отображение спрайта (Башни) около курсора. Не меняет функционал.

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Реализация SpriteRenderer
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // Условие, если мы будем нажимать 0 - Левая кнопка мыши. Анализировать будем только при нажатии мышки.
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Локальная переменная которая считывает где мы кликаем мышкой
                                                                                      // Camera.main.ScreenToWorldPoint - весь экран. Input.mousePosition - расположение нашей мыши.

            RaycastHit2D hit = Physics2D.Raycast(mousePoint, Vector2.zero); //Для работы Raycast необходимо, чтоб объект имел компонент Collider
                                                                            //В rayHit мы занесем результат выполнения команды Raycast
                                                                            //там будет либо null, если raycast никого не задел, либо первый
                                                                            //объект, стоящий на пути мышки (какой объект будет первым
                                                                            //решает его положение Z в мировом пространстве)

           if(hit.collider.tag == "TowerSide") // Проверяем, если мы кликаем на текстуру с этим тэгом.
           {
                buildTile = hit.collider; 
                buildTile.tag = "TowerSideFull"; // Строчка, отвечающая за то, что больше не будут ставиться башни в одно и тоже место. 
                RegisterBuildSite(buildTile);


                PlaceTower(hit); //Если If будет выполнять, то будет отправляться луч и будет происходить дествие PlaceTower;
           }
        }
        if (spriteRenderer.enabled) //В тому случае, если у нас spriteRenderer активен. Выноси за If нажатия левой кнопки , потому что нужен результат , когда картинка всегда следует за курсором. 
        {
            FollowMouse();
        }
    }

    public void RegisterBuildSite(Collider2D buildTag) // Добавляем в список какие-то башни
    {
        BuildList.Add(buildTag);
    }

    public void RegisterTower(TowerControl tower) 
    {
        TowerList.Add(tower);
    }

    public void RenameTagBuildSite() // Смена тега иест для башни , после удаления башни 
    {
        foreach (Collider2D buildTag in BuildList)
        {
            buildTag.tag = "TowerSide";
        }
        BuildList.Clear();
    }

    public void DeatroyAllTowers() // Удаление всех башен при начале новой игры 
    {
        foreach(TowerControl tower in TowerList)
        {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }


    public void PlaceTower(RaycastHit2D hit) // Метод, который будет указывать на расположение башни. RaycastHit2D hit будет отправляться луч для проверки.
    {
        if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null) // Условие евента,  IsPointerOverGameObject обозначает что мы не сможем поставить башню, если навели на кнопку.
                                                                                       // если мы не навели на одну из кнопок , а так же мы нажали на какую-то кнопку.     
        {
            TowerControl newTower = Instantiate(towerBtnPressed.TowerObject); // Объект новой башни будет спавниться , которая привязанна к определенной кнопке.
            newTower.transform.position = hit.transform.position; // Расположение новой башни будет такое же , куда мы кликнули
            BuyTower(towerBtnPressed.TowerPrice);
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            DisableDrag();
        }
        
    }

    public void BuyTower(int price)
    {
        Manager.Instance.subtractMoney(price);
    }

    public void SelectedTower(TowerBtn towerSelected) // Метод для отслеживания , какая башня была выбрана.
    {
        if (towerSelected.TowerPrice <= Manager.Instance.TotalMoney)
        {
            towerBtnPressed = towerSelected; // Привязываем башни к кнопке, какая кнопка была нажата такая башня и будет.
            EnableDrag(towerBtnPressed.DragSprite); // Метода , который будет считывать с towerBtnPressed и DragSprite, два действия в одно нажатие кнопки.
        }     
    }

    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Указываем что позиция этого метода будует ровна позиции мышки на экроане.
        transform.position = new Vector2(transform.position.x, transform.position.y); // Помимо того что сы нашли где наш курсор , мы должны написать что картинка должна следовать за курсором. 
    }

    public void EnableDrag(Sprite sprite) // Метод для того чтобы мы могли передвигать наш элемент, в скобках указываем что мы будем управлять картинкой. включает отображение
    {
        spriteRenderer.enabled = true; // Включает отображение картинки 
        spriteRenderer.sprite = sprite; // Указываем что это будет картинка 
    }

    public void DisableDrag() // выключает отображение
    {
        spriteRenderer.enabled = false; // Включает отображение картинки 
        towerBtnPressed = null; //Строчка для того чтобы скидывалась башня после того как поставили ее на место.
    }
}
