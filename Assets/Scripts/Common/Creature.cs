using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abilities.Actives;
using Assets.Scripts.Abilities.Passives;
using Assets.Scripts.Grid;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class Creature : MonoBehaviour
    {
        public Stats stats;
        [SerializeField] private int defaultStack = 1;
        [SerializeField] private Team team;
        [SerializeField] private TextMeshPro stackText;

        private const float MovementAnimationSpeed = 5;

        private bool moveTargetReached = true;
        private Vector3 moveTarget;
        private Queue<Vector2Int> moveSequence = new Queue<Vector2Int>();
        private Creature targetEnemy;

        public int currentHealth { get; private set; }

        public int currentStack { get; private set; }

        public Vector2Int gridPosition { get; private set; }

        public ActiveAbility activeAbility { get; private set; }

        private List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();

        void Start()
        {
            var position = GridManager.instance.GetGridPositionFromWorld(transform.position).Value;
            Initialize(position, defaultStack);
        }

        void FixedUpdate()
        {
            Move();
        }

        public void Initialize(Vector2Int position, int stack)
        {
            passiveAbilities = GetComponents<PassiveAbility>()
                .OrderBy(ability => ability.priority)
                .ToList();

            activeAbility = GetComponent<ActiveAbility>();

            SetGridPosition(position);
            SetStackValue(stack);

            currentHealth = stats.health * stack;

            SetTeam(team);

            transform.position = GridManager.instance.GetWorldPositionFromGrid(position).Value;
        }

        private void Move()
        {
            if (moveTargetReached)
            {
                return;
            }

            // checking if target move point reached 
            var distanceToTarget = Vector3.Distance(transform.position, moveTarget);
            if (distanceToTarget < 0.001f)
            {
                // go to next point if it exists
                if (moveSequence.Count > 0)
                {
                    var nextMove = moveSequence.Dequeue();
                    moveTarget = GridManager.instance.GetWorldPositionFromGrid(nextMove).Value;
                }
                else
                {
                    // end reached
                    moveTargetReached = true;
                    SetGridPosition(GridManager.instance.GetGridPositionFromWorld(transform.position).Value);

                    // if enemy was targeted, attack it
                    if (targetEnemy != null)
                    {
                        Attack(targetEnemy, AttackType.Melee);
                    }
                    else
                    {
                        CombatControlManager.instance.ResumeTurn(stats.turnRepeatChance);
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, moveTarget, MovementAnimationSpeed * Time.fixedDeltaTime);
            }
        }

        public void Attack(Creature target, AttackType attackType)
        {
            // if enemy too close for range attack, switch it to melee
            var isNeighbors = GridManager.instance.CheckIfNeighbors(gridPosition, target.gridPosition);
            if (isNeighbors && attackType == AttackType.Range)
            {
                attackType = AttackType.Melee;
            }

            var totalDamage = attackType == AttackType.Range
                ? GetRangeDamage()
                : GetMeleeDamage();

            target.Damage(totalDamage, this, attackType);

            targetEnemy = null;

            foreach (var passiveAbility in passiveAbilities)
            {
                passiveAbility.OnAfterAttack(this, totalDamage, target, attackType);
            }

            if (attackType != AttackType.CounterAttack)
            {
                if (currentHealth > 0)
                {
                    CombatControlManager.instance.NextTurn(stats.turnRepeatChance);
                }
                else
                {
                    CombatControlManager.instance.NextTurn();
                }
            }
        }

        public void Damage(int damage, Creature attacker, AttackType attackType)
        {
            foreach (var passiveAbility in passiveAbilities)
            {
                passiveAbility.OnBeforeDamage(this, ref damage, ref attacker, ref attackType);
            }

            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            LogManager.instance.Log($"{attacker.name} deals {damage} damage to {name}. ({attackType})");

            var damagedUnit = currentHealth % stats.health > 0 ? 1 : 0;
            var newStackValue = currentHealth / stats.health + damagedUnit;
            var unitsLost = currentStack - newStackValue;
            SetStackValue(newStackValue);

            foreach (var passiveAbility in passiveAbilities)
            {
                passiveAbility.OnAfterDamage(this, damage, unitsLost, attacker, attackType);
            }

            if (currentHealth <= 0)
            {
                Kill();
            }

            if (attackType == AttackType.Melee)
            {
                Attack(attacker, AttackType.CounterAttack);
            }
        }

        public void Kill()
        {
            LogManager.instance.Log($"{name} dies.");
            CombatControlManager.instance.RemoveCreatureFromQueue(this);
            CombatControlManager.instance.NextTurn();
            Destroy(gameObject);
        }

        public void Heal(int healAmount)
        {
            var maxHealAmount = currentHealth % stats.health;
            var totalHeal = Mathf.Clamp(healAmount, 0, maxHealAmount);
            currentHealth += totalHeal;
            LogManager.instance.Log($"{name} heals {totalHeal} hp.");
        }

        public void SetMeleeAttackSequence(List<Vector2Int> movePositions, Creature enemy)
        {
            targetEnemy = enemy;
            if (movePositions.Count > 0)
            {
                SetMoveSequence(movePositions);
            }
            else
            {
                Attack(enemy, AttackType.Melee);
            }
        }

        public void SetMoveSequence(IEnumerable<Vector2Int> positions)
        {
            moveTargetReached = false;
            moveSequence = new Queue<Vector2Int>(positions);
            var nextMove = moveSequence.Dequeue();
            moveTarget = GridManager.instance.GetWorldPositionFromGrid(nextMove).Value;
        }

        public T AppendAbilityOrStatus<T>() where T : PassiveAbility
        {
            var ability = gameObject.AddComponent<T>();
            passiveAbilities.Add(ability);
            passiveAbilities.Sort((x, y) => x.priority - y.priority);
            ability.OnAbilityApplied(this);
            return ability;
        }

        public void TriggerEndTurnEffects()
        {
            foreach (var passiveAbility in passiveAbilities)
            {
                passiveAbility.OnTurnEnd(this);
            }

            ClearRemovedAbilities();
        }

        private void ClearRemovedAbilities()
        {
            var removedAbilities = passiveAbilities.Where(x => x.toRemove).ToArray();
            foreach (var ability in removedAbilities)
            {
                ability.OnAbilityRemoved(this);
                passiveAbilities.Remove(ability);
                Destroy(ability, 10);
            }
        }

        private void SetStackValue(int newValue)
        {
            currentStack = newValue;
            stackText.text = currentStack.ToString();
        }

        public void SetGridPosition(Vector2Int position, bool move = false)
        {
            gridPosition = position;
            GridManager.instance.SetCreatureToCell(gridPosition, this);
            if (move)
            {
                transform.position = GridManager.instance.GetWorldPositionFromGrid(position).Value;
            }
        }

        public void SetTeam(Team newTeam)
        {
            team = newTeam;
            stackText.color = newTeam == Team.First ? Color.blue : Color.red;
        }

        public int GetMeleeDamage()
        {
            var baseDamage = Random.Range(stats.minDamageMelee, stats.maxDamageMelee + 1);
            return baseDamage * currentStack;
        }

        public int GetRangeDamage()
        {
            var baseDamage = Random.Range(stats.minDamageRange, stats.maxDamageRange + 1);
            return baseDamage * currentStack;
        }

        public Team GetTeam()
        {
            return team;
        }

        public int GetStack()
        {
            return currentStack;
        }

        public bool IsRanged()
        {
            return stats.minDamageRange > 0;
        }
    }
}
