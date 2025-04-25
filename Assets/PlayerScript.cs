using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float walkSpeed, runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float animWalkSpeed, animRunSpeed;
    [SerializeField] private AudioClip jumpSound, sprintSound;
    
    private Rigidbody2D m_rigidbody2D;
    private Animator m_animator;
    private ParticleSystem m_particleSystem;
    private AudioSource m_audioSource;
    
    private float m_xMovement;
    private bool m_isRunning, m_isJumping, m_onGround;
    private void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_particleSystem = GetComponentInChildren<ParticleSystem>();
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float linearVelocity = Mathf.Abs(m_rigidbody2D.linearVelocityX);
        m_animator.SetBool("IsWalking", linearVelocity >= animWalkSpeed);
        m_animator.SetBool("IsRunning", linearVelocity >= animRunSpeed);
        m_animator.SetBool("IsFalling", m_rigidbody2D.linearVelocityY < -.01f);
    }

    private void FixedUpdate()
    {
        if(m_xMovement > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if(m_xMovement < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        if (m_onGround && m_isJumping)
        {
            m_isJumping = false;
            m_animator.SetBool("IsJumping", true);
        }

        Vector2 force = Vector2.right * (m_xMovement * (m_isRunning ? runSpeed : walkSpeed));
        m_rigidbody2D.AddForce(60 * Time.deltaTime * force, ForceMode2D.Force);
    }

    public void Jump()
    {
        m_rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        m_audioSource.PlayOneShot(jumpSound);
    }

    private void OnMove(InputValue inputValue)
    {
        m_xMovement = inputValue.Get<float>();
    }

    private void OnRun()
    {
        m_isRunning = !m_isRunning;
        m_audioSource.PlayOneShot(sprintSound);
    }

    private void OnJump()
    {
        m_isJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            m_onGround = true;
            m_particleSystem.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            m_onGround = false;
            m_animator.SetBool("IsJumping", false);
        }
    }
}
