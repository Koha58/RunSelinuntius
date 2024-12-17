using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float acceleration = 0.01f;
    public float leftRightSpeed = 4f;
    public float limit = 5f;
    public Animator animator;

    public Vector3 jump;
    public float jumpForce = 2.0f;
    public bool isGrounded;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 3.0f, 0.0f);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void OnCollisionExit()
    {
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
        moveSpeed += Time.deltaTime * acceleration;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.position.x > -limit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed);
            }
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.position.x < limit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * leftRightSpeed * -1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool("Jump", true);
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            // コルーチンを開始して、0.8秒後に "Jump" を false にする
            StartCoroutine(SetJumpFalseAfterDelay(0.8f));
        }
    }

    IEnumerator SetJumpFalseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Jump", false);
    }
}
