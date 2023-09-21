using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;
using Unity.VisualScripting;
using System.Linq;

[System.Serializable]
public class TurnHandler
{
    public string Attacker; //ID of attacker
    public FighterTeam Type;

    public GameObject AttackersGameObject; //who attacks

    //    public GameObject AttackersTargets;  //who is going to be attacked
    public List<GameObject> AttackersTargets;  //whos is going to be attacked
    public BaseActiveSkill Skill;
    public ItemSO Item;
    public bool Counterable = true;
    public bool Reaction = false;

    private float multCritDamage = 1.5f;

    //which attack is performed

    private NCalcManager NCM;

    public TurnHandler(string attacker, FighterTeam type, GameObject attackersGameObject, List<GameObject> attackersTargets, BaseActiveSkill skill)
    {
        SetParameters(attacker, type, attackersGameObject, attackersTargets, skill);
    }

    public TurnHandler(FighterStateMachine attacker, FighterStateMachine target, BaseActiveSkill skill)
    {
        string attackerName = attacker.gameObject.name;
        FighterTeam type = attacker.Faction;
        GameObject attackerGO = attacker.gameObject;
        List<GameObject> targetsGO = new() { target.gameObject };

        SetParameters(attackerName, type, attackerGO, targetsGO, skill);
    }

    public TurnHandler()
    {
    }

    private void SetParameters(string attacker, FighterTeam type, GameObject attackersGameObject, List<GameObject> attackersTargets, BaseActiveSkill skill)
    {
        Attacker = attacker;
        Type = type;
        AttackersGameObject = attackersGameObject;
        AttackersTargets = attackersTargets;
        Skill = skill;

        Counterable = true;

        multCritDamage = BattleStateMachine.Instance.CritMult;
    }

    public object CalculateResultObject(GameObject target, bool doDamage)
    {
        FighterStateMachine attackerSM = AttackersGameObject.GetComponent<FighterStateMachine>();
        StatsHandler attacker = attackerSM.Hero.Stats;

        if (Skill.precision * attacker.ActualPrecision / 100 >= Random.Range(0, 101f))
        {
            int damage = 0;

            Expression expression = new(Skill.formula);
            NCM.AssignParameters(this, target, expression);

            object result = 0f;
            try
            {
                result = expression.Evaluate(null);
            }
            catch (EvaluationException e)
            {
                Debug.LogError("Error evaluating expression: " + e.Message);
            }
            // Convertir el objeto en un float

            if (result is float floatResult) // Verificar si el resultado es un float
            {
                floatResult *= Random.Range(Skill.variacion[0], Skill.variacion[1] + 1); // Variación de daño final
                damage = Mathf.RoundToInt(floatResult); // Convertir el float a int, redondeando al entero más cercano

                if (doDamage)
                    ApplyResult(damage, AttackersTargets, Skill);
            }
            else
            {
                Debug.Log("Result is not a valid float");
                // Realizar alguna acción de manejo de errores si el resultado no es un float válido
            }




            return damage;
        }

        return "MISS";
    }

