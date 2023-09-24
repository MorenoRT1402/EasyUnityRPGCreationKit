using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;
using System;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using System.Linq;

public enum DataType { INT, FLOAT, STRING, BOOL }
public class NCalcManager : Singleton<NCalcManager>
{
    public enum DataType { INT, FLOAT, STRING, BOOL }

    [Header("Parameters/Common")]
    public static string time = "time";
    public static string random0To1 = "rndm";
    public static string money = "gold";

    [Header("Parameters/Skills")]
    public static string power = "pwr";

    [Header("Parameters/Actor")]
    public static string UserPrefix = "a";
    public static string TargetPrefix = "b";
    public static string EmptyPrefix = "";

    [Header("Parameters/Stats")]
    public static string level = "lvl";
    public static string jobLevel = "jlv";
    public static string hp = "hp";
    public static string maxHP = "mhp";
    public static string mp = "mp";
    public static string maxMP = "mmp";
    public static string stamina = "sta";
    public static string maxStamina = "mst";
    public static string strength = "str";
    public static string attack = "atk";
    public static string defense = "def";
    public static string mind = "mnd";
    public static string magicAttack = "mat";
    public static string magicalDefense = "mdf";
    public static string dexterity = "dex";
    public static string speed = "spd";
    [Header("Parameters/Developer Variables")]
    public static string numericVariables = "numV";
    public static string switches = "bools";
    public static string stringsVariables = "strV";

    private BattleStateMachine BSM;

    private readonly Dictionary<Stats, string> statParameterMap = new()
    {
        { Stats.LEVEL, level },
        { Stats.JOB_LEVEL, jobLevel},
        { Stats.HP, hp },
        { Stats.HP_MAX, maxHP },
        { Stats.MP, mp },
        { Stats.MP_MAX, maxMP },
        { Stats.STAMINA, stamina },
        { Stats.STAMINA_MAX, maxStamina },
        { Stats.STRENGTH, strength },
        { Stats.ATTACK, attack},
        { Stats.DEX, dexterity },
        { Stats.MIND, mind },
        { Stats.MAGIC_ATTACK, magicAttack},
        { Stats.DEFENSE, defense },
        { Stats.MAGIC_DEFENSE, magicalDefense },
        { Stats.SPEED, speed },
    };

    private readonly Dictionary<DataType, string> dataTypeKeyMap = new()
    {
        { DataType.INT, "INT" },
        { DataType.FLOAT, "FLOAT" },
        { DataType.STRING, "STRING" },
        { DataType.BOOL, "BOOL" },
    };

