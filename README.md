# HommPrototype

A small prototype of Heroes of Might and Magic's combat system.

## Units

### Knight

- **Health**: 20
- **Speed**: 5
- **Initiative**: 4
- **Melee Damage**: 6-10
- **Range Damage**: 0
- **Combat Spirit**: 45%

#### Armor (Passive)
The first time damage is taken, it reduced by 99%, the second time - by 66%, the third time - by 33%, then - by 0%.

### Archer

- **Health**: 10
- **Speed**: 6
- **Initiative**: 4
- **Melee Damage**: 1-6
- **Range Damage**: 6-15
- **Combat Spirit**: 0%

#### Arrow Rain (Active)
The ability to shoot at a territory equal to 12 hex (double flower shape), while causing 50% reduced damage to all affected enemies.

### Mage

- **Health**: 5
- **Speed**: 5
- **Initiative**: 3
- **Melee Damage**: 1-4
- **Range Damage**: 9-18
- **Combat Spirit**: 0%

#### Mirror Image (Active)
Creates a fragile copy of a selected unit without any abilities but with the same stats and units amount. Copy's team will always be the same as it's summoner. Copy is destroyed when any damage is taken.

### Skeleton

- **Health**: 6
- **Speed**: 3
- **Initiative**: 6
- **Melee Damage**: 6
- **Range Damage**: 0
- **Combat Spirit**: 0%

#### Replication (Passive)
As soon as the stack of skeletons takes damage, the number of skeletons that died in it will appear next to it in a random place around the stack. The spawned skeletons also lose their characteristics depending on what stage of the charge the ability of the maternal skeletons they were born at. Only the mother stack has this ability. 

### Zombie

- **Health**: 3000
- **Speed**: 2
- **Initiative**: 4
- **Melee Damage**: 0
- **Range Damage**: 0
- **Combat Spirit**: 0%

#### Possession (Passive)
As soon as the zombie deals damage to an enemy unit, it will possess it for 1 turn and the player playing for zombie's side can use possessed unit as if it were his own.

### Ghost

- **Health**: 7
- **Speed**: 8
- **Initiative**: 5
- **Melee Damage**: 3-7
- **Range Damage**: 0
- **Combat Spirit**: 35%

#### Regeneration (Passive)
Heals damaged unit to full health at the end of turn.

#### Swap (Active)
Swaps places with a selected unit.
