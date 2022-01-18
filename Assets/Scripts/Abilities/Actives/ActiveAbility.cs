using System.Collections.Generic;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Abilities.Actives
{
    public abstract class ActiveAbility : MonoBehaviour
    {
        public abstract Vector2Int[] GetArea(Vector2Int centerPoint);

        public abstract bool ApplyEffect(IEnumerable<GridCell> cells);

        public abstract string GetName();
    }
}
