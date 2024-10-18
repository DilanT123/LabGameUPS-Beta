using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControler : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    private Vector2 movePlyr;
    private BoxCollider2D bodyBox;
    public Animator animator;

    [Header("Popup Settings")]
    public GameObject popUp;
    private ControladorPopUp controladorPopUp;
    private bool isNearTaskTrigger = false;

    [Header("Sync Settings")]
    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector3 syncPosition;
    [SyncVar(hook = nameof(OnMovementChanged))]
    private Vector2 syncMovement;
    [SyncVar(hook = nameof(OnAnimationChanged))]
    private string currentAnimation;
    [SyncVar(hook = nameof(OnNickNameChanged))]
    private string nickName = "";

    public string NickName
    {
        get => nickName;
        set
        {
            if (isLocalPlayer)
            {
                CmdSetNickName(value);
            }
        }
    }

    [Command]
    private void CmdSetNickName(string newName)
    {
        nickName = newName;
        // Actualizar el nombre en SyncVarPlayers
        var syncVarPlayers = GetComponent<SyncVarPlayers>();
        if (syncVarPlayers != null)
        {
            syncVarPlayers.UpdateDisplayName(newName);
        }
    }

    private void OnNickNameChanged(string oldName, string newName)
    {
        // Update the display name in SyncVarPlayers
        var syncVarPlayers = GetComponent<SyncVarPlayers>();
        if (syncVarPlayers != null)
        {
            syncVarPlayers.UpdateDisplayName(newName);
        }
    }



    private void Start()
    {
        bodyBox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        controladorPopUp = FindObjectOfType<ControladorPopUp>();

        if (!isLocalPlayer)
        {
            // Desactiva el control de física para los jugadores no locales
            if (GetComponent<Rigidbody2D>() != null)
                GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();
            HandlePopUpInput();
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            MovePlayer();
        }
    }

    [Client]
    private void HandleInput()
    {
        movePlyr = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        CmdSetMovement(movePlyr);
    }

    [Command]
    private void CmdSetMovement(Vector2 newMovement)
    {
        syncMovement = newMovement;
    }

    [Command]
    private void CmdSetPosition(Vector3 newPosition)
    {
        syncPosition = newPosition;
    }

    [Client]
    private void MovePlayer()
    {
        Vector2 moveAmount = movePlyr * speed * Time.fixedDeltaTime;
        Vector2 targetPosition = (Vector2)transform.position + moveAmount;
        MoveWithCollision(targetPosition);
        CmdSetPosition(transform.position);
    }

    private void MoveWithCollision(Vector2 targetPosition)
    {
        Vector2 startPosition = transform.position;
        Vector2 movement = targetPosition - (Vector2)startPosition;

        RaycastHit2D hit = Physics2D.BoxCast(startPosition, bodyBox.size, 0, movement.normalized, movement.magnitude, LayerMask.GetMask("Player", "Blocking"));

        if (hit.collider == null)
        {
            // No hay colisión, mover normalmente
            transform.position = targetPosition;
        }
        else
        {
            // Hay colisión, mover hasta el punto de colisión
            float distanceToObstacle = hit.distance;
            transform.position = startPosition + movement.normalized * (distanceToObstacle - 0.01f);
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", syncMovement.x);
            animator.SetFloat("Vertical", syncMovement.y);

            string animationName = "Personaje_idle";

            if (syncMovement.magnitude > 0.1f)
            {
                if (Mathf.Abs(syncMovement.x) > Mathf.Abs(syncMovement.y))
                {
                    animationName = syncMovement.x > 0 ? "Personaje_WalkRight" : "Personaje_WalkLeft";
                }
                else
                {
                    animationName = syncMovement.y > 0 ? "Personaje_WalkUp" : "Personaje_WalkDown";
                }
            }

            if (currentAnimation != animationName)
            {
                CmdSetAnimation(animationName);
            }
        }
    }

    [Command]
    private void CmdSetAnimation(string animationName)
    {
        currentAnimation = animationName;
        RpcSetAnimation(animationName);
    }

    [ClientRpc]
    private void RpcSetAnimation(string animationName)
    {
        if (!isLocalPlayer)
        {
            currentAnimation = animationName;
            if (animator != null)
            {
                animator.Play(animationName);
            }
        }
    }

    private void OnAnimationChanged(string oldAnimation, string newAnimation)
    {
        if (!isLocalPlayer && animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(newAnimation))
        {
            animator.Play(newAnimation);
        }
    }

    private void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        if (!isLocalPlayer)
        {
            transform.position = newPosition;
        }
    }

    private void OnMovementChanged(Vector2 oldMovement, Vector2 newMovement)
    {
        if (!isLocalPlayer)
        {
            movePlyr = newMovement;
        }
    }

    

    public void HandlePopUpInput()
    {
        if (controladorPopUp != null && isNearTaskTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Asegúrate de pasar el SyncVarPlayers adecuado si es necesario
                var syncVarPlayers = GetComponent<SyncVarPlayers>();
                controladorPopUp.MostrarPopUp(syncVarPlayers, "MultipleChoice", "Respuesta1;Respuesta2;Respuesta3;Respuesta4");
                isNearTaskTrigger = false; // Desactivar la interacción después de usarla
            }
        }
    }

    public void SetNearTaskTrigger(bool isNear)
    {
        isNearTaskTrigger = isNear;
    }

    public override void OnStartLocalPlayer()
    {
        if (GameManager.GetInstance().localPlayer == null)
        {
            GameManager.GetInstance().localPlayer = this;
        }
    }

    

}
