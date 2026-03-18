

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum enDirection
    {
        North,
        East,
        West
    };

    public AudioClip[] soundFXClips;
    public Text distanceScoreText;
    public Text coinScoreText;
    public Text bestDistanceScoreText;
    public Text bestCCoinScoreText;
    public GameObject deathMenu;

    CharacterController characterController;
    Vector3 playerVector;

    enDirection playerDirection = enDirection.North;
    enDirection playerNextDirection = enDirection.North;

    Animator anim;
    BridgeSpawner bridgeSpawner;
    AudioSource audioSource;

    int coinsCollected = 0;
    int coinsCollectedBest;

    int distanceRun = 0;
    int distanceRunBest;

    float playerStartSpeed = 10.0f;
    float playerSpeed;

    float gValue = 10.0f;
    float translationFactor = 10.0f;
    float jumpForce = 1.5f;

    float timer = 0;
    float distance = 0;

    bool canTurnRight = false;
    bool canTurnLeft = false;

    bool isDead = false;
    bool isSliding = false;

    void Start()
    {
        playerSpeed = playerStartSpeed;

        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        bridgeSpawner = GameObject.Find("BridgeManager").GetComponent<BridgeSpawner>();

        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime;

        deathMenu.SetActive(false);

        distanceRunBest = PlayerPrefs.GetInt("highscoreD");
        coinsCollectedBest = PlayerPrefs.GetInt("highscoreC");
    }

    void Update()
    {
        if (isDead) return;

        PlayerLogic();

        distanceScoreText.text = distanceRun.ToString();
        coinScoreText.text = "x" + coinsCollected.ToString();

        if (transform.position.y < -2f)
        {
            PlayerDied();
        }
    }

    void PlayerLogic()
    {
        if (isDead) return;

        timer += Time.deltaTime;
        playerSpeed += 0.005f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G) && canTurnRight)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;

                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }

            audioSource.PlayOneShot(soundFXClips[6], 0.4f);
        }

        else if (Input.GetKeyDown(KeyCode.F) && canTurnLeft)
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;

                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }

        playerDirection = playerNextDirection;

        if (playerDirection == enDirection.North)
            playerVector = Vector3.forward * playerSpeed * Time.deltaTime;

        else if (playerDirection == enDirection.East)
            playerVector = Vector3.right * playerSpeed * Time.deltaTime;

        else if (playerDirection == enDirection.West)
            playerVector = Vector3.left * playerSpeed * Time.deltaTime;

        switch (playerDirection)
        {
            case enDirection.North:
                playerVector.x = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;

            case enDirection.East:
            case enDirection.West:
                playerVector.z = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            DoSliding();
        }

        if (characterController.isGrounded)
        {
            playerVector.y = -0.2f;
        }
        else
        {
            playerVector.y -= gValue * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            audioSource.PlayOneShot(soundFXClips[3], 0.4f);
            anim.SetTrigger("isJumping");
            playerVector.y = Mathf.Sqrt(jumpForce * gValue);
        }

        characterController.Move(playerVector);

        distance = playerSpeed * timer;
        distanceRun = (int)distance;
    }

    void DoSliding()
    {
        if (isSliding) return;

        isSliding = true;

        characterController.height = 1.0f;
        characterController.center = new Vector3(0, 0.5f, 0);

        StartCoroutine(ReEnableCC());

        audioSource.PlayOneShot(soundFXClips[5], 0.4f);
        anim.SetTrigger("isSliding");
    }

    IEnumerator ReEnableCC()
    {
        yield return new WaitForSeconds(0.5f);

        characterController.height = 2.0f;
        characterController.center = new Vector3(0, 1f, 0);

        isSliding = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }
        else if (hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }
        else
        {
            canTurnLeft = false;
            canTurnRight = false;
        }

        if (hit.gameObject.tag == "Obstacle" && !isSliding)
        {
            isDead = true;
            audioSource.PlayOneShot(soundFXClips[1], 0.4f);
            anim.SetTrigger("isTripping");
            SaveScore();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coins")
        {
            coinsCollected++;

            Destroy(other.gameObject);

            if (soundFXClips.Length > 7)
                audioSource.PlayOneShot(soundFXClips[7], 0.4f);
        }
    }

    private void OnGUI()
    {
        if (isDead)
        {
            deathMenu.SetActive(true);
        }

    }


    public void DeathEvent()
    {
        Time.timeScale = 1f;

        transform.position = new Vector3(0f, 2.7f, 0f);
        transform.rotation = Quaternion.identity;

        coinsCollected = 0;
        timer = 0;
        distanceRun = 0;
        playerSpeed = playerStartSpeed;

        anim.SetTrigger("isSpawned");

        isDead = false;

        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;

        deathMenu.SetActive(false);

        bestCCoinScoreText.text = "";
        bestDistanceScoreText.text = "";

        bridgeSpawner.CleanTheScene();
        CleanTheScene();
    }

    void FootStepEventA()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }
    void FootStepEventB()
    {
        audioSource.PlayOneShot(soundFXClips[0], 0.4f);
    }
    void JumpLandEvent()
    {
        audioSource.PlayOneShot(soundFXClips[4], 0.4f);
    }


    public void CleanTheScene()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coins");

        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
    }

    void SaveScore()
    {
        if (coinsCollected > coinsCollectedBest)
        {
            coinsCollectedBest = coinsCollected;

            PlayerPrefs.SetInt("highscoreC", coinsCollectedBest);
            PlayerPrefs.Save();

            bestCCoinScoreText.text =
                "Wow! You've Achieved a New Best Coin Score of: " + coinsCollectedBest;
        }

        if (distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;

            PlayerPrefs.SetInt("highscoreD", distanceRunBest);
            PlayerPrefs.Save();

            bestDistanceScoreText.text =
                "Congrats! You Have a New Best Running Score of: " + distanceRunBest + "M";
        }
    }

    void PlayerDied()
    {
        if (isDead) return;

        isDead = true;
        Time.timeScale = 0f;
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}

