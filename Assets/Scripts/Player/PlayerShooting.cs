using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShooting : MonoBehaviour
{
    public static PlayerShooting Instance;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public float fireballSpeed = 10f;

    private Vector2 lastDirection = Vector2.right;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Time.timeScale <= 0.0001f) return;

        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == "HometownScene" || activeScene == "HouseInteriorScene")
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastDirection = new Vector2(moveX, moveY).normalized;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fireballPrefab != null)
        {
            GameObject fireballObj = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Fireball fireball = fireballObj.GetComponent<Fireball>();
            if (fireball != null)
            {
                int baseDamage = (GameManager.Instance != null) ? GameManager.Instance.CurrentFireballDamage : 50;

                // תוספת נזק Glass Cannon
                if (RiftManager.Instance != null && RiftManager.Instance.HasAffix(RiftAffixType.GlassCannon))
                {
                    baseDamage = Mathf.RoundToInt(baseDamage * 1.5f);
                }

                // בדיקת קריטיקל משדרוגי ה-Altar הקבועים
                float critChance = PermanentUpgrades.GetCritChanceBonus();
                bool isCrit = Random.value < critChance;
                int finalDamage = isCrit ? Mathf.RoundToInt(baseDamage * 2f) : baseDamage;

                fireball.damage = finalDamage;
                fireball.speed = fireballSpeed;
                fireball.Initialize(lastDirection, isCrit);

                if (isCrit)
                {
                    Debug.Log($"<color=#FFD700>[CRIT!] Fireball פגע בעוצמה כפולה: {finalDamage} נזק!</color>");
                }
            }
        }
    }
}