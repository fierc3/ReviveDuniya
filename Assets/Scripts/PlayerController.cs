using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public LifeBarScript lifeBarScript;

    [SerializeField]
    private Transform model;
    [SerializeField]
    private Transform targetLock;
    [SerializeField]
    private GameObject estusFlask;
    [SerializeField]
    private GameObject healEffect;
    [SerializeField]
    private GameObject bloodPrefab;
    [SerializeField]
    private Transform bloodPos;

    [SerializeField]
    private float fastSpeed = 12;
    [SerializeField]
    private float slowSpeed = 7;
    private float moveSpeed = 4;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private CapsuleCollider capsuleCol;
    private Rigidbody rb;

    [SerializeField] 
    private AudioClip swordDamageSound;

    private float lastDamageTakenTime = 0;

    //private Vector3 forwardLocked;

    [HideInInspector]
    public bool insideAuraMagic = false;
    [HideInInspector]
    public float swordCurrentDamage; // dano deste ataque da sword, setado pelo script nas animacoes

    public CameraShaker shaker;

    // Bonfire
    public Transform bonfire; // pai do bonfire
    public Text interactText; // texto dizendo para interagir com o bonfire
    private bool isBonfireLit; // controla se o bonfire esta aceso

    public AchievementManager achievementManager;
    public GameObject credits;

    private HumanAnimationController animController;
    [SerializeField]
    public LayerMask groundLayer;

    private Vector3 groundLocation;
    private bool isGrounded = false;


    [SerializeField]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;

    private bool OnSlope()
    {
        Debug.DrawRay(model.transform.position, Vector3.down, Color.green, capsuleCol.height * 0.5f + 0.3f);
        if(Physics.Raycast(model.transform.position, Vector3.down, out slopeHit, capsuleCol.height * 0.5f + 0.3f, groundLayer)){
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void Start()
    {
        animController = this.GetComponent<HumanAnimationController>();
        mainCamera = Camera.main;
        capsuleCol = model.GetComponentInChildren<CapsuleCollider>();
        rb = this.GetComponent<Rigidbody>();
        credits.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManagerScript.isBossDead) anim.SetBool("LockedCamera", false); // nao pode estar em modo de combate caso o boss esteja morto

        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (animController.IsEquipped()) moveSpeed = slowSpeed; // velocidade com a espada
        else moveSpeed = fastSpeed; // velocidade sem a espada

        if (animController.IsDrinking()) moveSpeed = 2; // velocidade bebendo estus

        if (animController.IsDead() || animController.IsGettingThrown() || animController.IsSweepFalling()) return; // retorna caso o jogador tenha caido ou esteja morto

        if (insideAuraMagic || GameManagerScript.gameIsPaused) // caso esteja dentro da aura magica ou o jogo esteja com o menu de pause aberto
        {
            animController.StopMovement();
            return;
        }

       // BonfireInteraction();

       // if (!anim.GetCurrentAnimatorStateInfo(2).IsName("None") || isBonfireLit) return; // retorna caso esteja tomando dano ou esteja morto

        Move();
        Rotation();
        EstusFlask();
        Attack();
        Dodge();

    }

    private bool IsGrounded()
    {
        float groundCheckDistance = capsuleCol.height * 0.5f + capsuleCol.radius;

        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }


    private void CheckFloor()
    {
        // Cast a ray downward from the object's position
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Ground layer detected
            Debug.Log("Ground detected!");

            // You can access information about the hit point and other properties
            groundLocation = hit.point;
            Debug.Log("Hit point: " + hit.point);
            Debug.Log("Hit normal: " + hit.normal);
            Debug.Log("Hit collider: " + hit.collider);
        }
        else
        {
            // No ground layer detected
            groundLocation = model.transform.position;
            Debug.Log("No ground detected!");
        }
    }

    public void DisableEstusFlask() // metodo chamado apos o boss ser derrotado
    {
        estusFlask.SetActive(false);
    }

    private void BonfireInteraction()
    {
        if (Vector3.Distance(model.transform.position, bonfire.position) < 2.5f && bonfire.gameObject.activeSelf)
        {
            interactText.gameObject.SetActive(!isBonfireLit);
            if (Input.GetKeyDown(KeyCode.E) && !isBonfireLit)
            {
               // anim.SetTrigger("LightBonfire");
                isBonfireLit = true;
                achievementManager.TriggerBonfireLit();
                credits.SetActive(true);
            }
        }
        else interactText.gameObject.SetActive(false);
    }

    private void RotateDirectionToGround(ref Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(model.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 groundNormal = hit.normal;
            Quaternion rotation = Quaternion.FromToRotation(model.transform.up, groundNormal);
            direction = rotation * direction;
        }
    }

