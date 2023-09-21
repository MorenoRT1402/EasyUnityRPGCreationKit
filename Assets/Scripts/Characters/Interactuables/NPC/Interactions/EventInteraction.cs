using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EventInteraction : CharacterMovement
{
    public enum TriggerType { ACTUAL, NONE, TOUCH, PRESS_KEY }
    public enum MoveRoute { ACTUAL, STATIC, RANDOM, CUSTOM }

    [Header("Interaction")]
    public Image npcInteractionButtonImage;
    public TextMeshProUGUI npcInteractionButtonText;
    public TriggerType triggerType = TriggerType.PRESS_KEY;

    [Header("Move")]
    public MoveRoute moveRoute = MoveRoute.STATIC;

    public float waitTime = 0.5f;
    public Vector3[] defaultRoute;
    public bool routeLoop = true;

    private GeneralManager GM;
    private Vector3 initialPosition;
    private int defaultRouteActualIndex = -1;
    private float waitTimer = 0;
    protected List<EventSO> actionList;

    [HideInInspector] public bool changeEventListMark;

    public Vector3 InitialPosition => initialPosition;
    public List<EventSO> ActionList { get { return actionList; } set { actionList = value; } }
    public int DefaultRouteActualIndex
    {
        get { return defaultRouteActualIndex; }
        set { defaultRouteActualIndex = value; }
    }


    private new void Awake()
    {
        base.Awake();

        GM = GeneralManager.Instance;

        diagonalMove = true;

    }

    private void Start()
    {
        initialPosition = transform.position;

        npcInteractionButtonImage.gameObject.SetActive(false);
        npcInteractionButtonText.gameObject.SetActive(false);

    }

    private void Update()
    {
        MoveUpdate();
        CheckInteraction();
    }

    protected void OnDrawGizmos()
    {
        GM = GeneralManager.Instance;
        UIManager UIM = UIManager.Instance;

        if (GM.GameStarted == false && transform.hasChanged)
            initialPosition = transform.position;
        if (defaultRoute == null || defaultRoute.Length <= 0) return;

        for (int i = 0; i < defaultRoute.Length; i++)
        {
            Gizmos.color = UIM.routeSpheresColor;
            Gizmos.DrawWireSphere(defaultRoute[i] + initialPosition, UIM.routeSpheresRadius);
            if (i < defaultRoute.Length - 1)
            {
                Gizmos.color = UIM.routeLinesColor;
                Gizmos.DrawLine(defaultRoute[i] + initialPosition, defaultRoute[i + 1] + initialPosition);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InteractionUpdate(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractionUpdate(other, false);
    }

    public virtual List<EventSO> GetEventList()
    {
        return new();
    }

    internal virtual void SetNewEventList(List<EventSO> newEventList)
    {
        actionList = newEventList;
    }

    #region Interaction
    private void InteractionUpdate(Collider2D other, bool state) //state => true = enter; false = exit
    {
        if (GameManager.InPriorityUI()) SetActiveInteractionButton(false);

        if (!other.CompareTag("Player")) return;

        EventManager.nearestEventInteraction = state ? this : null;

        if (UIManager.Instance.showNPCInteractKey)
        {
            if (triggerType == TriggerType.PRESS_KEY)
                SetActiveInteractionButton(state);
            else
                SetActiveInteractionButton(false);
        }
    }

    private void CheckInteraction()
    {
        //        Debug.Log($"Nearest {EventManager.nearestEventInteraction} Actual {EventManager.actualEventInteraction}");
        //        Debug.Log($"{EventManager.nearestEventInteraction} != {this} || {EventManager.actualEventInteraction} != null ? {EventManager.nearestEventInteraction != this || EventManager.actualEventInteraction != null}");
        if (EventManager.nearestEventInteraction != this || EventManager.actualEventInteraction != null) return;
//                Debug.Log($"triggerType {triggerType} ActionKeyDowned {Input.GetKeyDown(InputManager.Instance.action)} = {triggerType == TriggerType.PRESS_KEY && Input.GetKeyDown(InputManager.Instance.action) || triggerType == TriggerType.TOUCH}");
        if (triggerType == TriggerType.PRESS_KEY && Input.GetKeyDown(InputManager.Instance.action) || triggerType == TriggerType.TOUCH)
            Interact();
    }

    protected virtual void Interact()
    {
//                Debug.Log($"{actionList.Count} > {0} ? {actionList.Count > 0}");
        actionList = GetEventList();
//                Debug.Log($"{actionList.Count} > {0} ? {actionList.Count > 0}");

        GameManager.StartEventInteraction(this);
        SetActiveInteractionButton(false);
    }

    private void SetActiveInteractionButton(bool enable)
    {
        npcInteractionButtonImage.gameObject.SetActive(enable);
        npcInteractionButtonText.gameObject.SetActive(enable);
    }
    #endregion

    #region Movement

    private void MoveUpdate()
    {
        EventInteraction actualEI = EventManager.actualEventInteraction;
        if (actualEI != null && actualEI.id == id) return;

        if (EventManager.actualEventInteraction == this) return;
        if (!isMoving)
        {
            if (waitTimer < waitTime)
                waitTimer += Time.deltaTime;
            else
            {
                Vector3 moveVector = GetTargetPos();
                if (!diagonalMove && moveVector.x != 0) moveVector.y = 0;

                if (moveVector != Vector3.zero)
                {
                    animator.SetFloat("XSpeed", moveVector.x);
                    animator.SetFloat("YSpeed", moveVector.y);

                    Move(moveVector);
                }
                waitTimer = 0;
            }
        }
    }

    private Vector3 GetTargetPos()
    {
        return moveRoute switch
        {
            MoveRoute.STATIC => new Vector3(0, 0, 0),
            MoveRoute.RANDOM => new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0),
            MoveRoute.CUSTOM => GetDefaultRouteTargetPos(),
            _ => new Vector3(0, 0, 0),
        };
    }

    private Vector3 GetDefaultRouteTargetPos()
    {
        if (defaultRoute == null || defaultRoute.Length <= 0) return Vector3.zero;

        if (defaultRoute.Length > defaultRouteActualIndex + 1) // length >= index + 2 
            defaultRouteActualIndex++;
        else if (routeLoop)
            defaultRouteActualIndex = 0;
        else
            return Vector3.zero;

        Vector3 relativeInitialPos = transform.position - initialPosition;
        Vector3 relativeTargetPos = defaultRoute[defaultRouteActualIndex] - relativeInitialPos;

        return relativeTargetPos;
    }

    internal void Move(Vector3 displacement)
    {
        var targetPos = transform.position + displacement;

        if (IsWalkable(targetPos))
            StartCoroutine(MoveCR(targetPos));
    }

    public IEnumerator MoveCR(Vector3 targetPos)
    {
        isMoving = true;
        animator.SetBool("IsMoving", true);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //            DebugManager.DebugLog(EventManager.actualEventInteraction == null, "null", $"{EventManager.actualEventInteraction.id} == {id} ? {EventManager.actualEventInteraction.id != id}");
                //            Debug.Log($"{targetPos} - {transform.position}.sqrMagnitude = {(targetPos - transform.position).sqrMagnitude} > {Mathf.Epsilon} ? {(targetPos - transform.position).sqrMagnitude > Mathf.Epsilon}");
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        new WaitForSeconds(waitTime);

        isMoving = false;
        animator.SetBool("IsMoving", false);
    }

    private bool InRoute()
    {
        bool inEM = EventManager.actualEventInteraction == this;
        bool eMInRoute = EventManager.GetActualEvent().eventType == EventSO.GameEventType.ROUTE;
        return inEM && eMInRoute;
    }

    #endregion
}
