using System.Collections;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [Header("Attack Scripts")]
    [SerializeField] private EnemySphereCore sphereCore;
    [SerializeField] private EnemySphereMovement sphereMovement;
    [SerializeField] private EnemyChargeAttack chargeAttack;
    [SerializeField] private EnemyHorizontalSplitAttack horizontalSplitAttack;

    [Header("Attack Timing")]
    [SerializeField] private float startDelay = 1.5f;
    [SerializeField] private float timeBetweenAttacks = 1f;

    [Header("Split Attack Movement")]
    [SerializeField] private bool splitAttackCanMove = true;
    [SerializeField] private float splitAttackDelayAfterMovementStarts = 0.2f;
    [SerializeField] private bool waitForMovementAfterSplitAttack = true;

    [Header("Available Attacks")]
    [SerializeField] private bool useSplitAttack = true;
    [SerializeField] private bool useChargeAttack = true;
    [SerializeField] private bool useHorizontalSplitAttack = true;

    [Header("Debug")]
    [SerializeField] private bool automaticAttacks = true;

    private bool isRunningAttack;

    private void Awake()
    {
        if (sphereCore == null)
        {
            sphereCore = GetComponent<EnemySphereCore>();
        }

        if (sphereMovement == null)
        {
            sphereMovement = GetComponent<EnemySphereMovement>();
        }

        if (chargeAttack == null)
        {
            chargeAttack = GetComponent<EnemyChargeAttack>();
        }

        if (horizontalSplitAttack == null)
        {
            horizontalSplitAttack = GetComponent<EnemyHorizontalSplitAttack>();
        }
    }

    private void Start()
    {
        if (automaticAttacks)
        {
            StartCoroutine(AttackLoop());
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            yield return StartCoroutine(ExecuteNextAttack());

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    private IEnumerator ExecuteNextAttack()
    {
        if (isRunningAttack) yield break;

        isRunningAttack = true;

        int selectedAttack = ChooseRandomAttack();

        if (selectedAttack == 1)
        {
            yield return StartCoroutine(ExecuteSplitAttackWithMovementRule());
        }
        else if (selectedAttack == 2)
        {
            yield return StartCoroutine(MoveThenExecuteChargeAttack());
        }
        else if (selectedAttack == 3)
        {
            yield return StartCoroutine(MoveThenExecuteHorizontalSplitAttack());
        }

        isRunningAttack = false;
    }

    private IEnumerator ExecuteSplitAttackWithMovementRule()
    {
        if (splitAttackCanMove)
        {
            if (sphereMovement != null)
            {
                StartCoroutine(sphereMovement.MoveToRandomPoint());
            }

            yield return new WaitForSeconds(splitAttackDelayAfterMovementStarts);

            yield return StartCoroutine(ExecuteSplitAttack());

            if (waitForMovementAfterSplitAttack && sphereMovement != null)
            {
                while (sphereMovement.IsMoving)
                {
                    yield return null;
                }
            }
        }
        else
        {
            if (sphereMovement != null)
            {
                yield return StartCoroutine(sphereMovement.MoveToRandomPoint());
            }

            yield return StartCoroutine(ExecuteSplitAttack());
        }
    }

    private IEnumerator MoveThenExecuteChargeAttack()
    {
        if (sphereMovement != null)
        {
            yield return StartCoroutine(sphereMovement.MoveToRandomPoint());
        }

        yield return StartCoroutine(ExecuteChargeAttack());
    }

    private IEnumerator MoveThenExecuteHorizontalSplitAttack()
    {
        if (sphereMovement != null)
        {
            yield return StartCoroutine(sphereMovement.MoveToRandomPoint());
        }

        yield return StartCoroutine(ExecuteHorizontalSplitAttack());
    }

    private int ChooseRandomAttack()
    {
        int availableAttackCount = 0;

        if (useSplitAttack)
        {
            availableAttackCount++;
        }

        if (useChargeAttack)
        {
            availableAttackCount++;
        }

        if (useHorizontalSplitAttack)
        {
            availableAttackCount++;
        }

        if (availableAttackCount == 0)
        {
            return 0;
        }

        int randomIndex = Random.Range(0, availableAttackCount);
        int currentIndex = 0;

        if (useSplitAttack)
        {
            if (currentIndex == randomIndex)
            {
                return 1;
            }

            currentIndex++;
        }

        if (useChargeAttack)
        {
            if (currentIndex == randomIndex)
            {
                return 2;
            }

            currentIndex++;
        }

        if (useHorizontalSplitAttack)
        {
            if (currentIndex == randomIndex)
            {
                return 3;
            }
        }

        return 0;
    }

    private IEnumerator ExecuteSplitAttack()
    {
        if (sphereCore == null) yield break;

        yield return StartCoroutine(sphereCore.ExecuteSplitAttack());
    }

    private IEnumerator ExecuteChargeAttack()
    {
        if (chargeAttack == null) yield break;

        yield return StartCoroutine(chargeAttack.ExecuteChargeAttack());
    }

    private IEnumerator ExecuteHorizontalSplitAttack()
    {
        if (horizontalSplitAttack == null) yield break;

        yield return StartCoroutine(horizontalSplitAttack.ExecuteHorizontalSplitAttack());
    }
}