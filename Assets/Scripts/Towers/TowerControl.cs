using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    [SerializeField]
    float timeBetweenAttacks; // Задержка между выстрелами
    [SerializeField]
    float attackRadius; //Радиус выстрела
    [SerializeField]
    Projectile projectile; // Ссылаемся на скрипт Projectile и от туда забирать projectile
    Enemy targetEnemy = null; // По какому противнику будет стрелять. С самого начала null , потому что с самого начала башня не может стрелять, без проверки. 
                              // Enemy, потому что ссылаемся на скрипт Enemy

    private float attackCounter; //Счетчик для стрельбы
    private bool isAttacking = false; // С самого начала убриаем возможность стрелять.



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        attackCounter -= Time.deltaTime; // Счетчик будет снижаться при помощи времени, которое будет проходить

        if (targetEnemy == null || targetEnemy.IsDead) // Если противника нет в радиусе башни
        {
            Enemy nearestEnemy = GetNearestEnemy(); // Используем переменную чтобы не ссылаться всегда на скрипт GetNearestEnemy(), а будем использовать новую переменную. Ссылавется единожды, реализуется всегда множество раз
            if (nearestEnemy != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius) // если есть какой-то противнки по которому можно стрелять.
            {
                targetEnemy = nearestEnemy; // Считываем информацию с таргета и если по нему можно стрелять таргет становится ближайшим противником
            }
        }
        else
        {
            if (attackCounter <= 0)
            {
                isAttacking = true; 

                attackCounter = timeBetweenAttacks; // Сброс счетчика
            }
            else
            {
                isAttacking = false; // В любом дргом случае башнея не может стрелять
            }

            if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius) // Если позиция таргета больше радиуса башни
            {
                targetEnemy = null; // Перестаем видить таргет , по кторому можем стрелять и начинаем проверять заного
            }
        }
        if (isAttacking == true) // Если мы можем стрелять
        {
            Attack(); // Вызов метода атаки 
        }
    }
    

    public void Attack()
    {
        isAttacking = false; // Башня изночально не стреляет
        Projectile newProjectile = Instantiate(projectile, transform.localPosition, Quaternion.identity) as Projectile; // переменная будет использовать projectile как Projectile
        newProjectile.transform.localPosition = transform.localPosition; // Появление нового newProjectile
        if (newProjectile.PType == projectileType.arrow)
        {
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Arrow);
        }
        else if (newProjectile.PType == projectileType.fireball)
        {
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.FireBall);
        }
        else if (newProjectile.PType == projectileType.rock)
        {
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Rock);
        }


        if (targetEnemy == null) // Если целей нет
        {
            Destroy(newProjectile); //Удаляем наши projectile
        }
        else
        {
            //move projectile to enemy
            StartCoroutine(Moveprojectile(newProjectile)); // запускаем метод для движения projectile
        }
    }


    IEnumerator Moveprojectile(Projectile projectile)
    {
        while(GetTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null) // Пока расстояние до нашего противника больше чем 0.20f
                                                                                                   // И projectile существует (Можем стрелять)
                                                                                                   // И targetEnemy существует (По кому стрелять)
        {
            var dir = targetEnemy.transform.localPosition - transform.localPosition; // Дистанция уменьшается
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // Угол поворота , нарпавление по х и у умноженное на радиус угла поворота
            projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward); // Поворот снаряда после счиитывания информации поворота. Следовать к протвниику поворачивая угол снаряда.
            // Позиция projectile будет двишаться к протвинику от позиции projectile до позиции противника со скоростью  5f * Time.deltaTime
            projectile.transform.localPosition = Vector2.MoveTowards(projectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
            yield return null;
            
        }

        if (projectile != null || targetEnemy == null)
        {
            Destroy(projectile);
        }
    }

    private float GetTargetDistance(Enemy thisEnemy)
    {
        if (thisEnemy == null)
        {
            thisEnemy = GetNearestEnemy();
            if (thisEnemy == null)
            {
                return 0f;
            }
        }

        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition)); // Математическое вырожение считывает расстояне джо противника
    }

   private List<Enemy> GetEnemiesInRange() // Используем List для того что бы знать , какой из противников ближе, какой противник в зоне порожения.
    {
        List<Enemy> enemiesInRange = new List<Enemy>(); 

        foreach(Enemy enemy in Manager.Instance.EnemyList) // Проверка всех противников,попали они в зону поражения или ушли из нее. Проверяет из скрипта Manager наш Enemy<List> и искать там противника.
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius) // Из положения объекта проверяет какое расстояние до противников(enemy.transform.position) и если оно  <= радиуса башни(attackRadius)
            {
                enemiesInRange.Add(enemy); // То мы добавляем объект в который можно стрелять. объект enemy.
            }
        }

        return enemiesInRange; // Возвращение противников , которые дошли до зоны поражения.
    }


     private Enemy GetNearestEnemy() // Для того чтобы считывать какой противник к нам будет находится ближе для начала стрельбы.
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity; //PositiveInfinity - всегда положительная бесконечность. Грубо говоря. Стрельба по противнику, у которого меньше всего расстояния 

        foreach (Enemy enemy in GetEnemiesInRange()) // Считывает всех противников в диапазоне стрельбы.
        {
            if (Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <  smallestDistance) // Если расстояние от башни до противника меньше , всех остальных противников.
            {   
                smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition); // smallestDistance превращается в расстояние от башни до противника.
                nearestEnemy = enemy; //То этот ближайший противник должен стать главным таргетом
            }
        }
        return nearestEnemy; // Проверка каждого противника, который попал нам в диапазон.
     }
}
