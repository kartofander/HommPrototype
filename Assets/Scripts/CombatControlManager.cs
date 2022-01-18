using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Grid;
using Assets.Scripts.Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CombatControlManager : MonoBehaviour
    {
        [SerializeField] private Button skipButton;
        [SerializeField] private Button skillButton;

        public static CombatControlManager instance;

        private Queue<Creature> creaturesQueue = new Queue<Creature>();

        private int availableActionPoints;
        private Creature activeCreature;
        private List<Node<Vector2Int>> availableMovePositions;
        private List<Node<Vector2Int>> availableHitPositions;
        private bool inputLocked;

        private bool skillActive;
        private List<GridCell> skillSelectedArea = new List<GridCell>();

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            StartCoroutine(InitCreaturesOnBoard());
        }

        IEnumerator InitCreaturesOnBoard()
        {
            var creatures = new Creature[0];
            while (creatures.Length == 0)
            {
                creatures = FindObjectsOfType<Creature>();
                yield return new WaitForEndOfFrame();
            }
            var sorted = creatures.OrderByDescending(creature => creature.stats.initiative);
            creaturesQueue = new Queue<Creature>(sorted);
            NextTurn();
        }

        void Update()
        {
            HandleClick();
            UpdateSkillSelectedTiles();
        }

        public void NextTurn()
        {
            NextTurn(0);
        }

        public void NextTurn(float repeatChance)
        {
            activeCreature?.TriggerEndTurnEffects();

            if (skillActive)
            {
                ToggleSkill();
            }

            inputLocked = false;
            skipButton.interactable = true;

            // roll for turn repeat
            var rand = Random.Range(0f, 1f);
            if (rand <= repeatChance)
            {
                LogManager.instance.Log($"{activeCreature.name} got lucky and makes another turn!");
            }
            else
            {
                activeCreature = creaturesQueue.Dequeue();
                creaturesQueue.Enqueue(activeCreature);
            }

            availableActionPoints = activeCreature.stats.speed;

            UpdateState();
            UpdateSkillButtonState();
        }

        private void PauseTurn()
        {
            inputLocked = true;
            skipButton.interactable = false;
            skillButton.interactable = false;
        }

        public void ResumeTurn(float repeatChance)
        {
            inputLocked = false;
            skipButton.interactable = true;
            UpdateSkillButtonState();
            UpdateState();

            if (availableActionPoints <= 0 && HasEnemyNearby() == false)
            {
                NextTurn(repeatChance);
            }
        }

        private bool HasEnemyNearby()
        {
            return creaturesQueue.Any(creature =>
                creature.GetTeam() != activeCreature.GetTeam() 
                && GridManager.instance.CheckIfNeighbors(creature.gridPosition, activeCreature.gridPosition));
        }

        private void HandleClick()
        {
            if (Input.GetMouseButtonDown(0) == false || inputLocked)
            {
                return;
            }

            var cellFromMousePosition = GridManager.instance.GetCellFromMousePosition();
            if (cellFromMousePosition == null)
            {
                return;
            }

            if (skillActive)
            {
                HandleSkill();
                return;
            }

            if (cellFromMousePosition.occupier != null && cellFromMousePosition.occupier.GetTeam() != activeCreature.GetTeam())
            {
                HandleAttack(cellFromMousePosition.position, cellFromMousePosition.occupier);
            }
            else
            {
                MoveActiveCreature(cellFromMousePosition.position);
            }
        }

        private void HandleSkill()
        {
            var activationSucceed = activeCreature.activeAbility.ApplyEffect(skillSelectedArea);
            if (activationSucceed)
            {
                NextTurn(activeCreature.stats.turnRepeatChance);
            }
        }

        private void HandleAttack(Vector2Int position, Creature enemy)
        {
            if (activeCreature.IsRanged())
            {
                PauseTurn();
                activeCreature.Attack(enemy, AttackType.Range);
                return;
            }

            var currentNode = availableHitPositions.FirstOrDefault(node => node.obj == position);
            if (currentNode == null)
            {
                return;
            }

            PauseTurn();

            availableActionPoints -= currentNode.level;

            var positions = currentNode.BuildPath();
            // remove enemy position from path
            positions.RemoveAt(positions.Count - 1);
            activeCreature.SetMeleeAttackSequence(positions, enemy);
        }

        private void MoveActiveCreature(Vector2Int position)
        {
            var currentNode = availableMovePositions.FirstOrDefault(node => node.obj == position);

            if (currentNode == null)
            {
                return;
            }

            PauseTurn();

            availableActionPoints -= currentNode.level;

            var positions = currentNode.BuildPath();
            activeCreature.SetMoveSequence(positions);
        }

        private void UpdateState()
        {
            GridManager.instance.ClearTiles();

            UpdateAvailableCells(activeCreature.gridPosition, availableActionPoints);

            foreach (var creature in creaturesQueue)
            {
                // exluding alies
                if (creature.GetTeam() == activeCreature.GetTeam())
                {
                    GridManager.instance.SetTile(creature.gridPosition, TileType.Default);
                }
                // marking reachable enemies
                else if (availableHitPositions.Any(node => node.obj == creature.gridPosition) 
                         || activeCreature.IsRanged())
                {
                    GridManager.instance.SetTile(creature.gridPosition, TileType.Hostile);
                }
            }

            GridManager.instance.SetTile(activeCreature.gridPosition, TileType.Selected);
        }

        private void UpdateAvailableCells(Vector2Int start, int availableSteps)
        {
            var obstacles = creaturesQueue.Select(creature => creature.gridPosition);
            var pathfindingResult = Pathfinder.FindAvailablePositions(
                start, 
                obstacles,
                GridManager.GridWidth, 
                GridManager.GridHeight,
                availableSteps);

            availableMovePositions = pathfindingResult.moveReachableNodes;
            availableHitPositions = pathfindingResult.hitReachableNodes;

            foreach (var position in availableMovePositions)
            {
                GridManager.instance.SetTile(position.obj, TileType.Available);
            }
        }

        private void UpdateSkillSelectedTiles()
        {
            if (skillActive == false) return;

            GridManager.instance.ClearTiles();

            var gridPositionFromMousePosition = GridManager.instance.GetGridPositionFromMousePosition(false);

            var area = activeCreature.activeAbility.GetArea(gridPositionFromMousePosition.Value);
            var filteredArea = new List<GridCell>();

            foreach (var position in area)
            {
                if (GridManager.instance.IsPositionWithinField(position))
                {
                    var cell = GridManager.instance.GetCellFromGridPosition(position);
                    filteredArea.Add(cell);
                    GridManager.instance.SetTile(position, TileType.Available);
                }
            }

            skillSelectedArea = filteredArea;
        }

        private void UpdateSkillButtonState()
        {
            skillButton.interactable = activeCreature.activeAbility != null;
        }

        // this also used by skill button
        public void ToggleSkill()
        {
            skillActive = !skillActive;

            if (skillActive == false)
            {
                UpdateState();
            }
        }

        public void RemoveCreatureFromQueue(Creature creature)
        {
            var list = creaturesQueue.ToList();
            list.Remove(creature);
            creaturesQueue = new Queue<Creature>(list);
        }

        public void AddCreatureToQueue(Creature newCreature)
        {
            var list = creaturesQueue.ToList();
            var index = 0;
            foreach (var creature in list)
            {
                if (newCreature.stats.initiative > creature.stats.initiative)
                {
                    break;
                }

                index++;
            }

            list.Insert(index, newCreature);
            creaturesQueue = new Queue<Creature>(list);
        }

        public void Restart()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}