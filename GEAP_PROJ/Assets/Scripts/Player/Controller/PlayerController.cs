using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float jumpForce = 5.0f;

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool dodge;
    [SerializeField]
    bool isJump;

    int attackCount = 0;

    bool isLastAttackEnd = true;

    float checkTime = 0;

    Vector3 moveVec;

    Rigidbody rigid;
    Animator anim;
    public Transform cameraTransform;

    void Start()
    {

    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }


    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        MouseInput();
    }
    void Dodge()
    {
        if (dodge)
        {
            rigid.AddForce(Vector3.forward * 100, ForceMode.Impulse);
        }
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        dodge = Input.GetKeyDown(KeyCode.E);
    }

    void MouseInput()
    {
        if (wDown || jDown) return;
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Defence(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Defence(false);
        }
    }

    void Defence(bool isDefending)
    {
        anim.SetBool("defence", isDefending);
        attackEnd();
    }

    void Attack()
    {
        if (isLastAttackEnd)
        {
            attackCount = (attackCount + 1) % 4;

            anim.SetInteger("attCount", attackCount);

            anim.SetBool("isAttack", true);

            if (attackCount == 3)
                isLastAttackEnd = false;
        }
    }

    void attackEnd()
    {
        attackCount = 0;
        anim.SetInteger("attCount", 0);
        anim.SetBool("isAttack", false);
        isLastAttackEnd = true;
    }

    void Move()
    {
        if (anim.GetBool("defence"))
            return;
        Quaternion v3Rotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);

        moveVec = v3Rotation * new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * (wDown ? 1f : 0.3f) * Time.deltaTime;

        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", wDown);
        if (wDown)
            attackEnd();
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rigid.AddForce(moveVec * 10, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
            attackEnd();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            anim.SetBool("isJump", false);
            isJump = false;
            attackEnd();
        }
    }
}

