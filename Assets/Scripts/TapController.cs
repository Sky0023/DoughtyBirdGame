using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]//This tells the unity that when we put TapController on my object then it automatically creates Rigidbody 2D for it.
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigidbody1;
    Quaternion downRotation;//Quaternion is a fancy form of rotation, it has four values vector 4(x,y,z,w)
    Quaternion forwardRoattion;

    GameManager game;

    void Start()
    {
        rigidbody1 = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRoattion = Quaternion.Euler(0, 0, 40);
        game = GameManager.Instance;
        rigidbody1.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody1.velocity = Vector3.zero;
        rigidbody1.simulated = true;
    }
    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.GameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            transform.rotation = forwardRoattion;
            rigidbody1.velocity = Vector3.zero;
            rigidbody1.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth*Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Score Zone")
        {
            //register a score event
            OnPlayerScored();//event sent to GameManager
            //play a sound
            scoreAudio.Play();
        }
        if(col.gameObject.tag == "Dead Zone")
        {
            rigidbody1.simulated = false;
            //register a dead event
            OnPlayerDied();//event sent to GameManager
            //play a sound
            dieAudio.Play();
        }
    }
}
