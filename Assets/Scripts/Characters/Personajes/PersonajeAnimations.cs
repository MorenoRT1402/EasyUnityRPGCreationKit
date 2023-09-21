using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AnimationSheet
{
    IDDLE_DOWN, WALK_DOWN, IDDLE_LEFT, WALK_LEFT,
    IDDLE_RIGHT, WALK_RIGHT, IDDLE_UP, WALK_UP, DEAD,
    IN_BATTLE,
    WEAPON_HIT, SPECIAL_HIT, FIST_HIT,
    CHEST_CLOSED, CHEST_OPENING, CHEST_OPENED
}

public class PersonajeAnimations : Singleton<PersonajeAnimations> //Working to make it Scriptable Object
{

    public GameObject prefabAnimations;
    private readonly float animSpeed = 6;

    private Dictionary<AnimationSheet, Dictionary<string, object>> animationParameters;

    private new void Awake()
    {
        base.Awake();
        InitDict();
    }

    private void InitDict()
    {
        animationParameters = new(){
            { AnimationSheet.DEAD, SetAnimationParameters(true, true, false, false, 0, 0, -1f) },
            { AnimationSheet.IN_BATTLE, SetAnimationParameters(false, true, false, false, -1, 0, -1f) },
            { AnimationSheet.IDDLE_UP, SetAnimationParameters(false, false, true, false, 0, 1, -1f) },
            { AnimationSheet.IDDLE_RIGHT, SetAnimationParameters(false, false, false, false, 1, 0, -1f) },
            { AnimationSheet.IDDLE_DOWN, SetAnimationParameters(false, false, false, false, 0, -1, -1f) },
            { AnimationSheet.IDDLE_LEFT, SetAnimationParameters(false, false, false, false, -1, 0, -1f) },
            { AnimationSheet.WALK_LEFT, SetAnimationParameters(false, false, true, false, -1, 0, -1f) },
            { AnimationSheet.WALK_RIGHT, SetAnimationParameters(false, false, true, false, 1, 0, -1f) },
            { AnimationSheet.WEAPON_HIT, SetAnimationParameters(false, true, false, true, 0, 0, 0.3f)},
            { AnimationSheet.SPECIAL_HIT, SetAnimationParameters(false, true, false, true, 0, 0, 0.65f)},
            { AnimationSheet.FIST_HIT, SetAnimationParameters(false, true, false, true, 0, 0, 1f)},
            { AnimationSheet.CHEST_CLOSED, SetAnimationParameters(false, false, false, false, 0, 0, -1f)},
            { AnimationSheet.CHEST_OPENING, SetAnimationParameters(false, false, false, false, 0.5f, 0, -1f)},
            { AnimationSheet.CHEST_OPENED, SetAnimationParameters(false, false, false, false, 0.75F, 0, -1f)},
        };
    }

    Dictionary<string, object> SetAnimationParameters(bool dead, bool inBattle, bool isMoving, bool isAttacking, float xSpeed, float ySpeed, float hitType)
    {
        Dictionary<string, object> boolParameters = new()
        {
            { "Dead", dead },
            {"InBattle", inBattle},
            {"IsMoving", isMoving},
            {"IsAttacking", isAttacking},
        };
        Dictionary<string, object> floatParameters = new()
        {
            {"XSpeed", xSpeed},
            {"YSpeed", ySpeed},
        };
        if (hitType >= 0)
        floatParameters.Add("HitType", hitType);

        Dictionary<string, object> parameters = new();
        parameters.AddRange(boolParameters);
        parameters.AddRange(floatParameters);

        return parameters;
    }
    internal void ChangeBlendTreeAnimation(AnimationSheet animation, GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        foreach (KeyValuePair<AnimationSheet, Dictionary<string, object>> pair in animationParameters)
            if (animation == pair.Key)
            {
                foreach (KeyValuePair<string, object> value in pair.Value)
                {
                    string btVariableName = value.Key;
                    if (value.Value is bool boolValue)
                        animator.SetBool(btVariableName, boolValue);
                    if (value.Value is float floatValue)
                        animator.SetFloat(btVariableName, floatValue);
                }
            }
    }

    internal void ChangeBlendTreeAnimation(HitAnimationType hitAnimationType, GameObject gameObject)
    {
        switch (hitAnimationType)
        {
            case HitAnimationType.WEAPON:
                ChangeBlendTreeAnimation(AnimationSheet.WEAPON_HIT, gameObject);
                break;
            case HitAnimationType.SPECIAL:
                ChangeBlendTreeAnimation(AnimationSheet.SPECIAL_HIT, gameObject);
                break;
            case HitAnimationType.FIST:
                ChangeBlendTreeAnimation(AnimationSheet.FIST_HIT, gameObject);
                break;
        }
    }
    public IEnumerator ChangeBlendTreeAnimation(MoveAnimationType moveAnimationType, Vector3 targetPosition, GameObject gO)
    {
        Animator animator = gO.GetComponent<Animator>();
        switch (moveAnimationType)
        {
            case MoveAnimationType.STATIC:
                animator.SetBool("IsMoving", false);
                yield break;
            case MoveAnimationType.MOVE_TO_TARGET:
                float direction = targetPosition.x < transform.position.x ? -1 : 1;
                animator.SetBool("IsMoving", true);
                animator.SetFloat("XSpeed", direction);
                bool moving = MoveTowardsTarget3D(targetPosition, gO);
                Debug.Log(moving);
                while (moving)
                { //while target and gameobject positions are differents
                    animator.SetBool("IsMoving", true);
                    Debug.Log(animator.GetBool("IsMoving"));
                    yield return null;
                };
                Debug.Log("Sigue");
                animator.SetBool("IsMoving", false);

                yield break;
        }
    }


