using UnityEngine;

public abstract class StateBase 
{
    public virtual void Enter(InGameManager manager) { }
    public virtual void Update(InGameManager manager) { }
    public virtual void FixedUpdate(InGameManager manager) { }
    public virtual void Exit(InGameManager manager) { }
}

public class Preparation : StateBase
{
    public override void Enter(InGameManager manager) { }
    public override void Update(InGameManager manager) { }
    public override void FixedUpdate(InGameManager manager) { }
    public override void Exit(InGameManager manager) { }
}
public class PlayerPre : StateBase
{
    public override void Enter(InGameManager manager) { }
    public override void Update(InGameManager manager) { }
    public override void FixedUpdate(InGameManager manager) { }
    public override void Exit(InGameManager manager) { }
}
public class PlayerAttack : StateBase
{
    public override void Enter(InGameManager manager) { }
    public override void Update(InGameManager manager) { }
    public override void FixedUpdate(InGameManager manager) { }
    public override void Exit(InGameManager manager) { }
}
public class EnemyPre : StateBase
{
    public override void Enter(InGameManager manager) { }
    public override void Update(InGameManager manager) { }
    public override void FixedUpdate(InGameManager manager) { }
    public override void Exit(InGameManager manager) { }
}
public class EnemyAttack : StateBase
{
    public override void Enter(InGameManager manager) { }
    public override void Update(InGameManager manager) { }
    public override void FixedUpdate(InGameManager manager) { }
    public override void Exit(InGameManager manager) { }
}


