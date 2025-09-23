using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerElevator : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;
    private bool goingUp = false;
    private bool isMoving = false;

    private Transform player;
    private BoxCollider2D boxTriggerCollider;

    private void Awake()
    {
        boxTriggerCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (playerInside && !isMoving)
        {
            if (Input.GetKeyDown(interactKey))
            {
                isMoving = true;
                goingUp = !goingUp;
            }
        }

        if (isMoving)
        {
            Transform target = goingUp ? topPoint : bottomPoint;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) < 0.01f)
            {
                transform.position = target.position;
                isMoving = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(this);
    }
}
