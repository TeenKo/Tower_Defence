using UnityEngine;

public class Loader <T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>(); //В том случае, если instance не существует, мы создаем instace;
            }
            else if (instance != FindObjectOfType<T>())
            {
                Destroy(FindObjectOfType<T>()); //Эта строчка удаляет созданный manager<T>, в тех случаях, когда этот manager не нужен.
            }

            DontDestroyOnLoad(FindObjectOfType<T>()); //Эта строчка не удаляет manager<T>. (При загрузке, когда создастся manager<T>, он не удалиться)

            return instance;
        }
    }
}
