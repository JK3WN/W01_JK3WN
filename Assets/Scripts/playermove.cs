using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playermove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool isGrounded = true;
    public bool isStoped = false;
    public Rigidbody2D rb;

    private bool isAlive = true;

    private bool isInZone = false;

    public bool isPlaying = true;

    public bool isNoTime = false;

    public Vector2 postForce;

    // External Bind Objects
    public GameObject sandClockUI;
    public GameObject[] Indicators;
    public GameObject rMap;

    public TextDisplay TD;
    public GameObject clearUI;

    public string playerType;
    public string PlayerType {  get { return playerType; } }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isPlaying)
        {
            RayCheck();


            transform.rotation = Quaternion.Euler(rMap.transform.rotation.x,
                rMap.transform.rotation.y, -rMap.transform.rotation.z);

            // ÁÂ¿ì ÀÌµ¿
            float moveInput = Input.GetAxis("Horizontal");
            if (!isStoped)
            {
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            }

            // Á¡ÇÁ
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isStoped)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !rMap.GetComponent<RotateMap>().IsRotating && !isInZone && !isNoTime)
            {
                if (isStoped)
                {
                    isStoped = false;

                    foreach (GameObject ind in Indicators)
                    {
                        ind.SetActive(false);
                    }

                    sandClockUI.SetActive(false);
                    rb.gravityScale = 1;
                    rb.velocity = postForce;
                }
                else
                {
                    isStoped = true;

                    foreach (GameObject ind in Indicators)
                    {
                        ind.SetActive(true);
                    }

                    sandClockUI.SetActive(true);
                    postForce = rb.velocity;
                    rb.velocity = Vector2.zero;
                    rb.totalForce = Vector2.zero;
                    rb.gravityScale = 0;

                }

            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerPrefs.SetInt("FromGame", 1);
                SceneManager.LoadScene(0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            isPlaying = false;
            rb.velocity = Vector2.zero;
            rb.totalForce = Vector2.zero;
            rb.gravityScale = 0;
            if (PlayerPrefs.GetInt("Clear") < MenuManager.MapNum + 2) PlayerPrefs.SetInt("Clear", MenuManager.MapNum + 2);
            clearUI.SetActive(true);
            TD.MakeStar();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bound"))
        {
            Die();
        }
        if (collision.gameObject.CompareTag("NoStopZone"))
        {
            isInZone = true;
            isStoped = false;

            foreach (GameObject ind in Indicators)
            {
                ind.SetActive(false);
            }

            sandClockUI.SetActive(false);
            rb.gravityScale = 1;
            rb.velocity = postForce;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NoStopZone"))
        {
            isInZone = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NoStopZone"))
        {
            isInZone = true;
        }
    }

    public void RayCheck()
    {
        // 플레이어 벽 체크
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0.2f, Vector2.zero, 0.2f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Laser"))
            {

                rMap.GetComponent<RotateMap>().AttachPlayer();

                return;
            }
        }

        rMap.GetComponent<RotateMap>().DetachPlayer();
        return;

    }

    public void Die()
    {
        if (isAlive)
        {
            isAlive = false;
            TD.NowDeathUpdate();
            TD.TotalDeathUpdate();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeColor(string type)
    {
        Color color = type switch
        {
            "red" => Color.red,
            "blue" => Color.blue,
            "green" => Color.green,
            "purple" => Color.magenta,
            _ => Color.grey
        };

        playerType = type;

        GetComponent<SpriteRenderer>().color = color;
    }
}
