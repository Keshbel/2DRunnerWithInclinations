using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    
    [Header("Sphere")]
    public Material sphereMaterial;
    public Transform sphereTransform;

    [Header("Animations")]
    public States currentState;
    private States oldState;
    public AnimationReferenceAsset idle, running, slipping;

    [Header("Movements")]
    public float speed;
    public float flip = 1;
    public float horizontalInput;
    [SerializeField] private Rigidbody2D rigidbody2D;

    private void Start()
    {
        if (!rigidbody2D) rigidbody2D = GetComponent<Rigidbody2D>();
        currentState = States.Idle;
    }

    private void Update()
    { 
        GetMoveInput();
        SetCharacterState();
    }

    private void FixedUpdate()
    {
        Moving();
    }

    private void SetAnimation(AnimationReferenceAsset anim, bool loop = true, float timeScale = 1)
    {
        skeletonAnimation.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
    }

    private void SetCharacterState()
    {
        if (oldState == currentState) return;
        
        sphereMaterial.color = currentState != States.Slipping ? Color.green : Color.black;
        
        switch (currentState)
        {
            case States.Idle:
                SetAnimation(idle);
                break;
            case States.Running:
                SetAnimation(running);
                break;
            case States.Slipping:
                SetAnimation(slipping);
                break;
        }

        oldState = currentState;
    }

    private void GetMoveInput()
    {
        //movements
        if (currentState != States.Slipping) horizontalInput = Input.GetAxis("Horizontal");
        
        //rotations (character&sphere)
        if (horizontalInput > 0f) flip = 1;
        else if (horizontalInput < 0f) flip = -1;
        
        skeletonAnimation.skeleton.ScaleX = flip;
        sphereTransform.localPosition = new Vector3(-flip * 2, 7, 0);
        
        //animations
        if (rigidbody2D.velocity.y < -0.4f || rigidbody2D.velocity.y > 0.4f ) currentState = States.Slipping;
        else if (horizontalInput != 0) currentState = States.Running; 
        else currentState = States.Idle;
    }

    private void Moving()
    {
        rigidbody2D.velocity = new Vector2(horizontalInput *speed, rigidbody2D.velocity.y);
    }

    public enum States
    {
        Idle, 
        Running, 
        Slipping
    }
}