    private bool MoveTowardsTarget3D(Vector3 target, GameObject attacker)
    {
        float speed = animSpeed * Time.deltaTime;

        Debug.Log($"{target} != {attacker.transform.position} ? {target != (attacker.transform.position = Vector3.MoveTowards(attacker.transform.position, target, speed))}");
        //        Debug.Log($"{transform.position} = {Vector3.MoveTowards(transform.position, target, speed)} != {target}? {target != (transform.position = Vector3.MoveTowards(transform.position, target, speed))}");
        //        attacker.transform.position = Vector3.MoveTowards(attacker.transform.position, target, speed);
        return target != (attacker.transform.position = Vector3.MoveTowards(attacker.transform.position, target, speed));
    }

    internal void AnimateHit(TurnHandler myAttack)
    {
        Debug.Log(myAttack.Skill.hitAnimationType);
        Animator animator = myAttack.AttackersGameObject.GetComponent<Animator>();
        animator.SetBool("IsAttacking", true);
        switch (myAttack.Skill.hitAnimationType)
        {
            case HitAnimationType.WEAPON:
                animator.SetFloat("HitType", 0.3f);
                break;
            case HitAnimationType.SPECIAL:
                animator.SetFloat("HitType", 0.65f);
                break;
            case HitAnimationType.FIST:
                animator.SetFloat("HitType", 1f);
                break;
        }

        // Suscríbete al evento de finalización de la animación
        animator.GetCurrentAnimatorClipInfo(0);
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float clipLength = clipInfo[0].clip.length;
        StartCoroutine(ResetHitType(animator, clipLength));
    }

    private IEnumerator ResetHitType(Animator animator, float clipLength)
    {
        Debug.Log(clipLength);
        yield return new WaitForSeconds(clipLength + 0.2f); // Espera a que termine la animación
        animator.SetBool("IsAttacking", false);
        animator.SetFloat("HitType", -1); // Restablece el valor al estado original
    }



    public void FlipAnimation(bool flip, GameObject gO, Vector2 pos) // Axes that will be flipped
    {
        SpriteRenderer spriteRenderer;
        Debug.Log($"FlipAnimation {flip} {gO.GetInstanceID()} {pos}");

        spriteRenderer = gO.GetComponent<SpriteRenderer>();
        Debug.Log($"FlipAnimation {flip} {gO.GetInstanceID()} {pos} {spriteRenderer.GetInstanceID()}");

        if (pos.x > 0)
            spriteRenderer.flipX = flip;
        if (pos.y > 0)
            spriteRenderer.flipY = flip;
    }


    public float GetCurrentAnimationDuration(GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on the GameObject.");
            return 0f;
        }

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            AnimationClip clip = clipInfo[0].clip;
            return clip.length;
        }

        Debug.LogWarning("No current animation found.");
        return 0f;
    }

    internal void PlayAnimation(Vector3 targetPosition, AnimationClip animationClip)
    {
        if (animationClip != null)
        {

            // Instancia el prefab en la posición del target
            GameObject instance = Instantiate(prefabAnimations, targetPosition, Quaternion.identity);

            // Obtiene el Animator del prefab instanciado
            Animator animator = instance.GetComponent<Animator>();

            // Ejecuta la animación en el Animator
            animator.Play(animationClip.name);

            // Obtiene la duración de la animación
            float animationDuration = GetAnimationDuration(animationClip);

            // Programa la destrucción del prefab instanciado después de la duración de la animación
            StartCoroutine(DestroyInstance(instance, animationDuration));
        }
    }

    private IEnumerator DestroyInstance(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Destruye la instancia del prefab
        Destroy(instance);
    }

    private float GetAnimationDuration(AnimationClip animationClip)
    {
        return animationClip.length;
    }

    internal void ChangeAnimation(EventSO.RouteKeys key, float value, GameObject gameObject)
    {
        if (key == EventSO.RouteKeys.LOOK)
        {
            var animationSheet = value switch
            {
                0 => AnimationSheet.IDDLE_UP,
                1 => AnimationSheet.IDDLE_RIGHT,
                2 => AnimationSheet.IDDLE_DOWN,
                _ => AnimationSheet.IDDLE_LEFT,
            };
            ChangeBlendTreeAnimation(animationSheet, gameObject);
        }
    }
}
