using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum gameStatus // определяет какой статус у игры.
{
    next, play, gameover, win
}
public class Manager : Loader<Manager>
{
    [SerializeField]
    Button playBtn; // Кнопка "Играть"
    [SerializeField]
    Text playBtnLable; // Текст для кнопки "Играть"
    [SerializeField]
    Text totalEscapedLabel; // Текст для блока с отображением количества монстров на уровне.
    [SerializeField]
    Text currentWave; // Конкретная волная
    [SerializeField]
    Text totalMoneyLable; // Количество денег
    [SerializeField]
    int totalWaves = 10;


    [SerializeField] //Показывает в инспекторе поля для изменения , но не дает public для других скриптов.
    GameObject spawnPoint; //Сюда помещаем элемент спавна монстров.
    [SerializeField]
    Enemy[] enemies;  //Сюда помещаем всех наших противников.
    [SerializeField]
    int totalEnemies; //Параметр для отслеживания, сколько всего должно появляться противников за уровень.
    [SerializeField]
    int enemiesPerSpawn; //Параметр отвечает за должное количество спавна в один момент(одновременно).

    int waveNumber = 0; // Счетчик волны
    int totalMoney = 10; // Счетчик денег
    int totalEscaped = 0; // Счетчик врагов мы пропустили за игру 
    int roundEscaped = 0; // Счетчик врагов мы пропустили за волну
    int totalKilled = 0; // Счетчк убитых врагов
    int whichEnemiesToSpawn = 0; // Счетчки врагов , которые заспавнятся
    int enemiesToSpawn = 0;
    gameStatus currentState = gameStatus.play; // Изначально наше состояние игры это "Играем"
    AudioSource audioSource;


    public List<Enemy> EnemyList = new List<Enemy>(); //Для того чтобы помещать всех противников , которые остались на карте и он будет выбирать по какому врагу стрелять с этого списка.

    private const float spawnDelay = 0.5f; //Создали константу spawnDela 

    public int TotalEscaped
    {
        get
        {
            return totalEscaped; // Получаем информацию о сбижавших противниках 
        }
        set
        {
            totalEscaped = value; // Присвоение информации о сбижавших противниках 
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped; // Получаем информацию о сбижавших противниках на уровне 
        }
        set
        {
            roundEscaped = value; // Присвоение информации о сбижавших противниках на уровне  
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled; // Получаем информацию о убитых противниках на уровне
        }
        set
        {
            totalKilled = value; // Присвоение информации о убитых противниках на уровне  
        }
    }

    public int TotalMoney // количество денег
    {
        get
        {
            return totalMoney; // возврат значеня 
        }
        set
        {
            totalMoney = value; 
            totalMoneyLable.text = TotalMoney.ToString(); // перевод кол-во денег в текстовый формат.
        }
    }

    public AudioSource AudioSource
    {
        get
        {
            return audioSource;
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        playBtn.gameObject.SetActive (false); // отключаем активность кнопки, так как в начале игры она не должна появляться
        audioSource = GetComponent<AudioSource>();
        ShowMenu();
    }


    private void Update() // Делаем апдейт для того чтобы была проверка нажатия esc 
    {
        HandleEscape();
    }


    private IEnumerator Spawn()
    {
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) // В том случае, когда еще могут создаваться противники.
        {
            for (int i = 0; i < enemiesPerSpawn; i++) // Создается цикл , который создает по одному обьекту(монстру) пока мы не дойдем до значения totalEnemies.
            {
                if (EnemyList.Count < totalEnemies) // Если монстров меньше чем максимальное количество за раунд. То мы создаем нового монстра(enemies) как игровой объект.
                {
                    Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]) as Enemy; // Появление монстра.
                    newEnemy.transform.position = spawnPoint.transform.position; // Появление монстра в точки координат спавна.
                }
            }

            yield return new WaitForSeconds(spawnDelay); // Написали yield return с задержкой, чтобы работала IEnumerator, с spawnDelay для дальнейшего манипулирования. 
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy) // Регистриирует противников, которые в зоне стрельбы. из скрипта Enemy будет брать enem
    {
        EnemyList.Add(enemy); // Добавляет в наш List противников.
    }

    public void UnregisterEnemy(Enemy enemy) // Снимает регистрацию противников, которые в зоне стрельбы. из скрипта Enemy будет брать enem
    {
        EnemyList.Remove(enemy); // Удаляет из нашего List противников.
        Destroy(enemy.gameObject); // Удаляет enemy из спсика.
    }

    public void DestroyEnemies() // Создаем Условие, когда будет уничтожаться противник.
    {
        foreach (Enemy enemy in EnemyList) // Выбираем enemy из нашего EnemyList
        {
            Destroy(enemy.gameObject); // Уничтожает enemy из списка.
        }

        EnemyList.Clear(); // Это будет очищать список с возможность создания нового списка.
    }

    public void addMoney(int amount) // Получаем деньги
    {
        TotalMoney += amount;
    }

    public void subtractMoney(int amount) // Тратим деньги
    {
        TotalMoney -= amount;
    }

    public void IsWaveOver() 
    {
        totalEscapedLabel.text = "Escaped " + TotalEscaped + " / 10";

        if ((RoundEscaped + TotalKilled) == totalEnemies) 
        {
            if(waveNumber <= enemies.Length)
            {
                enemiesToSpawn = waveNumber;
            }
            SetCurrentGameState();
            ShowMenu();
        }
    }

    public void SetCurrentGameState()
    {
        if(totalEscaped >= 10 )
        {
            currentState = gameStatus.gameover;
        }
        else if (waveNumber == 0 && (RoundEscaped + TotalKilled == 0))
        {
            currentState = gameStatus.play;
        }
        else if (waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        }
        else
        {
            currentState = gameStatus.next;
        }
    }

    public void PlayButtonPressed()
    {
        switch(currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;
                break;

            default:
                totalEnemies = 5;
                TotalEscaped = 0;
                TotalMoney = 10;
                enemiesToSpawn = 0;
                TowerManager.Instance.DeatroyAllTowers();
                TowerManager.Instance.RenameTagBuildSite();
                totalMoneyLable.text = TotalMoney.ToString();
                totalEscapedLabel.text = "Escaped " + TotalEscaped + " / 10";
                audioSource.PlayOneShot(SoundManager.Instance.NewGame);
                break;
        }
        DestroyEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWave.text = "Wave " + (waveNumber + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }

    public void ShowMenu() 
    {
        switch(currentState) // Свитч для прверки состояния игры 
        {
            case gameStatus.gameover:
                playBtnLable.text = "Play again!";
                AudioSource.PlayOneShot(SoundManager.Instance.GameOver);
                break;

            case gameStatus.next:
                playBtnLable.text = "Next wave";

                break;

            case gameStatus.play:
                playBtnLable.text = "Play game";

                break;

            case gameStatus.win:
                playBtnLable.text = "Play game";

                break;
        }
        playBtn.gameObject.SetActive(true); // Активирует кнопку при одном из четрыех состояний. 
    }

   private void HandleEscape() // Отмена выбора башни.
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // Если нажать кнопку esc 
        {
            TowerManager.Instance.DisableDrag(); // Отключаем выбор
            TowerManager.Instance.towerBtnPressed = null;
        }
    }
}
