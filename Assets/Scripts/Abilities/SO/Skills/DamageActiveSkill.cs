using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { PHYSICAL, MAGICAL, HYBRID, NOTHING }


[CreateAssetMenu(menuName = "Skills/Skills/New Active Skill/Damage")]
public class DamageActiveSkill : BaseActiveSkill
{

    public List<Affinity> affinities;
    public SkillType type;
    public bool canCure = false;

    private void Start()
    {
        category = Category.DAMAGE;
    }

    public override void Exec(PersonajeHandler user, PersonajeHandler target)
    {
        base.Exec(user, target);
        GameObject userGo = user.gameObject;
        GameObject targetGo = target.gameObject;
        float damage = NCalcManager.Instance.CalculateObjectToFloat(userGo, targetGo, this);
        if (damage <= 0 && !canCure)
            damage = 0;
        target.TakeDamage(damage);
    }

    public override string ToString()
    {
        return base.ToString();
    }

}
