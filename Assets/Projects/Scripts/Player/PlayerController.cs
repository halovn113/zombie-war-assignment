using KHCore;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public CharacterController characterController;


    [Header("Shooting")]
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float nextFireTime;
    public Weapon weapon;

    [Header("Input System")]
    private PlayerInputAction inputActions;
    private Vector2 _moveVector;
    private Vector2 _rotateVector;
    private Vector3 _moveDirection;
    private Vector3 _rotateDirection;
    [SerializeField] private float fireThreshold = 0.1f;

    [Header("References")]
    public Transform throwPosition;
    public float throwForce = 10f;
    private Vector3 lastAimDir;

    private bool isFiring;

    [Header("Animation")]
    public Animator animator;

    public LayerMask groundLayer;
    private Vector3 _mouseWorldPosition;

    private bool _isUsingMouse;

    private Player player;

    void Awake()
    {
        inputActions = new PlayerInputAction();
        animator.SetInteger("State", 0);
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        isFiring = false;
        inputActions.Enable();
        inputActions.Game.Walk.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        inputActions.Game.Walk.canceled += ctx => _moveVector = Vector2.zero;

        // show if mobile
        ServiceLocator.Get<UIManager>().GetUI(UIEnum.GAMEPLAY).GetComponent<UIGameplay>().virtualController.gameObject.SetActive(Application.isMobilePlatform);

        inputActions.Game.Aim.performed += ctx =>
        {
            _rotateVector = ctx.ReadValue<Vector2>();
            DetectInputDevice(ctx);
        };

        inputActions.Game.Aim.canceled += ctx =>
        {
            _rotateVector = Vector2.zero;
        };

        inputActions.Game.Shoot.performed += ctx => isFiring = true;
        inputActions.Game.Shoot.canceled += ctx => isFiring = false;

        inputActions.Game.SwitchWeapon.performed += ctx => SwitchWeapon(ctx);

        inputActions.Game.Bom.performed += _ => TryThrow();
        inputActions.Game.Aim.performed += ctx => UpdateAim(ctx.ReadValue<Vector2>());
    }

    void Update()
    {
        if (GetComponent<Player>().state == PLAYER_STATE.DEAD || ServiceLocator.Get<GameStateManager>().CurrentState != GAME_STATE.GAME)
        {
            return;
        }
        HandleMovement();
        HandleRotate();
        HandleShooting();
    }

    private void HandleMovement()
    {
        _moveDirection = new Vector3(_moveVector.x, 0, _moveVector.y);

        if (_moveDirection.magnitude > 0.1f)
        {
            characterController.Move(_moveDirection * player.stats.moveSpeed * Time.deltaTime);
            animator.SetInteger("State", 1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    private void HandleRotate()
    {
        if (_isUsingMouse)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                _mouseWorldPosition = hit.point;
                Vector3 directionToMouse = _mouseWorldPosition - transform.position;
                directionToMouse.y = 0;

                if (directionToMouse.magnitude > 0.1f)
                {
                    Quaternion aimRotation = Quaternion.LookRotation(directionToMouse, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, aimRotation, player.stats.rotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            _rotateDirection = new Vector3(_rotateVector.x, 0, _rotateVector.y);

            if (_rotateDirection.magnitude > 0.1f)
            {
                Quaternion aimRotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, aimRotation, player.stats.rotationSpeed * Time.deltaTime);
            }
        }

    }

    private void DetectInputDevice(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (device is Mouse)
        {
            _isUsingMouse = true;
        }
        else if (device is Gamepad)
        {
            _isUsingMouse = false;
        }
    }

    private void HandleShooting()
    {
        if (!_isUsingMouse)
        {
            isFiring = inputActions.Game.Aim.ReadValue<Vector2>().sqrMagnitude > 0.01f;
        }

        if (isFiring)
        {
            weapon.TryFire();
            animator.SetBool("Shooting", true);
            animator.SetFloat("ShootSpeed", weapon.weaponData.animShootSpeed);
        }
        else
        {
            animator.SetBool("Shooting", false);
            weapon.StopFire();
            animator.SetFloat("ShootSpeed", 1);
        }
    }

    private void SwitchWeapon(InputAction.CallbackContext ctx)
    {
        player.shooting.OnSwitchWeapon(ctx);
        weapon = player.shooting.GetCurrentWeapon();
        ServiceLocator.Get<AudioManager>().PlaySFX(SFX_EFFECT.SWITCH_WEAPON);
    }

    private void TryThrow()
    {
        if (lastAimDir == Vector3.zero)
        {
            return;
        }

        if (GetComponent<Player>().useGrenade)
        {
            return;
        }
        GetComponent<Player>().UseGrenade();
        var grenade = ServiceLocator.Get<GrenadePool>().Get();
        grenade.transform.position = transform.position;
        grenade.Throw(throwPosition.position);
    }

    private void UpdateAim(Vector2 aim)
    {
        if (aim.sqrMagnitude > 0.01f)
            lastAimDir = new Vector3(aim.x, 0, aim.y).normalized;
    }
}
