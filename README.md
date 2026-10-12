MERGE GAME

A small Unity merge game for the Vuvy case study.

The required 2D artwork was generated with AI.

RUN

Open with Unity 6000.0.58f2 and play Assets/Scenes/Loading.unity. The project uses Built-in 2D and uGUI.

Spawn adds a random level 1 item to an empty cell. Matching items merge and give points. The MAX badge marks the last level. Restart clears the board and score.

ARCHITECTURE

I used a simple MVC structure to separate gameplay from visuals.

BoardState holds the cells. BoardDropRules checks drops. BoardActions applies them and updates the score. Controllers handle input and views show the result.

The model changes before animations start. Interrupting an animation does not undo a merge. Dragging uses a pooled visual so grid objects stay in place.

A single MonoBehaviour would need less setup but would mix these responsibilities.

DATA

ItemChainSO stores item levels with names and sprites. It also stores the score for each result level. ItemCatalogSO looks up items by type and level. Food, Tools and Decoration each have five levels. Chain length sets the maximum level.

I chose ScriptableObjects to edit content in the Inspector instead of hardcoding it.

To add a chain, add a unique ItemType value and a chain asset to the catalog. Keep existing enum values unchanged. The first reward is zero and later rewards increase. Extra levels only need new entries in the chain.

LevelDataSO stores dimensions and starting items. Edit Assets/_Game/Levels/Level 01.asset for board setup. The game creates a separate BoardState so playing does not change the asset.

PATTERNS AND DEPENDENCIES

GameController connects objects through constructors and Initialize methods. This is manual dependency injection. The setup is small enough without a container. Zenject could be an alternative.

Three interfaces support replaceable rules through the Strategy pattern.

- IItemSpawnPolicy chooses the item
- IBoardDropRules checks the drop
- IMergeScoreCalculator calculates points

Callers depend on these contracts rather than concrete rules. This applies dependency inversion. Rules could also live inside the callers but would be harder to replace.

EventManager uses the Observer approach. For example ScoreChanged updates the score view. Direct calls are an alternative but need more references.

I wrote the PoolFactory base for my own projects and added it here. It combines a simple factory with object pooling. Objects are reused and the pool grows when empty. Instantiate and Destroy are alternatives for less frequent use.

PoolFactory and SceneLoader use DontDestroyOnLoad because every scene may need them. They keep one instance each. Injecting them everywhere would reduce global access but add setup.

DOTween handles animations. UniTask waits one frame during startup and waits for scene loading and fades. Gameplay uses no coroutines. Coroutines could handle these waits too.

KNOWN LIMITS

There is no save or level progression. A full board with no merge needs Restart. Occupied items cannot swap.

The board fits when created but does not refit during window resizing. The model still uses some Unity types.

Compilation and temporary logic checks passed. No test suite is included. Final Play Mode and device checks remain.