    public ResultDamage CalculateResult(GameObject target, bool doDamage)
    {
        FighterStateMachine attackerFighter = AttackersGameObject.GetComponent<FighterStateMachine>();
        ResultDamage resultDamage = new();
        NCM = NCalcManager.Instance;

        if (Skill.precision * attackerFighter.Hero.Stats.ActualPrecision / 100 > Random.Range(0, 100))
        {
            resultDamage.setMiss(false);

            object result = NCM.CalculateObject(this, target);
            float floatResult = 0f;


            if (result is double doubleResult)
                floatResult = (float)doubleResult;

            else if (result is float f)
                floatResult = f;
            else
            {
                Debug.Log("Result is not a valid float");
            }
            floatResult *= GetAffinitiesMult(target);
            floatResult *= Skill.variacion.Count > 0 ? Random.Range(Skill.variacion[0], Skill.variacion[1] + 1) : 1; // Final damage variation
            int damage = Mathf.RoundToInt(floatResult);

            //                Debug.Log($"{damage} * {multCritDamage} = {(int)(damage * multCritDamage)}");
            int critNum = Random.Range(0, 100);
//                Debug.Log($"{Skill.criticProb} * {attackerFighter.Hero.Stats.ActualCritProb} /100 > {critNum} ? {Skill.criticProb * attackerFighter.Hero.Stats.ActualCritProb/100 > critNum}");
            if (Skill.criticProb * attackerFighter.Hero.Stats.ActualCritProb / 100 > critNum)
            { //critical hits
                damage = (int)(damage * multCritDamage);
                resultDamage.setCrit(true);
            }

            if (doDamage)
                ApplyResult(damage, AttackersTargets, Skill);

            resultDamage.setResult(damage);


            return resultDamage;
        }

        resultDamage.setMiss(true);

        return resultDamage;
    }

    public float GetAffinitiesMult(GameObject target)
    {
        float sum = 1;

        PersonajeHandler attackerHandler = AttackersGameObject.GetComponent<PersonajeHandler>();
        PersonajeHandler targetHandler = target.GetComponent<PersonajeHandler>();

        List<Affinity> affinities = attackerHandler.Stats.GetAffinities();
        if (Skill is DamageActiveSkill damageActiveSkill)
            affinities.AddRange(damageActiveSkill.affinities);
        affinities.Distinct().ToList();

        /*PersonajeAffinities targetResistances = targetHandler.Affinities;

        for (int i = 0; i < affinities.Count; i++)
            sum *= targetResistances.GetMult(affinities[i]);*/

        sum *= targetHandler.GetAffinityMult(affinities);

        return sum;
    }

    public float CalculateNoSkillDamage(GameObject target, string formula, bool doDamage)
    {
        object result = NCalcManager.Instance.CalculateObject(this, target, formula);
        // Convert objet to float

        if (result is float floatResult)
        {
            float damage = Mathf.RoundToInt(floatResult);

            //                Debug.Log($"{damage} * {multCritDamage} = {(int)(damage * multCritDamage)}");
            float critNum = Random.Range(0, 101f);
            //                Debug.Log($"{(skill.criticProb * attackerFighter.hero.Stats.critProbActual)/100} > {critNum} ? {(skill.criticProb * attackerFighter.hero.Stats.critProbActual)/100 > critNum}");



            if (doDamage)
                ApplyResult(damage, target);
            return damage;
        }
        else
        {
            Debug.Log("El resultado no es un float válido");
            return 0;
        }
    }

    private void ApplyResult(int result, List<GameObject> attackersTargets, BaseActiveSkill skill)
    {

        for (int i = 0; i < attackersTargets.Count; i++)
        {
            GameObject target = attackersTargets[i];
            ApplyResult(result, target);
            ApplyAilments(skill, target);
        }
    }

    private void ApplyResult(float damage, GameObject target)
    {

        target.GetComponent<FighterStateMachine>().Hero.TakeDamage(damage);
    }

    private void ApplyAilments(BaseActiveSkill skill, GameObject targetGO)
    {
        if (skill is AilmentActiveSkill aSkill)
        {
            FighterStateMachine target = targetGO.GetComponent<FighterStateMachine>();
            aSkill.ApplyAilmentsAndHeals(target.GetComponent<PersonajeHandler>());
        }
    }

    private void ApplyResult(int result, GameObject attackersTarget)
    {
        FighterStateMachine targetSM = attackersTarget.GetComponent<FighterStateMachine>();
        targetSM.TakeDamage(result);
        if (targetSM != null)
            targetSM.ailmentTakeDamage();
    }

    public override string ToString()
    {
        string targets = "";
        for (int i = 0; i < AttackersTargets.Count; i++)
            targets += AttackersTargets[i].ToString() + "\n";

        return $"{Attacker} ({Type}) => {Skill} To: {targets}";
    }
}
