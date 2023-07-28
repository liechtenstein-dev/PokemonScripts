using System;
using System.Collections;
using UnityEngine;

public class PlayerControlMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float moveSpeed;
    private bool isMoving;
    public LayerMask solidObjectLayer;
    public LayerMask grassLayer;
    [SerializeField] private int rangeOfEncounter;
    private Vector2 input;
    private Animator animator;

    private Functions functions;
    private Interpreter myInterpreter;
    private int increment;
    
    public event Action onEncounter; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Start()
    {
        functions = new Functions();
        myInterpreter = new Interpreter();
        functions.interpreter = myInterpreter;
        Debug.Log("Interpreter creado");
    }

    protected internal void ResetConfiguration()
    {
        functions.SettingValues(20);
        rangeOfEncounter = 20;
    }
    
    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (isWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * 0.5f * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        input = Vector2.zero;
        CheckForEncounters();
    }

    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            rangeOfEncounter -= functions.RotateNumber(increment += 3);
            Debug.Log(rangeOfEncounter);
            if (rangeOfEncounter <= 0)
            {
                onEncounter();
                animator.SetBool("isMoving", false);
            }
        }
    }
}