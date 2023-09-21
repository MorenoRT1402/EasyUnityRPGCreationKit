using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Category {DAMAGE, ALTERATE_STATS, AILMENT,
    SPECIAL
}

public enum TargetType {USER, ALLY, ALL_ALLIES, DEAD_ALLY, DEAD_ALLIES, ENEMY, ALL_ENEMIES, DEAD_ENEMY, DEAD_ENEMIES,
    ALL
}
public enum MoveAnimationType{STATIC, MOVE_TO_TARGET}
public enum HitAnimationType{WEAPON, SPECIAL, FIST, NOTHING}

[Serializable]
//[CreateAssetMenu(menuName = "Skills/Base")]
public class BaseActiveSkill : SkillBase
{
    public enum UsableOn{ALWAYS, BATTLE, MENU, NEVER}

    protected Category category;
    public string family = ""; //can be empty

    public TargetType targetType = TargetType.ENEMY;
    public UsableOn usableOn = UsableOn.BATTLE;

    public float power = 1;
    public List<float> variacion = new(2);
    public float manaCost;
    public float staminaCost;

    [TextArea] public string formula = "aatk * 4 - bdef * 2 + pwr";
    [Range(0, 100)] public float criticProb = 5f;

    public float precision = 95;
    public int hits = 1;

    public float castTime;

    public MoveAnimationType moveAnimationType;
    public HitAnimationType hitAnimationType;
    public AnimationClip animationOnTarget;

    public virtual void Exec(PersonajeHandler user, PersonajeHandler target)
    {
    }

        internal bool IsUsable(UsableOn context)
    {
        return usableOn == UsableOn.ALWAYS || usableOn == context;
    }

    public override string ToString()
    {
        return $"Name {skillName} \nCategory {category} \nTargetType{targetType}";
    }
}