private void Move()
    {
        float x = mainCamera.transform.TransformDirection(stickDirection).x;
        float z = mainCamera.transform.TransformDirection(stickDirection).z;
        if (x > 1) x = 1; // assegura que o jogador nao ira se mover mais rapido em diagonal
        if (z > 1) z = 1;
         
        if (animController.CanMove())
        {
            if(Mathf.Abs(animController.GetSpeed()) > 0.15f)
            {
                // Calculate the movement direction based on player input
                Vector3 movementDirection = new Vector3(x, 0f, z).normalized;

                /* if (isGrounded && movementDirection.magnitude > 0)
                 {
                     RotateDirectionToGround(ref movementDirection);
                 }*/

                // Move the model in the desired direction
                if (OnSlope())
                {
                    model.position += GetSlopeMoveDirection(movementDirection) * moveSpeed * Time.deltaTime;
                }
                else
                {
                    //model.position += movementDirection * moveSpeed * Time.deltaTime;
                    RaycastHit floorHit;
                    var originPos = model.transform.position;
                    originPos.y += 20f;
                    if (Physics.Raycast(originPos, Vector3.down, out floorHit, 10f, groundLayer))
                    {
                        var pos = model.position;
                        pos.y = floorHit.transform.position.y;
                        model.position = pos;
                    }

                    MovePlayer(movementDirection);


                }
                
                //MovePlayer(movementDirection);
                
            }
               //model.position += new Vector3(x * moveSpeed * Time.deltaTime, groundLocation.y , z * moveSpeed * Time.deltaTime); // move o jogador para frente
            float clampValue = 1; //Input.GetKey(KeyCode.Space) ? 1 : 0.35f; // controla a velocidade de caminhar e correr
            animController.SetSpeedWithDirection(stickDirection, clampValue);
            animController.SetHorizontalInput(stickDirection.x);
            animController.SetVerticalInput(stickDirection.z);

            if (animController.IsDrinking() && animController.GetSpeed() > 0.25f) animController.SetSpeed(0.25f); // desacelera o jogador caso ele esteja bebendo
            if (animController.IsDrinking() && animController.GetVertical() > 0.25f) animController.SetVerticalInput(0.25f); // desacelera o jogador caso ele esteja bebendo
        }
        
    }

    private void MovePlayer(Vector3 direction)
    {
        // Calculate the target velocity
        Vector3 targetVelocity = direction * moveSpeed;
        targetVelocity.y = GetComponent<Rigidbody>().velocity.y;

        // Apply the target velocity to the rigidbody
        GetComponent<Rigidbody>().velocity = targetVelocity;

        //Debug.Log("pum is grounded "  + isGrounded);
        // Check if the slope is too steep
        if (isGrounded && direction.magnitude > 0)
        {
            RaycastHit hit;
            var pos = model.transform.position;
            pos.y += 1f;
            Debug.DrawRay(pos, direction, Color.green, 10f);
            if (Physics.Raycast(pos, direction, out hit, 10f, groundLayer))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > 200f)
                {
                    // Prevent the player from walking up steep slopes
                    GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, 0f);
                }
            }
        }
    }


    /*   private void DodgeController() // Dodge da locked camera
       {
           Vector3 relativeForward = mainCamera.transform.TransformDirection(Vector3.forward);
           Vector3 relativeRight = mainCamera.transform.TransformDirection(Vector3.right);
           Vector3 relativeLeft = mainCamera.transform.TransformDirection(-Vector3.right);
           Vector3 relativeBack = mainCamera.transform.TransformDirection(-Vector3.forward) * 5;

           relativeForward.y = 0;
           relativeRight.y = 0;
           relativeLeft.y = 0;
           relativeBack.y = 0;

           if (Input.GetAxis("Horizontal") > 0.1f && Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = (relativeForward + relativeRight).normalized;
           }
           else if (Input.GetAxis("Horizontal") > 0.1f && Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = (relativeBack + relativeRight).normalized;
           }
           else if (Input.GetAxis("Horizontal") < -0.1f && Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = (relativeBack + relativeLeft).normalized;
           }
           else if (Input.GetAxis("Horizontal") < -0.1f && Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = (relativeForward + relativeLeft).normalized;
           }

           else if (Input.GetAxis("Horizontal") > 0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = relativeRight;
           }
           else if (Input.GetAxis("Horizontal") < -0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = relativeLeft;
           }
           else if (Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = relativeForward;
           }
           else if (Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
           {
               forwardLocked = relativeBack;
           }
           else if(!animController.IsDodging())
           {
               forwardLocked = targetLock.position - model.position;
               forwardLocked.y = 0;
           }
       }*/

    private void Rotation()
    {
        if (animController.IsAttacking()) return; // cant turn body during a normal attack

        Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection) * 4f;
        rotationOffset.y = 0;
        model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 30f);

        /*        if (!anim.GetBool("LockedCamera")) // camera livre
                {

                }*/
        /*    else // camera locked
            {
                //Vector3 rotationOffset = targetLock.position - model.position;
                //rotationOffset.y = 0;

                DodgeController(); // vira instantaneamente para o lado do dodge
                model.forward += Vector3.Lerp(model.forward, forwardLocked, Time.deltaTime * 20f);
            }
        */
    }

    private bool CanDoAnyAttack() => animController.CanAttack() && animController.IsEquipped() && !animController.IsDrinking();

    private void Attack()
    {
        if (InputManager.GetPrimaryAttackInput() && CanDoAnyAttack())
        {
            animController.DoLightAttack();
        }
        if (InputManager.GetSecondaryAttackInput() && CanDoAnyAttack())
        {
           animController.DoHeavyAttack();
        }

        if (InputManager.GetDrawSwordInput()) // botao do meio do mouse
        {
            animController.DoDrawWeapon();
        }

    }

    private void EstusFlask()
    {
        if (InputManager.GetEstusInput() && !animController.IsDrinking() && !animController.IsDodging())
        {
            animController.TakeDrink();
            //estusFlask.SetActive(true);
            StartCoroutine(DrinkEstus());
        }
    }

    IEnumerator DrinkEstus()
    {
        yield return new WaitForSeconds(1f);
        if (/*anim.GetCurrentAnimatorStateInfo(2).IsName("None") &&*/ lifeBarScript.estusFlask > 0 && !animController.IsDead())
        {
            lifeBarScript.UpdateLife(3);
            Instantiate(healEffect, model.position, Quaternion.identity, model.transform);
        }
        yield return new WaitForSeconds(0.5f);
        //estusFlask.SetActive(false);
        yield return new WaitForSeconds(3f);
    }

    private void Dodge()
    {
        //Vector3 diff = model.transform.eulerAngles - mainCamera.transform.eulerAngles;

        if (InputManager.GetDodgeInput() && CanDodge()) // rola caso nao esteja atacando e nem bebendo estus
        {
            animController.DoDodge();
        }
    }

  /*  private bool CanMove()
    {
        return anim.GetCurrentAnimatorStateInfo(2).IsName("None");
    }*/

    private bool CanDodge() // Verifica se o player pode rolar
    {
        return true;    
        //return !anim.GetBool("Attacking") && !anim.GetBool("Drinking") && !anim.GetCurrentAnimatorStateInfo(1).IsName("Sprinting Forward Roll");
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.name.Contains("Shock") && !animController.IsStunned())
        {
            RegisterDamage(4, other);
            return;
        }

        if (other.transform.root.name.Contains("Earth") && !animController.IsStunned())
        {
            RegisterDamage(4.2f, other);
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("AuraMagic"))
            insideAuraMagic = true;
    }

    private bool DamageInterval() // garante que tenha um tempo entre um dano e outro
    {
        return (Time.time > lastDamageTakenTime + 0.25f);
    }

    public void RegisterDamage(float damageAmount, GameObject damageDealer)
    {
        if (damageAmount == 0 || animController.IsStunned() || !DamageInterval()) return;

        animController.StopMovement();
        lastDamageTakenTime = Time.time; // atualiza o tempo do ultimo dano tomado
        capsuleCol.isTrigger = true;
        rb.isKinematic = true;
        //anim.SetBool("Intangible", true);
        //anim.SetBool("CanMove", false);
        animController.SetCanMove(false);
        lifeBarScript.UpdateLife(-damageAmount); // diminui a quantia de dano na vida
        DamageAnimation(damageAmount, damageDealer);
        shaker.ShakeCamera(0.3f);

        GameObject blood = Instantiate(bloodPrefab, bloodPos.position, Quaternion.identity);
        blood.transform.LookAt(damageDealer.transform.position);
        Destroy(blood, 0.2f);
    }

    private void DamageAnimation(float damageAmount, GameObject damageDealer)
    {
        if (damageAmount >= 4) // caso o dano seja muito forte, derruba o player
        {
            Vector3 dir = (damageDealer.transform.position - model.transform.position).normalized; // direcao para o boss
            float dot = Vector3.Dot(dir, model.transform.forward);

            if (dot >= 0) // estava olhando para o boss, cai de costas
                animController.DoFallDamage();
            else if (dot < 0) // estava de costas para o boss, cai de frente
                animController.DoFallForward();
            return;
        }

        switch (Random.Range(0, 3)) // caso o dano seja pequeno sorteia uma animacao
        {
            case 0:
                animController.DoTakeDamage();
                break;
            case 1:
                animController.DoTakeDamageLeft();
                break;
            case 2:
                animController.DoTakeDamageRight();
                break;
        }
        
    }

}
