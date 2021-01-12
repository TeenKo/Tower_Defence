using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    public Transform exit; // Точка выхода (Finish). Нужна для того чтобы проверять сколько противников прошли путь и не умерли. 
                           // Так же взаимодействие будет отдельное. Отличается от других точке(Spawn и target
    [SerializeField] 
    Transform[] movingPoint; // Создаем массив для того чтобы добавить все точки соприкосновения противником. Что бы мы могли считывать где противник.
    [SerializeField] 
    float navigation; // При помощи этого мы сможем просчитывать перемещение персонажа(enemy), сколько кадров должно использоваться или как часто персонаж должен обновляться.
    [SerializeField]
    int health; // Жизни противника
    [SerializeField]
    int rewardAmount;

    private bool isDead = false;

    

    private Transform enemy; // Переменная для считывания самого противника, его положение.
    private float navigationTime = 0; // С помощью этого мы сможем обновлять наших персонажей и обновлять положение в пространтсве.
    private int target = 0; // Эта переменная отвечает за то , какой цели(SpawnPoint), подошел противник. 
    Collider2D enemyCollider;
    Animator anim;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Transform>(); // Строчка для того чтобы мы могли реализовать и считывать положение персвонажа.
        enemyCollider = GetComponent<Collider2D>();
        Manager.Instance.RegisterEnemy(this);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()   
    {
        if(movingPoint != null && isDead == false)
        {
            navigationTime += Time.deltaTime; //Если у нас еще есть точки, к которым нужно идти. То мы продолжаем идти. Передвижение не равно 0.
            if (navigationTime > navigation)
            {
                if (target < movingPoint.Length) // Если наша цель меньше чем количество всех точек передвижения.
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, movingPoint[target].position, navigationTime); // То положение нашего противника это движение 
                                                                                                                        // "Позиция нашего enemy"(положение нашего противника) 
                                                                                                                        // К следующей точке target "target = счетчик"
                                                                                                                        // И просчитываем где наш противник через navigationTime.
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exit.position, navigationTime); // В любых других случаях мы идем к точке выхода.
                }
                navigationTime = 0;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MovingPoint") // Если противники соприкоснулся с Box Colloder2d tag MovingPoint. 
        {
            target += 1; //То счетчик таргета увеличивается на 1;
        }
        else if (collision.tag == "Finish") //Если противники соприкоснулся с Box Colloder2d tag Finis. 
        {
            Manager.Instance.RoundEscaped += 1; 
            Manager.Instance.TotalEscaped += 1;
            Manager.Instance.UnregisterEnemy(this); // Ссылаемся на Manager script и вызываем метод уменьшения счетчика противников.
            Manager.Instance.IsWaveOver(); 
        }
        else if (collision.tag == "Projectile")
        {
            Projectile newP = collision.gameObject.GetComponent<Projectile>();
            EnemyHit(newP.AttackDamage);
            Destroy(collision.gameObject);
        }
    }

    public void EnemyHit (int hitPoints)
    {
        if (health - hitPoints > 0) // Когда жизни остаются еще
        {
            health -= hitPoints; // Наши очки здоровья будут уменьшаться относительно кол-во урона , который получил противник.

            //hurt
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Hit);
            anim.Play("Hurt");
        }
        else
        {
            //die
            anim.SetTrigger("didDie");
            Die();
        }
    }
    
    public void Die ()
    {
        isDead = true;
        enemyCollider.enabled = false;
        Manager.Instance.TotalKilled += 1;
        Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Death);
        Manager.Instance.addMoney(rewardAmount);
        Manager.Instance.IsWaveOver();
    }
}