    public float ConvertToFloat(object obj)
    {
        float result = 0.0f;
        try
        {
            if (obj != null && float.TryParse(obj.ToString(), out float convertedValue))
            {
                result = convertedValue;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error converting object to float: " + ex.Message);
        }

        return result;
    }

    private void AssignCommonParameters(Expression expression)
    {
        expression.Parameters[time] = Time.deltaTime;
        expression.Parameters[random0To1] = (float)UnityEngine.Random.Range(0, 1);
        expression.Parameters[money] = InventoryManager.Instance.moneyInPosesion;
    }

    public static string LocateVariables(string formula)
    {
        string newFormula = ReplaceVariable(formula, numericVariables, SaveManager.numericVariables);
        newFormula = ReplaceVariable(newFormula, switches, SaveManager.switches);
        newFormula = ReplaceVariable(newFormula, stringsVariables, SaveManager.strings);

        return newFormula;
    }

    /*private static string ReplaceVariable(string formula, string parameter, string[] list)
    {
        Debug.Log($"Follow {formula}");
        string pattern = $"{parameter}\\[+[0-9]+\\]$";
        List<string> appearences = new();
        string searchIndexPattern = "[0-9]+";
        string newFormula = formula;

        try
        {

            MatchCollection matches = Regex.Matches(newFormula, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                appearences.Add(match.Value);
            }

            foreach (string appearence in appearences)
            {
                Match match = Regex.Match(appearence, searchIndexPattern);
                int index = int.Parse(match.ToString());
                string newValue = "";
                if(index < list.Length && list[index] != null) newValue = list[index];
                Debug.Log($"Follow 1.4 {newValue}");
                newFormula = newFormula.Replace(appearence, newValue.ToString());
            }
        }
        catch (ArgumentException argE)
        {
            Debug.LogError("ArgumentException " + argE);
            return newFormula;
        }
        return newFormula;
    }

    private static string ReplaceVariable(string formula, string parameter, bool[] list)
    {
        Debug.Log($"Follow {formula}");
        string pattern = $"{parameter}\\[+[0-9]+\\]$";
        List<string> appearences = new();
        string searchIndexPattern = "[0-9]+";
        string newFormula = formula;

        try
        {

            MatchCollection matches = Regex.Matches(newFormula, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                appearences.Add(match.Value);
            }

            foreach (string appearence in appearences)
            {
                Match match = Regex.Match(appearence, searchIndexPattern);
                int index = int.Parse(match.ToString());
                bool newValue = false;
                if(index < list.Length) newValue = list[index];
                newFormula = newFormula.Replace(appearence, newValue.ToString());
            }
        }
        catch (ArgumentException argE)
        {
            Debug.LogError("ArgumentException " + argE);
            return newFormula;
        }
        return newFormula;
    }

    private static string ReplaceVariable(string formula, string parameter, float[] list)
    {
        Debug.Log($"Follow {formula}");
        string pattern = $"{parameter}\\[+[0-9]+\\]$";
        List<string> appearences = new();
        string searchIndexPattern = "[0-9]+";
        string newFormula = formula;

        try
        {

            MatchCollection matches = Regex.Matches(newFormula, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                appearences.Add(match.Value);
            }

            foreach (string appearence in appearences)
            {
                Match match = Regex.Match(appearence, searchIndexPattern);
                int index = int.Parse(match.ToString());
                float newValue = 0;
                if (index < list.Length)newValue = list[index];
                Debug.Log($"Follow 1.4 {newValue}");
                newFormula = newFormula.Replace(appearence, newValue.ToString());
            }
        }
        catch (ArgumentException argE)
        {
            Debug.LogError("ArgumentException " + argE);
            return newFormula;
        }
        return newFormula;
    }*/

    private static string ReplaceVariable<T>(string formula, string parameter, T[] list)
{
    string pattern = $"{parameter}\\[+[0-9]+\\]$";
    List<string> appearences = new();
    string searchIndexPattern = "[0-9]+";
    string newFormula = formula;

    try
    {
        MatchCollection matches = Regex.Matches(newFormula, pattern);

        foreach (Match match in matches.Cast<Match>())
        {
            appearences.Add(match.Value);
        }

        foreach (string appearence in appearences)
        {
            Match match = Regex.Match(appearence, searchIndexPattern);
            int index = int.Parse(match.ToString());
            T newValue = default;
            if (index < list.Length) newValue = list[index];
            newFormula = newFormula.Replace(appearence, newValue.ToString());
        }
    }
    catch (ArgumentException argE)
    {
        Debug.LogError("ArgumentException " + argE);
        return newFormula;
    }
    return newFormula;
}


    private void AssignAttributesParameters(string prefix, PersonajeHandler handler, Expression expression)
    {
        foreach (var kvp in statParameterMap)
        {
            var stat = kvp.Key;
            var parameterName = kvp.Value;
            expression.Parameters[prefix + parameterName] = handler.Stats.GetActual(stat);
        }
    }

    private void AssignAttributesParameters(DamageActiveSkill skill, Expression expression)
    {
        expression.Parameters[power] = skill.power;
    }

    private void AssignAttributesParameters(string prefix, List<GameObject> groupList, Expression expression)
    {
        foreach (var kvp in statParameterMap)
        {
            var stat = kvp.Key;
            var parameterName = kvp.Value;
            expression.Parameters[prefix + parameterName] = GetTotalAtrFighters(stat, groupList);
        }
    }

    private List<GameObject> GetGroupList(FighterStateMachine fsm)
    {
        BSM = BattleStateMachine.Instance;

        if (fsm is AllyStateMachine)
            return BSM.AllysInGame;
        else if (fsm is EnemyStateMachine)
            return BSM.EnemysInGame;

        return new List<GameObject>();
    }

    private float GetTotalAtrFighters(Stats stat, List<GameObject> groupList)
    {
        float total = 0;
        for (int i = 0; i < groupList.Count; i++)
        {
            total += groupList[i].GetComponent<FighterStateMachine>().Hero.Stats.GetActual(stat);
        }
        return total;
    }

    private string GetFormula(TurnHandler attack, string formula, GameObject target)
    {
        //        DamageActiveSkill skill = (DamageActiveSkill)attack.Skill;
        //        float affinityMult = target.GetComponent<FighterStateMachine>().Hero.GetAffinityMult(skill.affinities);
        string checkedFormula = LocateVariables(formula);
        float affinityMult = attack.GetAffinitiesMult(target);
        string formattedMult = affinityMult.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        return $"{checkedFormula} * {formattedMult}";
    }

    private void AssignParameters(GameObject user, GameObject target, DamageActiveSkill skill, Expression expression)
    {
        AssignCommonParameters(expression);

        AssignAttributesParameters(skill, expression);

        if (user.TryGetComponent<PersonajeHandler>(out var userHandler))
            AssignAttributesParameters(UserPrefix, userHandler, expression);
        if (target.TryGetComponent<PersonajeHandler>(out var targetHandler))
            AssignAttributesParameters(TargetPrefix, targetHandler, expression);
    }

    private void AssignParameters(GameObject target, Expression expression)
    {
        AssignCommonParameters(expression);

        if (target.TryGetComponent<PersonajeHandler>(out var handler))
            AssignAttributesParameters(EmptyPrefix, handler, expression);
    }

    private void AssignParameters(TurnHandler actualTurn, BattleCommands specialCommand, Expression expression)
    {
        switch (specialCommand)
        {
            case BattleCommands.DEFEND:
            case BattleCommands.WAIT:
                AssignParameters(actualTurn.Skill, actualTurn.AttackersGameObject, expression);
                break;
            case BattleCommands.FLEE:
                AssignParametersGlobal(actualTurn, expression);
                break;
        }
    }

    private void AssignParameters(BaseActiveSkill skill, GameObject target, Expression expression)
    {

        expression.Parameters[power] = skill.power;
        AssignCommonParameters(expression);

        if (target.TryGetComponent<PersonajeHandler>(out var handler))
            AssignAttributesParameters(EmptyPrefix, handler, expression);
    }

    private void AssignParametersGlobal(TurnHandler actualTurn, Expression expression)
    {
        FighterTeam type = actualTurn.Type;
        GameObject attacker = actualTurn.AttackersGameObject;

        expression.Parameters[power] = actualTurn.Skill.power;
        AssignCommonParameters(expression);

        FighterStateMachine attackerFSM = attacker.GetComponent<FighterStateMachine>();

        List<GameObject> attackerGroupList = GetGroupList(attackerFSM);

        AssignAttributesParameters(UserPrefix, attackerGroupList, expression);


        List<GameObject> targetGroupList = new();
        // Asignar parámetros del objetivo según corresponda
        if (actualTurn.Skill.targetType == TargetType.USER)
            targetGroupList.Add(attackerFSM.gameObject);
        else
            targetGroupList = BSM.GetGroupList(type, actualTurn.Skill.targetType);

        AssignAttributesParameters(TargetPrefix, targetGroupList, expression);
    }


    public void AssignParameters(TurnHandler attack, GameObject target, Expression expression)
    {
        //        FighterTeam type = attack.Type;
        GameObject attacker = attack.AttackersGameObject;

        expression.Parameters[power] = attack.Skill.power;
        AssignCommonParameters(expression);

        if (attacker.TryGetComponent<PersonajeHandler>(out var handler))
            AssignAttributesParameters(UserPrefix, handler, expression);

        //target parameters
        if (target != null)
        {
            if (target.CompareTag("Ally"))
            {
                if (target.TryGetComponent<PersonajeHandler>(out var allyTarget))
                    AssignAttributesParameters(TargetPrefix, allyTarget, expression);

            }
            else if (target.CompareTag("Enemy"))
            {
                if (target.TryGetComponent<PersonajeHandler>(out var enemyTarget))
                    AssignAttributesParameters(TargetPrefix, enemyTarget, expression);

            }
        }
    }

    private static object GetObject(Expression expression)
    {
        try
        {
            object result = expression.Evaluate(null);
            return result;
        }
        catch (EvaluationException e)
        {
            Debug.LogError("Error evaluating expression: " + e.Message);
            return "Error evaluating expression: " + e.Message;
        }
    }

    internal object CalculateObject(GameObject target, string damagePerInterval)
    {
        string checkedFormula = LocateVariables(damagePerInterval);
        Expression expression = new(checkedFormula);
        AssignParameters(target, expression);

        return GetObject(expression);
    }

    public object CalculateObject(TurnHandler attack, GameObject target, string formula)
    {
        Expression expression = new(GetFormula(attack, formula, target));
        AssignParameters(target, expression);

        return GetObject(expression);
    }

    internal object CalculateObject(TurnHandler actualTurn)
    {
        SpecialActiveSkill sSkill = (SpecialActiveSkill)actualTurn.Skill;
        string formula = LocateVariables(sSkill.formula);
        Expression expression = new(formula);
        AssignParameters(actualTurn, sSkill.SpecialCommand, expression);

        return GetObject(expression);
    }
    public object CalculateObject(TurnHandler attack, GameObject target)
    {
        DamageActiveSkill skill = (DamageActiveSkill)attack.Skill;
        string formula = GetFormula(attack, skill.formula, target);
        Expression expression = new(formula);
        AssignParameters(attack, target, expression);

        return GetObject(expression);
    }

    internal float CalculateObjectToFloat(GameObject target, string formula)
    {
        string checkedFormula = LocateVariables(formula);
        Expression expression = new(checkedFormula);
        AssignParameters(target, expression);

        return ConvertToFloat(GetObject(expression));
    }

    internal float CalculateObjectToFloat(GameObject user, GameObject target, DamageActiveSkill skill)
    {
        string checkedFormula = LocateVariables(skill.formula);
        Expression expression = new(checkedFormula);
        AssignParameters(user, target, skill, expression);

        return ConvertToFloat(GetObject(expression));
    }
}
