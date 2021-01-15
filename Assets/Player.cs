using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Player : MonoBehaviour
{   
    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);
    [SerializeField] float wallCheckDistance;

    public float dashTime;
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;

    private float dashTimeLeft;
    private float lastImgageXpos;
    private float lastDash = -100f;

    // State
    bool isAlive = true;
    bool isTouchingWall;
    private bool isDashing;
    private bool haveDashed;
    private bool haveTouchedGround = true;

    // Cached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;
    float gravityScaleAtStart;
    //public Transform wallCheck;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        ClimbLadder();
        Jump();
        FlipSprite();
        Die();
        Dash();
        CheckDash();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ladder"))) 
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return; 
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {   
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            myRigidBody.velocity = new Vector2(0f, jumpSpeed);
        }
        //if (CrossPlatformInputManager.GetButtonUp("Jump"))
        //{
            //myRigidBody.velocity = new Vector2(0f, jumpSpeed * variableJumpHeightMultiplier);
        //}
    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash"))
        {   
            AttemptToDash();
            //if (Time.time >= (lastDash + dashCoolDown))
            //if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
            //{
                //haveTouchedGround = true;
                //haveDashed = false;
            //}
            //else
            //{
                //haveTouchedGround = false;
                //haveDashed = true;
            //}
        }
    }

    private void AttemptToDash()
    {
        //if (!haveDashed || haveTouchedGround)
        //{
            isDashing = true;
            dashTimeLeft = dashTime;
            lastDash = Time.time;

            PlayerAfterImagePool.Instance.GetFromPool();
            lastImgageXpos = transform.position.x;
            //haveDashed = true;
        //}
    }

    private void CheckDash()
    {   
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                myRigidBody.velocity = new Vector2(dashSpeed* myRigidBody.velocity.x, myRigidBody.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                if(Mathf.Abs(transform.position.x - lastImgageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImgageXpos = transform.position.x;
                }
            }
            
        }
    }


    //private void CheckSurrounding()
    //{
        //isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance);
    //}

    //private void OnDrawGizmos()
    //{
        //Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    //}
}
