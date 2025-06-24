using UnityEngine;

public class BatAI : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float attackSpeed = 5f;
    public float detectRange = 5f;
    private Vector3 target;
    private Animator animator;
    private bool isAttacking = false;
    private Transform playerTarget;

    void Start()
    {
        target = pointB.position;
        animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("MoveTrigger");
    }

    void Update()
    {
        if (!isAttacking)
        {
            // Flip hướng dơi trước khi di chuyển
            if (target.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (target.x < transform.position.x)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Di chuyển tuần tra giữa pointA và pointB (trên không)
            transform.position = Vector3.MoveTowards(transform.position, target, patrolSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
                target = pointB.position;
            else if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
                target = pointA.position;

            if (animator != null)
                animator.SetTrigger("MoveTrigger");

            // Phát hiện player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                PlayerBalance pb = player.GetComponent<PlayerBalance>();
                if (dist < detectRange && pb != null && !pb.hasDoubleJumped)
                {
                    isAttacking = true;
                    playerTarget = player.transform;
                }
            }
        }
        else
        {
            // Flip hướng dơi trước khi di chuyển tới player
            if (playerTarget != null)
            {
                if (playerTarget.position.x > transform.position.x)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                else if (playerTarget.position.x < transform.position.x)
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                // Bay thẳng tới player
                transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, attackSpeed * Time.deltaTime);

                if (animator != null)
                    animator.SetTrigger("MoveTrigger");

                if (Vector3.Distance(transform.position, playerTarget.position) < 0.5f)
                {
                    if (animator != null)
                        animator.SetTrigger("AttackTrigger");
                    PlayerBalance pb = playerTarget.GetComponent<PlayerBalance>();
                    if (pb != null)
                        pb.TriggerGameOverByObstacle();
                    isAttacking = false;
                }
            }
        }
    }
} 