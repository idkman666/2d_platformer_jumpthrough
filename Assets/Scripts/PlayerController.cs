using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D rb;
    public Transform startPoint;
    public bool isStart = false;
    bool isJumpEnabled = true;

    [Header("Temp public")]
    public float torqueForce = 7.5f;

    //posDir is the direction of the player
    // true mean, go to right, false means go left
    public bool posDir = true;
    public float speed = 8f;
    public float jumpSpeed = 5f;

    private SpriteRenderer _renderer;

    //ground check
    bool isFalling = false;
    [Header("RayCast Holder")]
    [SerializeField] GameObject wallCheck1;
    [SerializeField] GameObject wallCheck2;
    [SerializeField] GameObject bottom;

    //jump indicators
    [SerializeField] GameObject[] jumpIndicators;
    int timeJumpPressed = -1;

    //level controller
    private int LevelNumber = 0;
    private int MaxLevelUnlocked;

    //death 
    int TotalDeaths = 0;
    [SerializeField]
    ParticleSystem deathParticleSystem;
    BoxCollider2D boxCollider2D;
    [SerializeField] GameObject jumpIndicatorHodler;
    bool isDead = false;

    //animation
    [Header("Animation")]
    Animator anim;

    //sfx
    [SerializeField]
    AudioClip[] audioClips;
    AudioSource audioSource;

    GameManager gameManager;

    private void Awake()
    {
        startPoint = GameObject.Find("Start_Pos").transform;
        LevelNumber = SceneManager.GetActiveScene().buildIndex;
        TotalDeaths = PlayerPrefs.GetInt("TotalDeaths");
        anim = GetComponent<Animator>();
        MaxLevelUnlocked = PlayerPrefs.GetInt("LevelPlayed");
    }
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        transform.position = startPoint.position;
        _renderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<GameManager>();
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
#if UNITY_EDITOR       

        if (Input.GetKeyDown(KeyCode.D))
        {
            posDir = true;
            isStart = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            posDir = false;
            isStart = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            isStart = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isJumpEnabled && !isDead)
        {
            //touch input
            audioSource.clip = audioClips[1];
            audioSource.Play();
            ++timeJumpPressed;
            HideJumpIndicator(timeJumpPressed);
            anim.SetBool("isJumping", true);
            rb.velocity = Vector2.up * torqueForce;
            jumpIndicators[0].SetActive(false);
            isStart = true;
        }
#endif
        if (isStart)
        {
            anim.SetBool("isMoving", true);
            transform.Translate(new Vector2(2.5f * (posDir == true ? 1 : -1) * speed, 0) * Time.deltaTime);
        }
        if (rb.velocity.y < 0)
        {
            isFalling = true;
        }
        else if (rb.velocity.y > 0)
        {
            isFalling = false;
        }

    }

    public void JumpOrRun()
    {
        if (!isStart)
        {
            isStart = true;
        }
        else if (isStart)
        {
            if (isJumpEnabled && !isDead)
            {
                //touch input
                audioSource.clip = audioClips[1];
                audioSource.Play();
                ++timeJumpPressed;
                HideJumpIndicator(timeJumpPressed);
                anim.SetBool("isJumping", true);
                rb.velocity = Vector2.up * torqueForce;
                jumpIndicators[0].SetActive(false);
                isStart = true;

            }
        }
    }

    private void FixedUpdate()
    {
        _renderer.flipX = posDir == true ? false : true;
        if (Input.GetKeyDown(KeyCode.F))
        {
            StopPlayer();
        }
        OnRayCastCheck();
    }


    void OnRayCastCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right * (posDir == true ? 1 : -1)), 1.5f, 1 << LayerMask.NameToLayer("Walls"));
        RaycastHit2D hit1 = Physics2D.Raycast(wallCheck1.transform.position, transform.TransformDirection(Vector2.right * (posDir == true ? 1 : -1)), 1.5f, 1 << LayerMask.NameToLayer("Walls"));
        RaycastHit2D hit2 = Physics2D.Raycast(wallCheck2.transform.position, transform.TransformDirection(Vector2.right * (posDir == true ? 1 : -1)), 1.5f, 1 << LayerMask.NameToLayer("Walls"));
        RaycastHit2D groundHit = Physics2D.Raycast(bottom.transform.position, transform.TransformDirection(Vector2.down), 0.5f, 1 << LayerMask.NameToLayer("Walls"));

        if ((hit.collider != null && hit.collider.gameObject.tag == "RedWall") || (hit1.collider != null && hit1.collider.gameObject.tag == "RedWall") || (hit2.collider != null && hit2.collider.gameObject.tag == "RedWall"))
        {
            //apply velocity to opposite direction
            posDir = !posDir;
            //apply velocity on y-axis.
            float tempVel = rb.velocity.y;
            rb.velocity = Vector2.up * torqueForce;
            speed = jumpSpeed;
            anim.SetBool("isJumping", true);

        }
        if (groundHit.collider != null && groundHit.collider.gameObject.tag == "Ground" && groundHit.distance < 0.05f)
        {
            speed = 8f;
            if (isFalling)
            {
                ResetJumpIndicator();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
            PlayerDeathAction();
            GameManager.deathsToVideoAd++;
            TotalDeaths++;
            PlayerPrefs.SetInt("TotalDeaths", TotalDeaths);
            if (GameManager.deathsToVideoAd > 9)
            {
                gameManager.PlayVideoAd();
            }
        }

        if (collision.gameObject.tag == "CheckPoint")
        {
            if(LevelNumber+1 > MaxLevelUnlocked)
            {
                PlayerPrefs.SetInt("LevelPlayed", LevelNumber + 1);
            }            
            ++LevelNumber;
            if (LevelNumber > 10)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(LevelNumber);
            }

        }
    }

    void PlayerDeathAction()
    {
        //camera shake
        gameManager.ShakeCamera();
        GameManager.PLAYER_HEALTH--;
        gameManager.ResetStopButton();
        deathParticleSystem.Play();
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        _renderer.enabled = false;
        boxCollider2D.enabled = false;
        jumpIndicatorHodler.SetActive(false);
        StopPlayer();
        StartCoroutine(ReSpawn());
    }

    IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        //_renderer.flipX = false;       
        transform.position = startPoint.position;
        boxCollider2D.enabled = true;
        rb.gravityScale = 2;
        _renderer.enabled = true;
        jumpIndicatorHodler.SetActive(true);
        isDead = false;
    }

    public void StopPlayer()
    {
        isStart = false;
        anim.SetBool("isMoving", false);
    }

    void ResetJumpIndicator()
    {
        foreach (GameObject obj in jumpIndicators)
        {
            obj.SetActive(true);
        }
        timeJumpPressed = -1;
        isJumpEnabled = true;
        anim.SetBool("isJumping", false);
    }

    void HideJumpIndicator(int index)
    {
        if (index < jumpIndicators.Length)
        {
            jumpIndicators[index].SetActive(false);
        }
        if (index == jumpIndicators.Length - 1)
        {
            isJumpEnabled = false;
        }
    }

}
